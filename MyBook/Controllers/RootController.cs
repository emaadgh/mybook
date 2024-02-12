using Microsoft.AspNetCore.Mvc;
using MyBook.API.Models;

namespace MyBook.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
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
