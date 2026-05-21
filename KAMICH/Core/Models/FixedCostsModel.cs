namespace KAMICH.Core.Models;

public class FixedCostsModel
{
    public double Hotel { get; set; }
    public double Transport { get; set; }
    public double Service { get; set; }
    public double Other { get; set; }

    public double Total => Hotel + Transport + Service + Other;
}
