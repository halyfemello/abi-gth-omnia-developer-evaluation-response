using AutoMapper;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Commands.Products;
using DeveloperEvaluation.Core.Domain.Entities;

namespace DeveloperEvaluation.Core.Application.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductDto, CreateProductCommand>();
        CreateMap<UpdateProductDto, UpdateProductCommand>();

        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new ProductRatingDto
            {
                Rate = src.Rating.Rate,
                Count = src.Rating.Count
            }));

        CreateMap<ProductDto, Product>()
            .ConstructUsing(dto => new Product(
                dto.Title,
                dto.Price,
                dto.Description,
                dto.Category,
                dto.Image,
                new ProductRating(dto.Rating.Rate, dto.Rating.Count),
                dto.Stock));

        CreateMap<CreateProductDto, Product>()
            .ConstructUsing(dto => new Product(
                dto.Title,
                dto.Price,
                dto.Description,
                dto.Category,
                dto.Image,
                dto.Rating != null ? new ProductRating(dto.Rating.Rate, dto.Rating.Count) : new ProductRating(0, 0),
                dto.Stock));

        CreateMap<ProductRating, ProductRatingDto>();

        CreateMap<ProductRatingDto, ProductRating>()
            .ConstructUsing(dto => new ProductRating(dto.Rate, dto.Count));
    }
}
