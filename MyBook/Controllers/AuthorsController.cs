using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using MyBook.API.Helpers;
using MyBook.API.Models;
using MyBook.API.ResourceParameters;
using MyBook.API.Services;
using MyBook.Entities;
using MyBook.Services;
using System.Text.Json;

namespace MyBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : Controller
    {
        private readonly IMyBookRepository _myBookRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        private readonly string _authorShapingRequiredFields = JsonNamingPolicy.CamelCase.ConvertName(nameof(AuthorDto.Id));

        private readonly string[] ValidHeadersWithoutLink = new[]
        {
            "*/*",
            "application/json",
            "application/vnd.mybook.author+json"
        };

        private readonly string[] ValidHeadersWithLink = new[]
        {
            "application/vnd.mybook.author.hateoas+json"
        };

        public AuthorsController(IMyBookRepository myBookRepository, IMapper mapper,
            IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyCheckerService, ProblemDetailsFactory problemDetailsFactory)
        {
            _myBookRepository = myBookRepository;
            _mapper = mapper;
            _propertyMappingService = propertyMappingService;
            _propertyCheckerService = propertyCheckerService;
            _problemDetailsFactory = problemDetailsFactory;
        }

        [HttpGet(Name = "GetAuthor")]
        [Produces("application/json", "application/vnd.mybook.author+json", "application/vnd.mybook.author.hateoas+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAuthor(Guid id, string? fields)
        {
            var acceptHeader = Request.Headers.Accept.ToString();

            if (MediaTypeHeaderValue.TryParse(acceptHeader, out var parsedAccept))
            {
                if (ValidHeadersWithoutLink.Contains(parsedAccept.MediaType.Value))
                {
                    return await GetAuthorInternal(id, fields, includeLinks: false);
                }
                else if (ValidHeadersWithLink.Contains(parsedAccept.MediaType.Value))
                {
                    return await GetAuthorInternal(id, fields, includeLinks: true);
                }
                else
                {
                    var problemDetails = new ProblemDetails
                    {
                        Detail = "The Accept header value is not supported. Supported values are: "
                         + string.Join(", ", ValidHeadersWithoutLink) + string.Join(", ", ValidHeadersWithLink),
                        Status = StatusCodes.Status406NotAcceptable
                    };

                    return BadRequest(problemDetails);
                }
            }
            else
            {
                return BadRequest("Invalid Accept header format");
            }
        }

        private async Task<ActionResult> GetAuthorInternal(Guid id, string? fields, bool includeLinks)
        {
            if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all requested data shaping fields exist on " +
                    $"the resource: {fields}"));
            }

            var author = await _myBookRepository.GetAuthorAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            if (includeLinks)
            {
                IEnumerable<LinkDto> links = CreateLinksForAuthor(id, fields);

                var linkedResourceToReturn = _mapper.Map<AuthorDto>(author).ShapeData(fields, _authorShapingRequiredFields) as IDictionary<string, object?>;
                linkedResourceToReturn.Add("links", links);

                return Ok(linkedResourceToReturn);
            }
            else
            {
                return Ok(_mapper.Map<AuthorDto>(author).ShapeData(fields, _authorShapingRequiredFields));
            }
        }

        [HttpGet("all", Name = "GetAuthors")]
        [Produces("application/json", "application/vnd.mybook.author+json", "application/vnd.mybook.author.hateoas+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            var acceptHeader = Request.Headers.Accept.ToString();

            if (MediaTypeHeaderValue.TryParse(acceptHeader, out var parsedAccept))
            {
                if (ValidHeadersWithoutLink.Contains(parsedAccept.MediaType.Value))
                {
                    return await GetAuthorsInternal(authorsResourceParameters, includeLinks: false);
                }
                else if (ValidHeadersWithLink.Contains(parsedAccept.MediaType.Value))
                {
                    return await GetAuthorsInternal(authorsResourceParameters, includeLinks: true);
                }
                else
                {
                    var problemDetails = new ProblemDetails
                    {
                        Detail = "The Accept header value is not supported. Supported values are: "
                         + string.Join(", ", ValidHeadersWithoutLink) + string.Join(", ", ValidHeadersWithLink),
                        Status = StatusCodes.Status406NotAcceptable
                    };

                    return BadRequest(problemDetails);
                }
            }
            else
            {
                return BadRequest("Invalid Accept header format");
            }
        }

        private async Task<ActionResult> GetAuthorsInternal(AuthorsResourceParameters authorsResourceParameters, bool includeLinks)
        {
            if (!_propertyMappingService
            .ValidMappingExistsFor<AuthorDto, Author>(
                authorsResourceParameters.OrderBy))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all requested ordering fields exist on " +
                    $"the resource: {authorsResourceParameters.OrderBy}"));
            }

            if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(authorsResourceParameters.Fields))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all requested data shaping fields exist on " +
                    $"the resource: {authorsResourceParameters.Fields}"));
            }

            var authors = await _myBookRepository.GetAuthorsAsync(authorsResourceParameters);

            var paginationMetadata = new
            {
                totalCount = authors.TotalCount,
                pageSize = authors.PageSize,
                currentPage = authors.CurrentPage,
                totalPages = authors.TotalPages
            };

            Response.Headers.Append("X-Pagination",
                   JsonSerializer.Serialize(paginationMetadata));

            if (includeLinks)
            {
                var links = CreateLinksForAuthors(authorsResourceParameters, authors.HasNext, authors.HasPrevious);

                var shapedAuthors = _mapper.Map<IEnumerable<AuthorDto>>(authors)
                    .ShapeData(authorsResourceParameters.Fields, _authorShapingRequiredFields);

                var authorsWithLinks = shapedAuthors.Select(author =>
                {
                    var authorAsDictionary = author as IDictionary<string, object?>;

                    var authorLinks = CreateLinksForAuthor((Guid)authorAsDictionary["Id"], null);

                    authorAsDictionary.Add("links", authorLinks);

                    return authorAsDictionary;
                });

                var linkedResourcesToReturn = new
                {
                    value = authorsWithLinks,
                    links = links
                };

                return Ok(linkedResourcesToReturn);
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors)
                .ShapeData(authorsResourceParameters.Fields, _authorShapingRequiredFields));
            }
        }

        [HttpPost(Name = "CreateAuthor")]
        [Produces("application/json", "application/vnd.mybook.author+json", "application/vnd.mybook.author.hateoas+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateAuthor(AuthorForCreationDto authorForCreationDto)
        {
            var acceptHeader = Request.Headers.Accept.ToString();

            if (MediaTypeHeaderValue.TryParse(acceptHeader, out var parsedAccept))
            {
                if (ValidHeadersWithoutLink.Contains(parsedAccept.MediaType.Value))
                {
                    return await CreateAuthorInternal(authorForCreationDto, false);
                }
                else if (ValidHeadersWithLink.Contains(parsedAccept.MediaType.Value))
                {
                    return await CreateAuthorInternal(authorForCreationDto, true);
                }
                else
                {
                    var problemDetails = new ProblemDetails
                    {
                        Detail = "The Accept header value is not supported. Supported values are: "
                         + string.Join(", ", ValidHeadersWithoutLink) + string.Join(", ", ValidHeadersWithLink),
                        Status = StatusCodes.Status406NotAcceptable
                    };

                    return BadRequest(problemDetails);
                }
            }
            else
            {
                return BadRequest("Invalid Accept header format");
            }
        }

        private async Task<ActionResult> CreateAuthorInternal(AuthorForCreationDto authorForCreationDto, bool includeLinks)
        {
            var authorEntity = _mapper.Map<Author>(authorForCreationDto);

            _myBookRepository.AddAuthor(authorEntity);
            await _myBookRepository.SaveAsync();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            if (includeLinks)
            {
                var links = CreateLinksForAuthor(authorToReturn.Id, null);

                var linkedResourceToReturn = authorToReturn.ShapeData(null, null) as IDictionary<string, object?>;
                linkedResourceToReturn.Add("links", links);

                return CreatedAtRoute("GetAuthor", new { id = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
            }
            else
            {
                return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAuthor(Guid id, AuthorForUpdateDto authorForUpdateDto)
        {
            if (!await _myBookRepository.AuthorExistsAsync(id))
            {
                return NotFound();
            }

            var authorFromRepo = await _myBookRepository.GetAuthorAsync(id);

            _mapper.Map(authorForUpdateDto, authorFromRepo);

            _myBookRepository.UpdateAuthor(authorFromRepo);

            await _myBookRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PartiallyUpdateAuthor(Guid id,
            JsonPatchDocument<AuthorForUpdateDto> patchDocument)
        {
            if (!await _myBookRepository.AuthorExistsAsync(id))
            {
                return NotFound();
            }

            var authorFromRepo = await _myBookRepository.GetAuthorAsync(id);

            var authorToPatch = _mapper.Map<AuthorForUpdateDto>(authorFromRepo);

            patchDocument.ApplyTo(authorToPatch, ModelState);

            if (!TryValidateModel(authorToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(authorToPatch, authorFromRepo);

            await _myBookRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAuthor(Guid id)
        {
            var author = await _myBookRepository.GetAuthorAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            _myBookRepository.DeleteAuthor(author);
            await _myBookRepository.SaveAsync();

            return NoContent();
        }

        private List<LinkDto> CreateLinksForAuthor(Guid id, string? fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new(Url.Link("GetAuthorWithLinks", new { id }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new(Url.Link("GetAuthorWithLinks", new { id, fields }),
                  "self",
                  "GET"));
            }

            links.Add(
                  new(Url.Link("GetBooks", new { AuthorId = id }),
                  "get_author_books",
                  "GET"));

            links.Add(
                  new(Url.Link("CreateBookForAuthor", new { AuthorId = id }),
                  "create_book_for_author",
                  "POST"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParameters authorsResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                // self 
                new(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.Current),
                    "self",
                    "GET")
            };

            if (hasNext)
            {
                links.Add(
                    new(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage),
                        "previousPage",
                        "GET"));
            }

            return links;
        }

        private string? CreateAuthorsResourceUri(
            AuthorsResourceParameters authorsResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthorsWithLinks",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            dateOfBirth = authorsResourceParameters.DateOfBirth,
                            fullName = authorsResourceParameters.FullName
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthorsWithLinks",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            dateOfBirth = authorsResourceParameters.DateOfBirth,
                            fullName = authorsResourceParameters.FullName
                        });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetAuthorsWithLinks",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            dateOfBirth = authorsResourceParameters.DateOfBirth,
                            fullName = authorsResourceParameters.FullName
                        });
            }
        }
    }
}
