using System.ComponentModel.DataAnnotations;

namespace DeveloperEvaluation.Core.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserNameDto Name { get; set; } = new();
    public UserAddressDto? Address { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CreateUserDto
{

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username é obrigatório")]
    [MinLength(3, ErrorMessage = "Username deve ter pelo menos 3 caracteres")]
    [MaxLength(50, ErrorMessage = "Username não pode ter mais de 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password é obrigatório")]
    [MinLength(6, ErrorMessage = "Password deve ter pelo menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    public UserNameDto Name { get; set; } = new();
    public UserAddressDto? Address { get; set; }

    [Phone(ErrorMessage = "Telefone deve ter formato válido")]
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string Role { get; set; } = "Customer";
}

public class UpdateUserDto
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username é obrigatório")]
    [MinLength(3, ErrorMessage = "Username deve ter pelo menos 3 caracteres")]
    [MaxLength(50, ErrorMessage = "Username não pode ter mais de 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password é obrigatório")]
    [MinLength(6, ErrorMessage = "Password deve ter pelo menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome é obrigatório")]
    public UserNameDto Name { get; set; } = new();
    public UserAddressDto? Address { get; set; }

    [Phone(ErrorMessage = "Telefone deve ter formato válido")]
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string Role { get; set; } = "Customer";
}

public class UserNameDto
{
    [Required(ErrorMessage = "Primeiro nome é obrigatório")]
    [MaxLength(50, ErrorMessage = "Primeiro nome não pode ter mais de 50 caracteres")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sobrenome é obrigatório")]
    [MaxLength(50, ErrorMessage = "Sobrenome não pode ter mais de 50 caracteres")]
    public string LastName { get; set; } = string.Empty;
}

public class UserAddressDto
{
    [Required(ErrorMessage = "Cidade é obrigatória")]
    [MaxLength(100, ErrorMessage = "Cidade não pode ter mais de 100 caracteres")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rua é obrigatória")]
    [MaxLength(200, ErrorMessage = "Rua não pode ter mais de 200 caracteres")]
    public string Street { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Número deve ser maior que 0")]
    public int Number { get; set; }

    [Required(ErrorMessage = "CEP é obrigatório")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve ter formato válido (ex: 12345-678)")]
    public string ZipCode { get; set; } = string.Empty;
    public GeoLocationDto? GeoLocation { get; set; }
}

public class GeoLocationDto
{
    [Required(ErrorMessage = "Latitude é obrigatória")]
    public string Latitude { get; set; } = string.Empty;

    [Required(ErrorMessage = "Longitude é obrigatória")]
    public string Longitude { get; set; } = string.Empty;
}

public class UsersQueryParametersDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Página deve ser maior que 0")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Tamanho da página deve estar entre 1 e 100")]
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
        return Page >= 1 && Size >= 1 && Size <= 100;
    }
    public int GetOffset()
    {
        return (Page - 1) * Size;
    }
}
