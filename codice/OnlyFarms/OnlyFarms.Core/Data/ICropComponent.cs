namespace OnlyFarms.Core.Data;

public interface ICropComponent : IHasId
{
    int FarmingCompanyId { get; set; }
    int CropId { get; set; }
}