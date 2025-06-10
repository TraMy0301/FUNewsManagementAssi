using AutoMapper;
using BusinessObjects.Entities;
using Services.DTOs;


namespace Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest => dest.ParentCategory, opt => opt.MapFrom(src => src.ParentCategory));
        }
    }

}
