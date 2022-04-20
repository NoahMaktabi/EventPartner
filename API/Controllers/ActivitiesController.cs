using System;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        /// <summary>
        /// Get a list of activities. Params can be provided for pagination
        /// </summary>
        /// <param name="param">provides page number and page size for pagination</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetActivities([FromQuery]ActivityParams param )
        {
            return HandlePagedResult(await Mediator.Send(new List.Query {Params = param}));
        }

        /// <summary>
        /// Get details for a specific activity
        /// </summary>
        /// <param name="id">The id of the activity to return</param>
        /// <returns></returns>
        [HttpGet("{id}")] //activities/id
        public async Task<IActionResult> GetActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query {Id = id}));
        }

        /// <summary>
        /// Create a new activity and save to db 
        /// </summary>
        /// <param name="activity">An activity model</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            return HandleResult(await Mediator.Send(new Create.Command {Activity = activity}));
        }

        /// <summary>
        /// Edit an activity provided that the user is the host to the activity
        /// </summary>
        /// <param name="id">Id to the specific activity</param>
        /// <param name="activity">An activity model</param>
        /// <returns></returns>
        [Authorize(Policy = "IsActivityHost")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command {Activity = activity}));
        }

        /// <summary>
        /// Delete an activity provided that the user is the host to the activity
        /// </summary>
        /// <param name="id">Id to the specific activity</param>
        /// <returns></returns>
        [Authorize(Policy = "IsActivityHost")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id}));
        }

        /// <summary>
        /// Attend a specific activity, the user is listed as an attender.
        /// </summary>
        /// <param name="id">Id to the specific activity</param>
        /// <returns></returns>
        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(Guid id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command {Id = id}));
        }


    }
}
