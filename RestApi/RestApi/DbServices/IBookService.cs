using RestApi.Models;

namespace RestApi.DbServices
{
    public interface IBookService
    {
        Task<BookModel> AddBook(BookModel book);
        Task<List<BookModel>> GetAllBooks(string username);
      //  Task<List<BookModel>> GetAllBooks();
        Task<BookModel> GetBook(Guid id);
        Task<List<BookModel>> SearchBook(string searchString);
        Task<BookModel> UpdateBook(BookModel updatedBook);
        Task<bool> DeleteBook(Guid id);
    }
}
