using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.DbServices;
using RestApi.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService bookService;

        public BooksController(IBookService bookService)
        {
            this.bookService = bookService;
        }

       
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllBooks()
        {
            var username = HttpContext.User.Identity.Name.ToString();
            var books = await bookService.GetAllBooks(username);
            return StatusCode(StatusCodes.Status200OK, books);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookModel>> GetBook(Guid id)
        {
            var foundBook = await bookService.GetBook(id);
            return StatusCode(StatusCodes.Status200OK, foundBook);

        }
        [Authorize]
        [HttpGet("SearchBook/{searchString}")]
        public async Task<IActionResult> SearchBook(string searchString)
        {
            
            var listOfFoundBook = await bookService.SearchBook(searchString);
            return StatusCode(StatusCodes.Status200OK, listOfFoundBook);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BookModel>> AddBook(BookModel book)
        {
            var username = HttpContext.User.Identity.Name.ToString();
            book.User = username;
            var dbBook = await bookService.AddBook(book);
            return StatusCode(StatusCodes.Status200OK, book);

        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<BookModel>> UpdateBook(BookModel updatedBook)
        {
            if (updatedBook.BookId == null)
            {
                return BadRequest("Id in the URL does not match the id in the request body.");
            }

            var dbBook = await bookService.UpdateBook(updatedBook);
            return StatusCode(StatusCodes.Status200OK, dbBook);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<BookModel>> DeleteBook(Guid id)
        {
            var isDelete = await bookService.DeleteBook(id);

            return StatusCode(StatusCodes.Status200OK, isDelete);

        }


    }
}
