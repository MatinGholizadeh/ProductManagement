using AutoMapper;
using ProdManagement.Application.Features.Products.Commands.CreateProduct;
using ProdManagement.Application.Features.Products.DTOs;
using ProdManagement.Domain.Entities.Products;

namespace ProdManagement.Application.Common.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ProductDate, opt => opt.MapFrom(src => src.ProductDate))
            .ForMember(dest => dest.ManufacturePhone, opt => opt.MapFrom(src => src.ManufacturePhone))
            .ForMember(dest => dest.ManufactureEmail, opt => opt.MapFrom(src => src.ManufactureEmail))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
        CreateMap<Product, CreateProductResponse>();
    }
}
