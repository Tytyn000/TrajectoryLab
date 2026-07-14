namespace TrajectoryLab.Core.Atmosphere;

// Modélise la température de l'atmosphère standard entre 86 km et 1 000 km.
public sealed class UpperAtmosphere1976TemperatureModel
{
    public const double MinimumSupportedAltitude =
        86_000.0;

    public const double MaximumSupportedAltitude =
        1_000_000.0;

    public const double ExosphericTemperature =
        1_000.0;

    public const double EarthRadiusKilometers =
        6_356.766;

    public double GetTemperature(double Altitude)
    {
        ValidateAltitude(Altitude);

        double AltitudeKilometers =
            Altitude / 1_000.0;

        if (AltitudeKilometers <= 91.0)
        {
            // La mésopause est isotherme entre 86 km et 91 km.
            return 186.8673;
        }

        if (AltitudeKilometers <= 110.0)
        {
            return CalculateCurvedTemperature(
                AltitudeKilometers
            );
        }

        if (AltitudeKilometers <= 120.0)
        {
            // La température augmente de 12 K par kilomètre.
            return 240.0 +
                12.0 *
                (
                    AltitudeKilometers -
                    110.0
                );
        }

        return CalculateThermosphericTemperature(
            AltitudeKilometers
        );
    }

    private static double CalculateCurvedTemperature(
        double AltitudeKilometers)
    {
        const double CenterTemperature =
            263.1905;

        const double TemperatureAmplitude =
            -76.3232;

        const double ReferenceAltitude =
            91.0;

        const double CurvatureScale =
            19.9429;

        double NormalizedAltitude =
            (
                AltitudeKilometers -
                ReferenceAltitude
            ) / CurvatureScale;

        double SquareRootArgument =
            1.0 -
            NormalizedAltitude *
            NormalizedAltitude;

        // Les arrondis numériques ne doivent pas produire
        // une valeur légèrement négative à la limite de couche.
        SquareRootArgument =
            Math.Max(
                0.0,
                SquareRootArgument
            );

        return CenterTemperature +
            TemperatureAmplitude *
            Math.Sqrt(
                SquareRootArgument
            );
    }

    private static double CalculateThermosphericTemperature(
        double AltitudeKilometers)
    {
        const double BaseAltitude =
            120.0;

        const double BaseTemperatureDifference =
            640.0;

        const double TemperatureGradientParameter =
            0.01875;

        double TransformedAltitude =
            (
                AltitudeKilometers -
                BaseAltitude
            ) *
            (
                EarthRadiusKilometers +
                BaseAltitude
            ) /
            (
                EarthRadiusKilometers +
                AltitudeKilometers
            );

        // La température converge progressivement vers 1 000 K.
        return ExosphericTemperature -
            BaseTemperatureDifference *
            Math.Exp(
                -TemperatureGradientParameter *
                TransformedAltitude
            );
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
                $"{MaximumSupportedAltitude} m."
            );
        }
    }
}