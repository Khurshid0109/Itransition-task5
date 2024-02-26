using DataGenerator.Domain.Enums;

namespace DataGenerator.Models;
public class DataRequestModel
{
    public Regions Region { get; set; }
    public double MistakesRate { get; set; }
    public int Seed { get; set; }
    public int PageNumber { get; set; }
}
