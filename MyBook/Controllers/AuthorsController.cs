using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

        public AuthorsController(IMyBookRepository myBookRepository, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _myBookRepository = myBookRepository;
            _mapper = mapper;
            _propertyMappingService = propertyMappingService;
        }

        [HttpGet(Name = "GetAuthor")]
        public async Task<ActionResult> GetAuthor(Guid id)
        {
            var author = await _myBookRepository.GetAuthorAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult> GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            if (!_propertyMappingService
            .ValidMappingExistsFor<AuthorDto, Author>(
                authorsResourceParameters.OrderBy))
            {
                return BadRequest();
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

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors));
        }

        [HttpPost]
        public async Task<ActionResult> CreateAuthor(AuthorForCreationDto authorForCreationDto)
        {
            var authorEntity = _mapper.Map<Author>(authorForCreationDto);

            _myBookRepository.AddAuthor(authorEntity);
            await _myBookRepository.SaveAsync();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { authorToReturn.Id }, authorToReturn);
        }

        [HttpPut]
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
