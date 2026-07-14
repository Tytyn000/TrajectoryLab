namespace TrajectoryLab.Core.Atmosphere;

/// Réunit les modèles inférieur et supérieur de l'atmosphère standard de 1976.
public sealed class StandardAtmosphere1976Model : IAtmosphereModel
{
    public const double MinimumSupportedAltitude =
        -5_000.0;

    public const double TransitionAltitude =
        86_000.0;

    public const double TemperatureBlendEndAltitude =
        91_000.0;

    public const double MaximumSupportedAltitude =
        1_000_000.0;

    private static readonly LowerAtmosphere1976Model
        LowerAtmosphereModel =
            new();

    private static readonly UpperAtmosphere1976Model
        UpperAtmosphereModel =
            new();

    private static readonly double
        UpperPressureCorrectionFactor =
            LowerAtmosphereModel.GetPressure(
                TransitionAltitude
            ) /
            UpperAtmosphereModel.GetPressure(
                TransitionAltitude
            );

    private static readonly double
        UpperDensityCorrectionFactor =
            LowerAtmosphereModel.GetAirDensity(
                TransitionAltitude
            ) /
            UpperAtmosphereModel.GetAirDensity(
                TransitionAltitude
            );

    public double GetTemperature(double Altitude)
    {
        ValidateAltitude(Altitude);

        if (Altitude <= TransitionAltitude)
        {
            return LowerAtmosphereModel.GetTemperature(
                Altitude
            );
        }

        if (Altitude >= TemperatureBlendEndAltitude)
        {
            return UpperAtmosphereModel.GetTemperature(
                Altitude
            );
        }

        double BlendRatio =
            (
                Altitude -
                TransitionAltitude
            ) /
            (
                TemperatureBlendEndAltitude -
                TransitionAltitude
            );

        // Ce raccord cubique conserve une dérivée nulle
        // aux deux extrémités de la zone de transition.
        double SmoothBlendRatio =
            BlendRatio *
            BlendRatio *
            (
                3.0 -
                2.0 * BlendRatio
            );

        double LowerBoundaryTemperature =
            LowerAtmosphereModel.GetTemperature(
                TransitionAltitude
            );

        double UpperTemperature =
            UpperAtmosphereModel.GetTemperature(
                Altitude
            );

        return LowerBoundaryTemperature +
            SmoothBlendRatio *
            (
                UpperTemperature -
                LowerBoundaryTemperature
            );
    }

    public double GetPressure(double Altitude)
    {
        ValidateAltitude(Altitude);

        if (Altitude <= TransitionAltitude)
        {
            return LowerAtmosphereModel.GetPressure(
                Altitude
            );
        }

        return UpperAtmosphereModel.GetPressure(
            Altitude
        ) * UpperPressureCorrectionFactor;
    }

    public double GetAirDensity(double Altitude)
    {
        ValidateAltitude(Altitude);

        if (Altitude <= TransitionAltitude)
        {
            return LowerAtmosphereModel.GetAirDensity(
                Altitude
            );
        }

        return UpperAtmosphereModel.GetAirDensity(
            Altitude
        ) * UpperDensityCorrectionFactor;
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