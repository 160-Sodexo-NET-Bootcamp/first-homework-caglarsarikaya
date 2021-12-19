using Bogus;
using Odev1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Odev1.Services
{
    public class BookService : IBookService
    {

        public List<Book> CreateRandom(int num,int count)
        {
            var books = new List<Book>();

            for (var i = 0; i < num; ++i)
            {
                var data = new Faker<Book>()
               .RuleFor(x => x.Name, x => x.Name.JobTitle())
               .RuleFor(x => x.Writer, x => x.Person.FullName)
               .RuleFor(x => x.Genre, x => x.Name.JobType())
               .RuleFor(x => x.Id, i+ count)
               .RuleFor(x => x.ISBN, RandomISBN);

                books.Add(data);
            }



            return books;
        }

        public string RandomISBN()
        {
            Random R = new Random();

            return ((long)R.Next(0, 100000) * (long)R.Next(0, 100000)).ToString().PadLeft(13, '0');
        }

    }
}
