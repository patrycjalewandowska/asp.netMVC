using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RestApi.Models;

namespace RestApi.DbServices
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }
            public DbSet<BookModel> Books { get; set; }
            public DbSet<UserRegisterModel> Users { get; set; }






    }
}