using MongoDB.Driver;

namespace SalaryCalculation.Shared.Extensions.MoreLinq;

public static class MoreLinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var element in enumerable)
            action(element);
    }

    public static void ParallelForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        enumerable.AsParallel().ForEach(action);
    }

    public static long NewNumberId<T>(this IMongoCollection<T> collection) =>
        collection.Find(Builders<T>.Filter.Empty).CountDocuments()+1;
}