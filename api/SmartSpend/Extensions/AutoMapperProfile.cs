using AutoMapper;
using SmartSpend.Dtos;
using SmartSpend.Models;

namespace SmartSpend.Extensions
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserBaseDto>().ReverseMap();

            CreateMap<Category, CategoryDto>().ReverseMap();

        }
    }
}
