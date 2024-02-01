using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyBook.API.Services;
using MyBook.Controllers;
using MyBook.Entities;
using MyBook.Models;
using MyBook.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBook.Test
{
    public class BooksControllerTests
    {
        private readonly Mock<IMyBookRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPropertyMappingService> _mockPropertyMappingService;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockRepository = new Mock<IMyBookRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockPropertyMappingService = new Mock<IPropertyMappingService>();
            _controller = new BooksController(_mockRepository.Object, _mockMapper.Object, _mockPropertyMappingService.Object);
        }

        [Fact]
        public async Task GetBook_BookExists_MustReturnsOkResult()
        {
            // Arrange
            var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            var book = new Book("Book1");
            book.Id = bookId;

            _mockRepository.Setup(repo => repo.GetBookAsync(bookId)).ReturnsAsync(book);
            _mockMapper.Setup(mapper => mapper.Map<BookDto>(book)).Returns(new BookDto { Id= bookId });

            // Act
            var result = await _controller.GetBook(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BookDto>(okResult.Value);
            Assert.Equal(bookId, model.Id);
        }

        [Fact]
        public async Task GetBook_BookNotExist_MustReturnsNotFound()
        {
            // Arrange
            var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            Book? book = null;

            _mockRepository.Setup(repo => repo.GetBookAsync(bookId)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetBook(bookId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }
    }
}
