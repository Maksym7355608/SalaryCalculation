namespace SalaryCalculation.Shared.Extensions.MoreLinq;

public static class MoreLinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var element in enumerable)
            action(element);
    }
}