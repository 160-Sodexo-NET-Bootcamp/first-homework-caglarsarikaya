using AutoMapper;
using Odev1.DTO;
using Odev1.Models;

namespace Odev1
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookViewModel>();
            CreateMap<BookViewModel, Book>();
        }
    }
}
