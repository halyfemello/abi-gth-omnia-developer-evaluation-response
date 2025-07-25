using DeveloperEvaluation.Core.Domain.Entities;
using System.Linq.Expressions;

namespace DeveloperEvaluation.Core.Application.Services;

public class UserFilterBuilder
{
    public Expression<Func<User, bool>> BuildFilter(
        string? email = null,
        string? username = null,
        string? status = null,
        string? role = null,
        string? firstName = null,
        string? lastName = null,
        string? city = null)
    {
        var filters = new List<Expression<Func<User, bool>>>();

        if (!string.IsNullOrWhiteSpace(email))
        {
            filters.Add(BuildEmailFilter(email));
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            filters.Add(BuildUsernameFilter(username));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            filters.Add(BuildStatusFilter(status));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            filters.Add(BuildRoleFilter(role));
        }

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            filters.Add(BuildFirstNameFilter(firstName));
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            filters.Add(BuildLastNameFilter(lastName));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            filters.Add(BuildCityFilter(city));
        }

        return CombineFilters(filters);
    }

    private Expression<Func<User, bool>> BuildEmailFilter(string email)
    {
        if (email.StartsWith("*") && email.EndsWith("*"))
        {
            // *termo* - contém
            var term = email.Trim('*');
            return u => u.Email.ToLower().Contains(term.ToLower());
        }
        else if (email.StartsWith("*"))
        {
            // *termo - termina com
            var term = email.TrimStart('*');
            return u => u.Email.ToLower().EndsWith(term.ToLower());
        }
        else if (email.EndsWith("*"))
        {
            // termo* - começa com
            var term = email.TrimEnd('*');
            return u => u.Email.ToLower().StartsWith(term.ToLower());
        }
        else
        {
            // termo exato (case insensitive)
            return u => u.Email.ToLower().Contains(email.ToLower());
        }
    }

    private Expression<Func<User, bool>> BuildUsernameFilter(string username)
    {
        if (username.StartsWith("*") && username.EndsWith("*"))
        {
            // *termo* - contém
            var term = username.Trim('*');
            return u => u.Username.ToLower().Contains(term.ToLower());
        }
        else if (username.StartsWith("*"))
        {
            // *termo - termina com
            var term = username.TrimStart('*');
            return u => u.Username.ToLower().EndsWith(term.ToLower());
        }
        else if (username.EndsWith("*"))
        {
            // termo* - começa com
            var term = username.TrimEnd('*');
            return u => u.Username.ToLower().StartsWith(term.ToLower());
        }
        else
        {
            // termo exato (case insensitive)
            return u => u.Username.ToLower().Contains(username.ToLower());
        }
    }

    private Expression<Func<User, bool>> BuildStatusFilter(string status)
    {
        if (Enum.TryParse<UserStatus>(status, true, out var userStatus))
        {
            return u => u.Status == userStatus;
        }

        return u => true;
    }

    private Expression<Func<User, bool>> BuildRoleFilter(string role)
    {
        if (Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            return u => u.Role == userRole;
        }

        return u => true;
    }

    private Expression<Func<User, bool>> BuildFirstNameFilter(string firstName)
    {
        if (firstName.StartsWith("*") && firstName.EndsWith("*"))
        {
            // *termo* - contém
            var term = firstName.Trim('*');
            return u => u.Name.FirstName.ToLower().Contains(term.ToLower());
        }
        else if (firstName.StartsWith("*"))
        {
            // *termo - termina com
            var term = firstName.TrimStart('*');
            return u => u.Name.FirstName.ToLower().EndsWith(term.ToLower());
        }
        else if (firstName.EndsWith("*"))
        {
            // termo* - começa com
            var term = firstName.TrimEnd('*');
            return u => u.Name.FirstName.ToLower().StartsWith(term.ToLower());
        }
        else
        {
            // termo exato (case insensitive)
            return u => u.Name.FirstName.ToLower().Contains(firstName.ToLower());
        }
    }

    private Expression<Func<User, bool>> BuildLastNameFilter(string lastName)
    {
        if (lastName.StartsWith("*") && lastName.EndsWith("*"))
        {
            // *termo* - contém
            var term = lastName.Trim('*');
            return u => u.Name.LastName.ToLower().Contains(term.ToLower());
        }
        else if (lastName.StartsWith("*"))
        {
            // *termo - termina com
            var term = lastName.TrimStart('*');
            return u => u.Name.LastName.ToLower().EndsWith(term.ToLower());
        }
        else if (lastName.EndsWith("*"))
        {
            // termo* - começa com
            var term = lastName.TrimEnd('*');
            return u => u.Name.LastName.ToLower().StartsWith(term.ToLower());
        }
        else
        {
            // termo exato (case insensitive)
            return u => u.Name.LastName.ToLower().Contains(lastName.ToLower());
        }
    }

    private Expression<Func<User, bool>> BuildCityFilter(string city)
    {
        if (city.StartsWith("*") && city.EndsWith("*"))
        {
            // *termo* - contém
            var term = city.Trim('*');
            return u => u.Address != null && u.Address.City.ToLower().Contains(term.ToLower());
        }
        else if (city.StartsWith("*"))
        {
            // *termo - termina com
            var term = city.TrimStart('*');
            return u => u.Address != null && u.Address.City.ToLower().EndsWith(term.ToLower());
        }
        else if (city.EndsWith("*"))
        {
            // termo* - começa com
            var term = city.TrimEnd('*');
            return u => u.Address != null && u.Address.City.ToLower().StartsWith(term.ToLower());
        }
        else
        {
            // termo exato (case insensitive)
            return u => u.Address != null && u.Address.City.ToLower().Contains(city.ToLower());
        }
    }

    private Expression<Func<User, bool>> CombineFilters(List<Expression<Func<User, bool>>> filters)
    {
        if (!filters.Any())
        {
            return u => true; // Sem filtros, retorna todos
        }

        Expression<Func<User, bool>> combinedFilter = filters.First();

        for (int i = 1; i < filters.Count; i++)
        {
            combinedFilter = CombineWithAnd(combinedFilter, filters[i]);
        }

        return combinedFilter;
    }

    private Expression<Func<User, bool>> CombineWithAnd(
        Expression<Func<User, bool>> left,
        Expression<Func<User, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(User), "u");
        var leftBody = ReplaceParameter(left.Body, left.Parameters[0], parameter);
        var rightBody = ReplaceParameter(right.Body, right.Parameters[0], parameter);
        var andExpression = Expression.AndAlso(leftBody, rightBody);
        return Expression.Lambda<Func<User, bool>>(andExpression, parameter);
    }

    private Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}
