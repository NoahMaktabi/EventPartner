using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using CloudinaryDotNet.Actions;
using Domain;
using Flurl.Http;
using Infrastracture.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly EmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly HttpClient _httpClient;

        public AccountController(
            UserManager<AppUser> userManager, 
            TokenService tokenService,
            IConfiguration config,
            EmailSender emailSender,
            ILogger<AccountController> logger,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _emailSender = emailSender;
            _logger = logger;
            _signInManager = signInManager;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri("https://graph.facebook.com")
            };
        }

        /// <summary>
        /// Login user and return a token to use for authorization, also returns display name, username and image. 
        /// </summary>
        /// <param name="loginDto">A model containing email and password</param>
        /// <returns>Token</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                _logger.Log(LogLevel.Trace, "Failed to login, user not found");
                return Unauthorized("Invalid Email");
            }

            if (user.UserName == "bob" || user.UserName == "tom" || user.UserName == "jane")
                user.EmailConfirmed = true;

            if (!user.EmailConfirmed)
            {
                _logger.Log(LogLevel.Trace, "Failed to login, Email not confirmed, username is: " + user.UserName);
                return Unauthorized("Email not confirmed");
            }
            

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                _logger.Log(LogLevel.Trace, "Failed to login, Wrong Password, username is: " + user.UserName);
                return Unauthorized("Invalid Password");
            }
            await SetRefreshToken(user);
            return CreateUserObject(user);

        }

        /// <summary>
        /// Register a user provided a model containing info about the user. The method also send a verification link to the provided email.
        /// </summary>
        /// <param name="registerDto">Model containing email, username, displayName and password</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("email", "Email taken");
                return ValidationProblem();
            }
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            {
                ModelState.AddModelError("username", "Username taken");
                return ValidationProblem();
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest("Problem Registering user");

            var origin = Request.Headers["origin"];
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
            var message =
                $"<p>Please click the below link to verify your account:</p>" +
                $"<p><a href='{verifyUrl}'>Click to verify Email</a></p>";

            await _emailSender.SendEmailAsync(user.Email, "Please verify Email", message);

            return Ok("Your account is created. Please verify your email");
        }

        /// <summary>
        /// Verify email address, this method is called when the user clicks on the link in the verification mail and verify the email in the DB
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized();

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded) return BadRequest("Could not verify Email");

            return Ok("Email is confirmed. You can now login");
        }

        /// <summary>
        /// Method to resend an email verification link in case the first link is lost
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("resendEmailConfirmationLink")]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized();

            var origin = Request.Headers["origin"];
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
            var message =
                $"<p>Please click the below link to verify your account:</p>" +
                $"<p><a href='{verifyUrl}'>Click to verify Email</a></p>";

            await _emailSender.SendEmailAsync(user.Email, "Please verify Email", message);

            return Ok("Email verification link is resent");
        }


        /// <summary>
        /// Method to get info about the current logged in user provided a valid token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

            await SetRefreshToken(user);
            return CreateUserObject(user);
        }

        /// <summary>
        /// Method to log in with Facebook, the method register a new user if the username is not found in the db
        /// </summary>
        /// <param name="accessToken">facebooks access token</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("fbLogin")]
        public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
        {
            var fbVerifyKeys = _config["Facebook:AppId"] + "|" + _config["Facebook:AppSecret"];

            var verifyToken =
                await _httpClient.GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");

            if (!verifyToken.IsSuccessStatusCode) return Unauthorized();

            var fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";

            var response = await _httpClient.GetAsync(fbUrl);

            if (!response.IsSuccessStatusCode) return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();

            var fbInfo = JsonConvert.DeserializeObject<dynamic>(content);

            var username = (string) fbInfo.id;

            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (user != null) return CreateUserObject(user);

            user = new AppUser
            {
                DisplayName = (string)fbInfo.name,
                Email = (string)fbInfo.email,
                UserName = (string)fbInfo.id,
                Photos = new List<Photo> {new Photo {Id = "fb_" + (string)fbInfo.id, Url = (string)fbInfo.picture.data.url, IsMain = true}},
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded) return BadRequest("Problem creating user account");

            await SetRefreshToken(user);
            return CreateUserObject(user);
        }

        /// <summary>
        /// Login method using Github OAuth. The method uses the temp code that Github provides to user.
        /// The method will try to find the user in the db, if not found a new user is created.A token is returned
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("githubLogin/{code}")]
        public async Task<ActionResult<UserDto>> GithubLogin(string code)
        {
            var accessToken = await GetGithubAccessToken(code);
            if (string.IsNullOrEmpty(accessToken)) return Unauthorized();


            var result = await "https://api.github.com/user"
                .WithHeader("Authorization", $"token {accessToken}")
                .WithHeader("User-Agent", "request")
                .GetJsonAsync<GithubUser>();

            if (result == null) return Unauthorized();

            var username = result.login;

            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (user != null) return CreateUserObject(user);

            var email = string.IsNullOrEmpty(result.email) ?  $"{result.login}@github.com" : result.email;
            user = new AppUser()
            {
                DisplayName = result.name,
                Email = email,
                EmailConfirmed = true,
                UserName = result.login,
                Photos = new List<Photo>() {new Photo() {Id = "gh_" + result.node_id, Url = result.avatar_url, IsMain = true}}
            };

            var userCreated = await _userManager.CreateAsync(user);

            if (!userCreated.Succeeded) return BadRequest("Problem creating user account");

            await SetRefreshToken(user);
            return CreateUserObject(user);
        }

        private async Task<string> GetGithubAccessToken(string code)
        {
            var clientId = _config["Github:ClientId"];
            var clientSecret = _config["Github:ClientSecret"];
            var url =
                $"https://github.com/login/oauth/access_token?code={code}&client_id={clientId}&client_secret={clientSecret}";

            var tokenResult = await url
                .WithHeader("Accept", "application/json")
                .PostAsync()
                .ReceiveJson<GithubAccessToken>();
            
            var accessToken = tokenResult.access_token;
            return accessToken;
        }

        /// <summary>
        /// Method to generate a new token to the user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _userManager.Users.Include(r => r.RefreshTokens)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

            if (user == null) return Unauthorized();

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive) return Unauthorized();

            if (oldToken != null) oldToken.Revoked = DateTime.UtcNow;

            return CreateUserObject(user);
        }

        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);

            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
