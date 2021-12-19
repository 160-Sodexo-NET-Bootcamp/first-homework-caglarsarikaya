using Odev1.Models;
using System.Collections.Generic;

namespace Odev1.Services
{
    public interface IBookService
    {
        List<Book> CreateRandom(int num, int count);
    }
}