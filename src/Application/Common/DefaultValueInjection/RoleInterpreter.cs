using System.Text.RegularExpressions;
using Application.Common.Abstractions;
using Selise.Ecap.Infrastructure;

namespace Application.Common.DefaultValueInjection;

public class RoleInterpreter
{
    private readonly IRmwSecurityContextProvider _securityContextProvider;

    public RoleInterpreter(IRmwSecurityContextProvider securityDataProvider)
    {
        _securityContextProvider = securityDataProvider;
    }

    public string[] InterpretRoles(string role)
    {
        var authData = _securityContextProvider.GetSecurityContext();

        const string regexPrefix = "regex_";
        if (role.Equals("OWNERROLE", StringComparison.InvariantCultureIgnoreCase))
            return BuildRoles(authData.Roles.ToList(), Roles.SystemDefinedRoles);
            
        if (!role.StartsWith(regexPrefix, StringComparison.InvariantCultureIgnoreCase)) return new[] { role };

        var specialUserRoles = BuildRoles(authData.Roles.ToList(), Roles.SystemDefinedRoles);
        var regex = role.Substring(regexPrefix.Length);
        return BuildRolesBasedOnRegex(specialUserRoles, regex);
    }

    private static string[] BuildRolesBasedOnRegex(IEnumerable<string> specialUserRole, string regex)
    {
        return (from role in specialUserRole let reg = new Regex(@regex) where reg.IsMatch(role) select role).ToArray();
    }

    public string[] InterpretIds(string role)
    {
        var authData = _securityContextProvider.GetSecurityContext();
        return role.Equals("OWNER", StringComparison.InvariantCultureIgnoreCase) 
            ? new[] { authData.UserId } 
            : new[] { string.Empty };
    }

    private static string[] BuildRoles(IEnumerable<string> roles, string[] systemDefinedRoles) 
        => roles.Where(role => IsASystemDefinedRole(role, systemDefinedRoles) == false).ToArray();

    private static bool IsASystemDefinedRole(string role, string[] systemDefinedRoles) 
        => Array.IndexOf(systemDefinedRoles, role.ToLower()) != -1;
}