using OnlyFarms.Core.Data;

namespace OnlyFarms.Core.Infrastructure;

public static class CollectionExtensions
{
    public static T? Find<T>(this ICollection<T> list, int id) where T : IHasId
    {
        return list.FirstOrDefault(item => item.Id == id);
    }
}