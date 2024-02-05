using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MyBook.API.Helpers;
using MyBook.API.Models;
using MyBook.API.ResourceParameters;
using MyBook.API.Services;
using MyBook.Entities;
using MyBook.Models;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAuthor(Guid id, string? fields)
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

            return Ok(_mapper.Map<AuthorDto>(author).ShapeData(fields));
        }

        [HttpGet]
        [Route("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
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

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors)
                .ShapeData(authorsResourceParameters.Fields));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateAuthor(AuthorForCreationDto authorForCreationDto)
        {
            var authorEntity = _mapper.Map<Author>(authorForCreationDto);

            _myBookRepository.AddAuthor(authorEntity);
            await _myBookRepository.SaveAsync();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { authorToReturn.Id }, authorToReturn);
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
    }
}
