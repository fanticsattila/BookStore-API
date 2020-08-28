using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Books in the Book Store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, ILoggerService logger, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all book
        /// </summary>
        /// <returns>List of books</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                _logger.LogInfo("Attempted get all books");

                IList<Book> books = await _bookRepository.FindAll();
                IList<BookDTO> response = _mapper.Map<IList<BookDTO>>(books);

                _logger.LogInfo("Successfully got all books");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        /// <summary>
        /// Get a book by id
        /// </summary>
        /// <param name="id">Book's id</param>
        /// <returns>Book object</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookById(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted get book with id = {id}");

                Book book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"Book with id = {id} is not found");
                    return NotFound();
                }

                BookDTO response = _mapper.Map<BookDTO>(book);

                _logger.LogInfo($"Successfully got book with id = {id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        /// <summary>
        /// Create a book
        /// </summary>
        /// <param name="bookDTO">Book object</param>
        /// <returns>Created object or error</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            try
            {
                _logger.LogInfo("Book submission attempted");

                if (bookDTO == null)
                {
                    _logger.LogWarn("Empty request was submitted");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Book data was incomplete");
                    return BadRequest(ModelState);
                }

                Book book = _mapper.Map<Book>(bookDTO);
                bool isSuccess = await _bookRepository.Create(book);
                if (!isSuccess)
                    return InternalError(new Exception($"Book creation failed"));

                _logger.LogInfo("Book created successfully");
                return Created("Create", new { book });
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        /// <summary>
        /// Update a book
        /// </summary>
        /// <param name="id">Book object id</param>
        /// <param name="bookDTO">Book object</param>
        /// <returns>Updated object or error</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            try
            {
                _logger.LogInfo($"Book update attempted - id: {id}");

                if (id < 1 || bookDTO == null || id != bookDTO.Id)
                {
                    _logger.LogWarn("Empty request was submitted or Id is invalid");
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Book data was incomplete");
                    return BadRequest(ModelState);
                }

                Book book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Update(book);
                if (!isSuccess)
                    return InternalError(new Exception($"Book update failed"));

                _logger.LogInfo("Book updated successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id">Book object id</param>
        /// <returns>Status</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo($"Book delete attempted - id: {id}");

                if (id < 1)
                {
                    _logger.LogWarn("Id is invalid");
                    return BadRequest();
                }

                bool isExists = await _bookRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn("Book is not found");
                    return NotFound();
                }

                Book book = await _bookRepository.FindById(id);
                bool isSuccess = await _bookRepository.Delete(book);
                if (!isSuccess)
                    return InternalError(new Exception($"Book remove failed"));

                _logger.LogInfo("Book removed successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        private ObjectResult InternalError(Exception ex)
        {
            string msg = ex.Message + (ex.InnerException != null ? $" - {ex.InnerException.Message}" : "");
            _logger.LogError(msg);
            return StatusCode(500, "Something went wrong. Please contact your Administrator!");
        }
    }
}
