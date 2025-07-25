using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Queries.Users;

public class GetAllUsersQuery : IRequest<GetAllUsersResponse>
{
}

public class GetAllUsersResponse
{
    public IList<UserDto> Users { get; set; } = new List<UserDto>();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static GetAllUsersResponse CreateSuccess(IList<UserDto> users)
    {
        return new GetAllUsersResponse
        {
            Users = users,
            Success = true,
            Message = "Usuários obtidos com sucesso"
        };
    }

    public static GetAllUsersResponse CreateFailure(string message)
    {
        return new GetAllUsersResponse
        {
            Users = new List<UserDto>(),
            Success = false,
            Message = message
        };
    }
}
