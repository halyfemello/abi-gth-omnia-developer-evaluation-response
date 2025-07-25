using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Queries.Carts;

public class GetCartsQuery : IRequest<GetCartsResponse>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public int? UserId { get; set; }
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }

    public bool IsValid()
    {
        if (Page < 1 || Size < 1 || Size > 100)
            return false;

        if (MinDate.HasValue && MaxDate.HasValue && MinDate > MaxDate)
            return false;

        return true;
    }
}

public class GetCartsResponse
{
    public CartsPagedResponseDto Data { get; set; } = new();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static GetCartsResponse CreateSuccess(CartsPagedResponseDto pagedResult)
    {
        return new GetCartsResponse
        {
            Data = pagedResult,
            Success = true,
            Message = "Carrinhos obtidos com sucesso"
        };
    }

    public static GetCartsResponse CreateFailure(string message)
    {
        return new GetCartsResponse
        {
            Data = new CartsPagedResponseDto(),
            Success = false,
            Message = message
        };
    }
}
