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

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>().ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id property in CategoryDto>
            CreateMap<Transaction, TransactionDto>(); // Map Transaction to TransactionDto>
            CreateMap<TransactionDto, Transaction>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore());


        }
    }
}
