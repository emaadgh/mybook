using Microsoft.AspNetCore.Mvc;
using MyBook.API.Models;

namespace MyBook.API.Controllers
{
    /// <summary>
    /// Controller for accessing root API endpoints.
    /// </summary>
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        /// <summary>
        /// Get the root of the API.
        /// </summary>
        /// <returns>An IActionResult containing links to different API endpoints.</returns> 
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>
            {
                new(Url.Link("GetRoot", new { }),
                    "self",
                    "GET"),
                new(Url.Link("GetAuthors", new { }),
                    "authors",
                    "GET"),
                new(Url.Link("GetBooks", new { }),
                    "books",
                    "GET"),
                new(Url.Link("CreateAuthor", new { }),
                    "create_author",
                    "POST")
            };

            return Ok(links);
        }
    }
}
