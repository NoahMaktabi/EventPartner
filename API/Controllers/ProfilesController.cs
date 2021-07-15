using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using Application.Interfaces;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController : BaseApiController
    {
        private readonly IUserAccessor _userAccessor;

        public ProfilesController(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            return HandleResult(await Mediator.Send(new Details.Query {Username = username}));
        }

        [HttpPut]
        public async Task<IActionResult> EditProfile(EditProfileDto profileDto)
        {
            return HandleResult(await Mediator.Send(new Edit
                .Command() { DisplayName = profileDto.DisplayName, Bio = profileDto.Bio}));
        }
    }
}
