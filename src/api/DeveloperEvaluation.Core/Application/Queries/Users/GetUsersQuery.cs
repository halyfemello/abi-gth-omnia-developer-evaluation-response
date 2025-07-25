using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Queries.Users;

public class GetUsersQuery : IRequest<GetUsersResponse>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Status { get; set; }
    public string? Role { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? City { get; set; }

    public bool IsValid()
    {
        if (Page < 1 || Size < 1 || Size > 100)
            return false;

        return true;
    }

    public int GetOffset()
    {
        return (Page - 1) * Size;
    }
}

public class GetUsersResponse
{
    public PagedResultDto<UserDto> Data { get; set; } = new();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static GetUsersResponse CreateSuccess(PagedResultDto<UserDto> pagedResult)
    {
        return new GetUsersResponse
        {
            Data = pagedResult,
            Success = true,
            Message = "Usuários obtidos com sucesso"
        };
    }

    public static GetUsersResponse CreateFailure(string message)
    {
        return new GetUsersResponse
        {
            Data = new PagedResultDto<UserDto>(),
            Success = false,
            Message = message
        };
    }
}
