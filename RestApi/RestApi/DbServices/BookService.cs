using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.DbServices
{
    public class BookService : IBookService
    {
        private readonly AppDbContext db;

        public BookService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<BookModel> AddBook(BookModel book)
        {
            await db.Books.AddAsync(book);
            await db.SaveChangesAsync();
            return book;
        }

        public async Task<BookModel> GetBook(Guid id)
        {
            var foundBook = await db.Books.FindAsync(id);
            return foundBook;
        }

        public async Task<List<BookModel>> GetAllBooks(string username)
        {
            return await db.Books
                .Where(book => book.User == username)
                .ToListAsync();
        }
/*
        public async Task<List<BookModel>> GetAllBooks()
        {

          return await db.Books.ToListAsync();
        }
*/
        public async Task<List<BookModel>> SearchBook(string searchString)
        {
            Console.WriteLine("searchString" + searchString);
            List<BookModel> foundBooks = await db.Books.Where(b => b.Title.Contains(searchString)).ToListAsync();
            return foundBooks;

        }

        public async Task<BookModel> UpdateBook(BookModel updatedBook)
        {

            var existingBook = await db.Books.FindAsync(updatedBook.BookId);

            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.Description = updatedBook.Description;

            await db.SaveChangesAsync();

            return existingBook;
        }

        public async Task<bool> DeleteBook(Guid id)
        {
            var book = await db.Books.FindAsync(id);

             db.Books.Remove(book);

            await db.SaveChangesAsync();

            return true;
        }
    }
}
