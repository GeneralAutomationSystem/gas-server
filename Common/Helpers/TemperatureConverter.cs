namespace Gas.Common.Helpers;

public static class TemperatureConverter
{
    private const double c0 = 9.9081205150721914e+001;
    private const double c1 = -3.8807163082565111e+001;
    private const double c2 = 3.4060047037341250e+000;
    private const double c3 = -2.4407647483171743e-001;
    private const double c4 = 6.2231311180038868e-003;
    private const double c5 = 4.9651478947491500e-004;

    public static double ToCelsius(double voltageLevel)
    {
        if (voltageLevel <= 0 || 4096 <= voltageLevel)
        {
            throw new ArgumentException("Voltage level must be in 12bit resolution.");
        }

        double x = Math.Log((40960 / voltageLevel) - 10);

        return c0 + c1 * x + c2 * Math.Pow(x, 2) + c3 * Math.Pow(x, 3) + c4 * Math.Pow(x, 4) + c5 * Math.Pow(x, 5);
    }
}