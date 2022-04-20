using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Photos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PhotosController : BaseApiController
    {
        /// <summary>
        /// Adds a photo to cloudinary and returns a photo model
        /// </summary>
        /// <param name="command">A file containing the photo to add</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Add.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Deletes a photo from cloudinary and the db.
        /// </summary>
        /// <param name="id">The id of the photo</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command {Id = id}));
        }

        /// <summary>
        /// Finds the photo via ID and sets the photo as the user's main photo
        /// </summary>
        /// <param name="id">The id of the photo</param>
        /// <returns></returns>
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMain(string id)
        {
            return HandleResult(await Mediator.Send(new SetMain.Command {Id = id}));
        }
    }
}
