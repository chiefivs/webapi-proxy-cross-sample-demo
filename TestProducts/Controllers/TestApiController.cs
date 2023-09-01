using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiInterfaces.TestProducts;

namespace TestProducts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestApiController : ControllerBase, ITestApi
    {
        private static List<Book> _books;

        static TestApiController()
        {
            _books = new List<Book>
            {
                new Book{Id = 1, Name = "WW2"},
                new Book{Id = 2, Name = "W&P"}
            };
        }

        public async Task AddBook(Book book)
        {
            var id = _books.Select(b => b.Id).Max() + 1;
            book.Id = id;
            _books.Add(book);
        }

        public async Task DelBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null)
                _books.Remove(book);
        }

        public async Task<IEnumerable<Book>> GetBooksList()
        {
            return _books;
        }
    }
}
