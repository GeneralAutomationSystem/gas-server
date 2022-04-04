namespace Gas.WebApp.Models;

public class StatusModel : BaseModel
{
    public IEnumerable<(string, int)>? SystemTemperatures { get; set; }
}