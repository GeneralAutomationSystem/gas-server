using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gas.Common.Models.Device;

public class Interval
{
    public int Begin { get; set; }
    public int End { get; set; }

    [JsonIgnore]
    public int Size
    {
        get { return End - Begin; }
    }

    public Interval ZeroEnd()
    {
        return new Interval
        {
            Begin = 0,
            End = End,
        };
    }

    public Interval BeginMax(int max)
    {
        return new Interval
        {
            Begin = Begin,
            End = max,
        };
    }

    public void Trim(int min, int max)
    {
        Begin = TrimInt(Begin, min, max);
        End = TrimInt(End, min, max);
    }

    private static int TrimInt(int x, int min, int max)
    {
        if (x < min)
        {
            return min;
        }

        if (max < x)
        {
            return max;
        }

        return x;
    }

}
