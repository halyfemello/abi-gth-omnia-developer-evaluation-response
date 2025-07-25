using DeveloperEvaluation.Core.Domain.Common;

namespace DeveloperEvaluation.Core.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserName Name { get; set; } = null!;

    public UserAddress? Address { get; set; }

    public string Phone { get; set; } = string.Empty;

    public UserStatus Status { get; set; } = UserStatus.Active;

    public UserRole Role { get; set; } = UserRole.Customer;

    protected User() { }

    public User(
        string email,
        string username,
        string passwordHash,
        UserName name,
        string phone = "",
        UserAddress? address = null,
        UserRole role = UserRole.Customer)
    {
        Email = email;
        Username = username;
        PasswordHash = passwordHash;
        Name = name;
        Phone = phone;
        Address = address;
        Role = role;
        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Email) &&
               IsValidEmail(Email) &&
               !string.IsNullOrWhiteSpace(Username) &&
               Username.Length >= 3 &&
               !string.IsNullOrWhiteSpace(PasswordHash) &&
               Name.IsValid() &&
               (string.IsNullOrWhiteSpace(Phone) || IsValidPhone(Phone));
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = UserStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            throw new ArgumentException("Email deve ser válido");

        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            throw new ArgumentException("Username deve ter pelo menos 3 caracteres");

        Username = username;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Hash da senha é obrigatório");

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(UserName name)
    {
        if (name == null || !name.IsValid())
            throw new ArgumentException("Nome deve ser válido");

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePhone(string phone)
    {
        if (!string.IsNullOrWhiteSpace(phone) && !IsValidPhone(phone))
            throw new ArgumentException("Telefone deve ter formato válido");

        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAddress(UserAddress? address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive()
    {
        return Status == UserStatus.Active;
    }

    public bool CanLogin()
    {
        return Status == UserStatus.Active;
    }

    public bool HasRole(UserRole role)
    {
        return Role == role;
    }

    public bool IsAdmin()
    {
        return Role == UserRole.Admin;
    }

    public bool IsManager()
    {
        return Role == UserRole.Manager;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPhone(string phone)
    {
        var cleanPhone = phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "");
        return cleanPhone.Length >= 10 && cleanPhone.All(char.IsDigit);
    }
}

public class UserName
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    protected UserName() { }

    public UserName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName);
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}".Trim();
    }
}

public class UserAddress
{
    public string City { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public int Number { get; set; }

    public string ZipCode { get; set; } = string.Empty;

    public GeoLocation? GeoLocation { get; set; }

    protected UserAddress() { }

    public UserAddress(string city, string street, int number, string zipCode, GeoLocation? geoLocation = null)
    {
        City = city;
        Street = street;
        Number = number;
        ZipCode = zipCode;
        GeoLocation = geoLocation;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(City) &&
               !string.IsNullOrWhiteSpace(Street) &&
               Number > 0 &&
               !string.IsNullOrWhiteSpace(ZipCode);
    }
}

public class GeoLocation
{
    public string Latitude { get; set; } = string.Empty;

    public string Longitude { get; set; } = string.Empty;

    protected GeoLocation() { }

    public GeoLocation(string latitude, string longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Latitude) && !string.IsNullOrWhiteSpace(Longitude);
    }
}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended
}

public enum UserRole
{
    Customer,
    Manager,
    Admin
}
