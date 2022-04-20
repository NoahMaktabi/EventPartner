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

        /// <summary>
        /// Provided a username, the method will find a user profile and return it.
        /// </summary>
        /// <param name="username">A valid username</param>
        /// <returns></returns>
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            return HandleResult(await Mediator.Send(new Details.Query {Username = username}));
        }

        /// <summary>
        /// Edits the profile of the user
        /// </summary>
        /// <param name="profileDto">A model containing the edited details of the user</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> EditProfile(EditProfileDto profileDto)
        {
            return HandleResult(await Mediator.Send(new Edit
                .Command() { DisplayName = profileDto.DisplayName, Bio = profileDto.Bio}));
        }

        /// <summary>
        /// Gets a list of activities of a specific user
        /// </summary>
        /// <param name="appUsername"></param>
        /// <param name="predicate">past, hosting, future which indicate what kind of activities to return</param>
        /// <returns></returns>
        [HttpGet("{appUsername}/activities")]
        public async Task<IActionResult> GetListActivities(string appUsername, [FromQuery] string predicate)
        {
            return HandleResult(await Mediator.Send(new ListActivities.Query
                {Username = appUsername, Predicate = predicate}));
        }
    }
}
