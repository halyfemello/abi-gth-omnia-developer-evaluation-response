using AutoMapper;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Commands.Sales;

namespace DeveloperEvaluation.Core.Application.Mappings;

public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        CreateMap<CreateSaleDto, CreateSaleCommand>();
        CreateMap<UpdateSaleDto, UpdateSaleCommand>();

        CreateMap<Sale, SaleDto>();

        CreateMap<SaleItem, SaleItemDto>();

        CreateMap<CreateSaleDto, Sale>();

        CreateMap<CreateSaleItemDto, SaleItem>();
    }
}
