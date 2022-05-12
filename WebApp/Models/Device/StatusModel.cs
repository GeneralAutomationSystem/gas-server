namespace Gas.WebApp.Models;

public class StatusModel : BaseModel
{
    public List<(DateTime, int)> Rssis { get; set; } = new();
    public List<(DateTime, double)> SystemTemperatures { get; set; } = new();
}