namespace OnlyFarms.Core.Data;

public interface ICompany :IHasId
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string UniqueCompanyCode { get; init; }
    public int WaterSupply { get; set; }
}