using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SalaryCalculation.Shared.Extensions.EnumExtensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        return value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DisplayAttribute>()
            ?.GetName() ?? value.ToString();
    }

    public static string GetDescription(this Enum value)
    {
        return value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DescriptionAttribute>()
            ?.Description ?? value.ToString();
    }

    public static IEnumerable<T> ForEach<T>() where T : Enum
    {
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            yield return value;
        }
    }
}