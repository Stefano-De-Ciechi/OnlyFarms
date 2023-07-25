using OnlyFarms.Data;

namespace OnlyFarms.Infrastructure;

public static class ColletionExtensions
{
    public static T? Find<T>(this ICollection<T> list, int id) where T : IHasId
    {
        if (list == null) return default;
        return list.FirstOrDefault(item => item.Id == id);
    }
}