using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Followers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FollowController : BaseApiController
    {
        /// <summary>
        /// Register the logged in user as a follower to the user provided in params
        /// </summary>
        /// <param name="username">a username that the current user will follow</param>
        /// <returns></returns>
        [HttpPost("{username}")]
        public async Task<IActionResult> Follow(string username)
        {
            return HandleResult(await Mediator.Send(new FollowToggle.Command {TargetUsername = username}));
        }

        /// <summary>
        /// Provided a username and a predicate, the method will return the followers or users following the username
        /// </summary>
        /// <param name="username">The username that the method should use</param>
        /// <param name="predicate">followers or following indicating what the request should do</param>
        /// <returns></returns>
        [HttpGet("{username}")]
        public async Task<IActionResult> GetFollowings(string username, string predicate)
        {
            return HandleResult(await Mediator.Send(new List.Query {Username = username, Predicate = predicate}));
        }
    }
}
