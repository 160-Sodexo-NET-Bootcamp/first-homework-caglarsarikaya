using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Odev1.DTO;
using Odev1.Models;
using Odev1.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Odev1.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;


        public BookController(
            IBookService bookService,
            IMemoryCache memoryCache,
            IMapper mapper)
        {
            _bookService = bookService;
            _memoryCache = memoryCache;
            _mapper = mapper;
        }


        /// <summary>
        /// İstenilen sayıda kitap üretmek için kullanılır. Sayıyı gönderin ve rastgele kitaplar üretilsin.
        /// </summary>
        [HttpGet("Create")]
        public ActionResult CreateBook(int num)
        {
            //this method creates books of desired number
            //read from mem 
            _memoryCache.TryGetValue("TempDB", out List<Book> books);

            if (books == null) books = new List<Book>();

            //add new to mem saved list
            books.AddRange(_bookService.CreateRandom(num, books.Count));

            //save list with news
            _memoryCache.GetOrCreate("TempDB", cacheEntry =>
           {
               cacheEntry.AbsoluteExpiration = DateTime.Now.AddSeconds(150);
               cacheEntry.Priority = CacheItemPriority.High;
               return books;
           });

            return Ok();
        }

        /// <summary>
        /// Kayıtlı kitapların hepsini görüntülemek için kullanabilirsiniz
        /// </summary>
        [HttpGet("GetAll")]
        public List<BookViewModel> GetAll()
        {
            _memoryCache.TryGetValue("TempDB", out List<Book> books);
            return _mapper.Map<List<BookViewModel>>(books); ;
        }

        /// <summary>
        /// Kayıtlı kitaplarda id ile arama yapabilirsiniz. Id parametresi route ile alınacaktır. İlk eklenenin id si 0 dan başlayarak +1 şeklinde gidecektir.
        /// </summary>
        [HttpGet("FromRouteGetById/{Id}")]
        public Book FromRouteGetById([FromRoute] int Id)
        {
            _memoryCache.TryGetValue("TempDB", out List<Book> books);
            return books?.Where(x => x.Id == Id).SingleOrDefault();
        }


        /// <summary>
        /// Kayıtlı kitaplarda id ile arama yapabilirsiniz. Id parametresi query stringden alınacaktır. İlk eklenenin id si 0 dan başlayarak +1 şeklinde gidecektir.
        /// </summary>
        [HttpGet("FromQueryGetById")]
        public Book FromQueryGetById([FromQuery] int Id)
        {
            _memoryCache.TryGetValue("TempDB", out List<Book> books);
            return books?.Where(x => x.Id == Id).SingleOrDefault();
        }


        /// <summary>
        /// Yeni kitap eklemek için kullanabilirsiniz
        /// </summary>
        [HttpPost]
        public ActionResult AddNewBook([FromBody] BookViewModel book)
        {

            _memoryCache.TryGetValue("TempDB", out List<Book> books);

            if (books == null) books = new List<Book>();

            var mapped = _mapper.Map<Book>(book);

            //increment the Id
            mapped.Id = books.LastOrDefault()?.Id + 1 ?? 0;

            books.Add(mapped);

            //save list with news
            _memoryCache.GetOrCreate("TempDB", cacheEntry =>
            {
                cacheEntry.AbsoluteExpiration = DateTime.Now.AddSeconds(150);
                cacheEntry.Priority = CacheItemPriority.High;
                return books;
            });

            return Ok();
        }


        /// <summary>
        /// id 'sini gönderdiğiniz kitabı güncellersiniz
        /// </summary>
        [HttpPut("{Id}")]
        public ActionResult UpdateBook(int Id, [FromBody] BookViewModel book)
        {
            _memoryCache.TryGetValue("TempDB", out List<Book> books);
            var updateBook = books.Where(x => x.Id == Id).SingleOrDefault();
            if (updateBook == null) return Ok("Book not found");

            var mapped = _mapper.Map<Book>(book);
            mapped.Id = updateBook.Id;

            books.Remove(updateBook);
            books.Add(mapped);

            //save list with news
            _memoryCache.GetOrCreate("TempDB", cacheEntry =>
            {
                cacheEntry.AbsoluteExpiration = DateTime.Now.AddSeconds(150);
                cacheEntry.Priority = CacheItemPriority.High;
                return books;
            });

            return Ok();
        }

        /// <summary>
        /// id' sini gönderdiğini kitabı silersiniz.
        /// </summary>
        [HttpDelete("{Id}")]
        public ActionResult DeleteBook(int Id)
        {
            _memoryCache.TryGetValue("TempDB", out List<Book> books);
            var deleteBook = books.Where(x => x.Id == Id).SingleOrDefault();
            if (deleteBook == null) return Ok("Book not found");
            books.Remove(deleteBook);

            //save list with news
            _memoryCache.GetOrCreate("TempDB", cacheEntry =>
            {
                cacheEntry.AbsoluteExpiration = DateTime.Now.AddSeconds(150);
                cacheEntry.Priority = CacheItemPriority.High;
                return books;
            });

            return Ok();

        }
    }
}
