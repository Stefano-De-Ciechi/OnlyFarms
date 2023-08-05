namespace OnlyFarms.RestApi.Infrastructure;

public class NotFoundException<T> : Exception where T : IHasId
{
    public NotFoundException(int id) : base($"no resource of type '{ typeof(T).Name }' with ID = { id } was found")
    { }
}