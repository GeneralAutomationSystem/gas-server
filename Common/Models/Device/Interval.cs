using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gas.Common.Models.Device;

public class Interval
{
    public int Start { get; set; }
    public int End { get; set; }

    [JsonIgnore]
    public int Size
    {
        get { return End - Start; }
    }

    public Interval ZeroEnd()
    {
        return new Interval
        {
            Start = 0,
            End = End,
        };
    }

    public Interval StartMax(int max)
    {
        return new Interval
        {
            Start = Start,
            End = max,
        };
    }

    public void Trim(int min, int max)
    {
        Start = TrimInt(Start, min, max);
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
