namespace TrajectoryLab.Core.Atmosphere;

/// Modélise les couches inférieures de l'atmosphère standard de 1976.
public sealed class LowerAtmosphere1976Model : IAtmosphereModel
{
    public const double SeaLevelTemperature = 288.15;

    public const double SeaLevelPressure = 101_325.0;

    public const double StandardGravity = 9.80665;

    public const double SpecificGasConstant = 287.05287;

    public const double EarthRadius = 6_356_766.0;

    public const double MinimumSupportedAltitude = -5_000.0;

    public const double MaximumGeopotentialAltitude = 84_852.0;

    private static readonly double[] BaseGeopotentialAltitudes =
    [
        0.0,
        11_000.0,
        20_000.0,
        32_000.0,
        47_000.0,
        51_000.0,
        71_000.0,
        84_852.0
    ];

    private static readonly double[] TemperatureGradients =
    [
        -0.0065,
        0.0,
        0.0010,
        0.0028,
        0.0,
        -0.0028,
        -0.0020
    ];

    private static readonly double[] BaseTemperatures = CreateBaseTemperatures();

    private static readonly double[] BasePressures = CreateBasePressures();

    public const double MaximumSupportedAltitude = 86_000.0;

    public double GetTemperature(double Altitude)
    {
        AtmosphericState State =
            CalculateState(Altitude);

        return State.Temperature;
    }

    public double GetPressure(double Altitude)
    {
        AtmosphericState State =
            CalculateState(Altitude);

        return State.Pressure;
    }

    public double GetAirDensity(double Altitude)
    {
        AtmosphericState State =
            CalculateState(Altitude);

        return State.AirDensity;
    }

    public static double GeometricToGeopotentialAltitude(
        double GeometricAltitude)
    {
        if (!double.IsFinite(GeometricAltitude))
        {
            throw new ArgumentOutOfRangeException(
                nameof(GeometricAltitude),
                "L'altitude géométrique doit être finie."
            );
        }

        if (GeometricAltitude <= -EarthRadius)
        {
            throw new ArgumentOutOfRangeException(
                nameof(GeometricAltitude),
                "L'altitude géométrique est incompatible avec le rayon terrestre."
            );
        }

        return EarthRadius * GeometricAltitude /
            (EarthRadius + GeometricAltitude);
    }

    public static double GeopotentialToGeometricAltitude(
        double GeopotentialAltitude)
    {
        if (!double.IsFinite(GeopotentialAltitude))
        {
            throw new ArgumentOutOfRangeException(
                nameof(GeopotentialAltitude),
                "L'altitude géopotentielle doit être finie."
            );
        }

        if (GeopotentialAltitude >= EarthRadius)
        {
            throw new ArgumentOutOfRangeException(
                nameof(GeopotentialAltitude),
                "L'altitude géopotentielle est incompatible avec le rayon terrestre."
            );
        }

        return EarthRadius * GeopotentialAltitude /
            (EarthRadius - GeopotentialAltitude);
    }

    private static AtmosphericState CalculateState(
        double Altitude)
    {
        ValidateAltitude(Altitude);

        double GeopotentialAltitude =
            GeometricToGeopotentialAltitude(
                Altitude
            );

        int LayerIndex =
            FindLayerIndex(
                GeopotentialAltitude
            );

        double BaseAltitude =
            BaseGeopotentialAltitudes[LayerIndex];

        double BaseTemperature =
            BaseTemperatures[LayerIndex];

        double BasePressure =
            BasePressures[LayerIndex];

        double TemperatureGradient =
            TemperatureGradients[LayerIndex];

        double AltitudeDifference =
            GeopotentialAltitude -
            BaseAltitude;

        double Temperature =
            BaseTemperature +
            TemperatureGradient *
            AltitudeDifference;

        double Pressure =
            CalculatePressure(
                BasePressure,
                BaseTemperature,
                Temperature,
                TemperatureGradient,
                AltitudeDifference
            );

        // L'équation des gaz parfaits donne rho = P / (R * T).
        double AirDensity =
            Pressure /
            (
                SpecificGasConstant *
                Temperature
            );

        return new AtmosphericState(
            Temperature,
            Pressure,
            AirDensity
        );
    }

    private static double CalculatePressure(
    double BasePressure,
    double BaseTemperature,
    double Temperature,
    double TemperatureGradient,
    double AltitudeDifference)
    {
        if (TemperatureGradient == 0.0)
        {
            // Dans une couche isotherme, la pression suit une loi exponentielle.
            double IsothermalExponent =
                -StandardGravity *
                AltitudeDifference /
                (
                    SpecificGasConstant *
                    BaseTemperature
                );

            return BasePressure *
                Math.Exp(IsothermalExponent);
        }

        // Dans une couche à gradient thermique constant,
        // l'équilibre hydrostatique conduit à une loi de puissance.
        double GradientExponent =
            StandardGravity /
            (
                SpecificGasConstant *
                TemperatureGradient
            );

        return BasePressure *
            Math.Pow(
                BaseTemperature / Temperature,
                GradientExponent
            );
    }

    private static int FindLayerIndex(
        double GeopotentialAltitude)
    {
        if (GeopotentialAltitude < 0.0)
        {
            return 0;
        }

        for (
            int Index =
                BaseGeopotentialAltitudes.Length - 2;
            Index >= 0;
            Index--)
        {
            if (GeopotentialAltitude >=
                BaseGeopotentialAltitudes[Index])
            {
                return Index;
            }
        }

        return 0;
    }

    private static double[] CreateBaseTemperatures()
    {
        double[] Temperatures =
            new double[
                BaseGeopotentialAltitudes.Length
            ];

        Temperatures[0] =
            SeaLevelTemperature;

        for (
            int Index = 0;
            Index < TemperatureGradients.Length;
            Index++)
        {
            double LayerHeight =
                BaseGeopotentialAltitudes[Index + 1] -
                BaseGeopotentialAltitudes[Index];

            Temperatures[Index + 1] =
                Temperatures[Index] +
                TemperatureGradients[Index] *
                LayerHeight;
        }

        return Temperatures;
    }

    private static double[] CreateBasePressures()
    {
        double[] Pressures =
            new double[
                BaseGeopotentialAltitudes.Length
            ];

        Pressures[0] =
            SeaLevelPressure;

        for (
            int Index = 0;
            Index < TemperatureGradients.Length;
            Index++)
        {
            double LayerHeight =
                BaseGeopotentialAltitudes[Index + 1] -
                BaseGeopotentialAltitudes[Index];

            Pressures[Index + 1] =
                CalculatePressure(
                    Pressures[Index],
                    BaseTemperatures[Index],
                    BaseTemperatures[Index + 1],
                    TemperatureGradients[Index],
                    LayerHeight
                );
        }

        return Pressures;
    }

    private static void ValidateAltitude(
        double Altitude)
    {
        if (!double.IsFinite(Altitude))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Altitude),
                "L'altitude doit être finie."
            );
        }

        if (Altitude < MinimumSupportedAltitude ||
            Altitude > MaximumSupportedAltitude)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Altitude),
                $"L'altitude doit être comprise entre " +
                $"{MinimumSupportedAltitude} m et " +
                $"{MaximumSupportedAltitude:F3} m."
            );
        }
    }

    private readonly record struct AtmosphericState(
        double Temperature,
        double Pressure,
        double AirDensity
    );
}