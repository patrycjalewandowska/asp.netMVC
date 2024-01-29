using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectClient.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using static System.Reflection.Metadata.BlobBuilder;
using System.Collections.Generic;

namespace ProjectClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("");
        }


        public async Task<IActionResult> SortBooks(string sortOrder)
        {
            List<BookModel> books = await GetBooksFromApi();

            Console.WriteLine(sortOrder);
         
            if (sortOrder == null)
            {
                sortOrder = "Title";
            }
            Console.WriteLine(sortOrder);

            switch (sortOrder)
            {

                case "Title":
                    books = books.OrderBy(b => b.Title).ToList();
                    break;
                case "Author":
                    books = books.OrderBy(b => b.Author).ToList();
                    break;
            }

            return View("RestApi", books);
        }


        public async Task<IActionResult> SearchBook(string searchString)
        {
            Console.WriteLine(searchString);
            List<BookModel> foundBooks = new List<BookModel>();
            var client = new HttpClient();

            await Task.Run(async () =>
            {
               
                var response = await client.GetAsync($"https://localhost:7046/api/Books/SearchBook/{searchString}");

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<List<BookModel>>(dataObjects);

                    foreach (var item in result)
                    {
                        foundBooks.Add(item);
                    }
                }
                else {

                    TempData["AlertStatus"] = "Wrong";
                    TempData["AlertMessage"] = "Book not found";
                    Console.WriteLine("{0} {1}", (int)response.StatusCode, response.ReasonPhrase);

                }
            });
         
            return View("RestApi", foundBooks);
        }


        public async Task<IActionResult> RestApi()
        {
            List<BookModel> books = await GetBooksFromApi();
            return View(books);
        }

        private async Task<List<BookModel>> GetBooksFromApi()
        {
            List<BookModel> books = new List<BookModel>();

            var client = new HttpClient();
            var token = HttpContext.Session.GetString("JWTToken");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await Task.Run(() =>
            {
                var response = client.GetAsync("https://localhost:7046/api/Books").Result;

              
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine();
                    var result = JsonConvert.DeserializeObject<List<BookModel>>(dataObjects);

                    foreach (var item in result)
                    {
                        books.Add(item);
                        
                    }

                }
            });

            return books;
        }

        

        public async Task<IActionResult> Edit(Guid id)
        {
            var client = new HttpClient();
            BookModel book = null;

            var token = HttpContext.Session.GetString("JWTToken");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await Task.Run(() =>
            {
                
                var response = client.GetAsync($"https://localhost:7046/api/Books/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataJson = response.Content.ReadAsStringAsync().Result;
                    book = JsonConvert.DeserializeObject<BookModel>(dataJson);
                }
            });

            return View(book);
        }

        public async Task<IActionResult> UpdateBook(BookModel book)
        {
            var client = new HttpClient();

            var token = HttpContext.Session.GetString("JWTToken");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (book.BookId == null)
            {
                return BadRequest("BookId cannot be empty.");
            }

            string json = JsonConvert.SerializeObject(book);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7046/api/Books/{book.BookId}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Book updated successfully.");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }

            return RedirectToAction("RestApi");
        }


        public async Task<IActionResult> CreateBook(BookModel book)
        {
            book.BookId = Guid.NewGuid().ToString();
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("JWTToken");

          
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);



            string json = JsonConvert.SerializeObject(book);

            StringContent httpContent = new StringContent(json,System.Text.Encoding.UTF8,"application/json");

            var response = await client.PostAsync("https://localhost:7046/api/Books",httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("data posted");
                TempData["AlertStatus"] = "Success";
                TempData["AlertMessage"] = "The book has been successfully added";
            }
            else 
            {
                TempData["AlertStatus"] = "Wrong";
                TempData["AlertMessage"] = "Something go wrong";
                Console.WriteLine("{0} {1}",(int)response.StatusCode, response.ReasonPhrase);
            }

            return RedirectToAction("RestApi");
        }

        public async Task<IActionResult> CreateUser(UserRegisterModel user)
        {
           user.UserId = Guid.NewGuid();
              
            var client = new HttpClient();
    

            string json = JsonConvert.SerializeObject(user);

            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7046/api/Users/Register", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("data posted");
            }
            else
            {
                Console.WriteLine("{0} {1}", (int)response.StatusCode, response.ReasonPhrase);
            }

            return RedirectToAction("");
        }

        public async Task<IActionResult> LoginUser(UserLoginModel user)
        {
            var client = new HttpClient();

            string json = JsonConvert.SerializeObject(user);

            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7046/api/Users/Login", httpContent);

            Console.WriteLine(response);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("JWTToken", token);
                Console.WriteLine("User logged in");

            }
            else
            {
                Console.WriteLine("{0} {1}", (int)response.StatusCode, response.ReasonPhrase);
            }

           
            return RedirectToAction("RestApi");
        }

        public async Task<IActionResult> DeleteShow(Guid id) {

            var client = new HttpClient();
            BookModel book = null;
            var token = HttpContext.Session.GetString("JWTToken");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);



            await Task.Run(() =>
            {

                var response = client.GetAsync($"https://localhost:7046/api/Books/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataJson = response.Content.ReadAsStringAsync().Result;
                    book = JsonConvert.DeserializeObject<BookModel>(dataJson);
                }
            });
           
            return View(book);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("JWTToken");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await Task.Run(() =>
            {
                var response = client.DeleteAsync($"https://localhost:7046/api/Books/{id}").Result;
               
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("data posted");
                    TempData["AlertStatus"] = "Success";
                    TempData["AlertMessage"] = "The book has been successfully added";
                }
            });
            return RedirectToAction("RestApi");
        }


        public async Task<IActionResult> Details(Guid id)
        {

            var client = new HttpClient();
            BookModel book = null;
            var token = HttpContext.Session.GetString("JWTToken");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);



            await Task.Run(() =>
            {

                var response = client.GetAsync($"https://localhost:7046/api/Books/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataJson = response.Content.ReadAsStringAsync().Result;
                    book = JsonConvert.DeserializeObject<BookModel>(dataJson);
                }
            });

            return View(book);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
