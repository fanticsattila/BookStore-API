using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Authors in the Book Store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all author
        /// </summary>
        /// <returns>List of authors</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attempted get all authors");

                IList<Author> authors = await _authorRepository.FindAll();
                IList<AuthorDTO> response = _mapper.Map<IList<AuthorDTO>>(authors);
                
                _logger.LogInfo("Successfully got all authors");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        /// <summary>
        /// Get author by id
        /// </summary>
        /// <param name="id">Author's id</param>
        /// <returns>Author object</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted get author with id = {id}");

                Author author = await _authorRepository.FindById(id);
                if(author==null)
                {
                    _logger.LogWarn($"Author with id = {id} is not found");
                    return NotFound();
                }

                AuthorDTO response = _mapper.Map<AuthorDTO>(author);

                _logger.LogInfo($"Successfully got author with id = {id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        /// <summary>
        /// Create an author
        /// </summary>
        /// <param name="authorDTO">Author object</param>
        /// <returns>Created object or error</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo("Author submission attempted");

                if (authorDTO == null)
                {
                    _logger.LogWarn("Empty request was submitted");
                    return BadRequest(ModelState);
                }

                if(!ModelState.IsValid)
                {
                    _logger.LogWarn("Author data was incomplete");
                    return BadRequest(ModelState);
                }

                Author author = _mapper.Map<Author>(authorDTO);
                bool isSuccess = await _authorRepository.Create(author);
                if(!isSuccess)
                    return InternalError(new Exception($"Author creation failed"));

                _logger.LogInfo("Author created successfully");
                return Created("Create", new { author });
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        /// <summary>
        /// Update an author
        /// </summary>
        /// <param name="id">Author object id</param>
        /// <param name="authorDTO">Author object</param>
        /// <returns>Updated object or error</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Author update attempted - id: {id}");

                if (id<1 || authorDTO == null || id!= authorDTO.Id)
                {
                    _logger.LogWarn("Empty request was submitted or Id is invalid");
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Author data was incomplete");
                    return BadRequest(ModelState);
                }

                Author author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                    return InternalError(new Exception($"Author update failed"));

                _logger.LogInfo("Author updated successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError(ex);
            }
        }

        /// <summary>
        /// Delete an author
        /// </summary>
        /// <param name="id">Author object id</param>
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
                _logger.LogInfo($"Author delete attempted - id: {id}");

                if (id < 1)
                {
                    _logger.LogWarn("Id is invalid");
                    return BadRequest();
                }

                bool isExists = await _authorRepository.isExists(id);
                if(!isExists)
                {
                    _logger.LogWarn("Author is not found");
                    return NotFound();
                }

                Author author = await _authorRepository.FindById(id);
                bool isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                    return InternalError(new Exception($"Author remove failed"));

                _logger.LogInfo("Author removed successfully");
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
