using Identity.Data.Enums;
using Microsoft.AspNetCore.Authorization;

namespace SalaryCalculation.Shared.Common.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Field, Inherited = false)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public EPermission Permission { get; }
    public HasPermissionAttribute(EPermission permission) : base(permission.ToString())
    {
        Permission = permission;
    }
}