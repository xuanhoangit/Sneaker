using AutoMapper;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Models.OrderEntities;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Core.Models.UserEntities;


public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, ProductDTO>().ReverseMap();

        CreateMap<ProductColorDTO, ProductColor>()
        // .ForMember(dest => dest.Color, opt => opt.Ignore()) 
        // .ForMember(dest => dest.Color., opt => opt.Ignore())
        .ReverseMap();
        CreateMap<StaffInfo,StaffInfoDTO>().ReverseMap();
        CreateMap<CustomerInfo,CustomerInfoDTO>().ReverseMap();
        CreateMap<Address,AddressDTO>().ReverseMap();
        CreateMap<CartItem,AddCartDTO>().ReverseMap();
        CreateMap<Favorite,AddFavoriteDTO>().ReverseMap();
    }
}
