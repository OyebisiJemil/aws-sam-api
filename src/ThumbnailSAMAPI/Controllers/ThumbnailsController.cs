using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ThumbnailSAMAPI.BusinessLayer.Services;
using ThumbnailSAMAPI.Models;

namespace ThumbnailSAMAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class ThumbnailsController : ControllerBase
    {
        private IS3BucketService s3BucketService;
        ILogger Logger { get; set; }
        public ThumbnailsController(IS3BucketService s3BucketService, ILogger<ThumbnailsController> logger)
        {
            this.s3BucketService = s3BucketService;
            Logger = logger;
        }

        // POST: api/Thumbnails
        [Route("api/thumbnails")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Image image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {               
                 string bucketName = "thumbnail-service-s3-bucket";
                 await this.s3BucketService.UploadImage(image.ImagePath, bucketName, Guid.NewGuid().ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                this.Logger.LogError("An error occured while saving an event " + ex.Message);
                return StatusCode(500, "An error occured while saving an event " + ex.Message);
            }
        }
    }
}
