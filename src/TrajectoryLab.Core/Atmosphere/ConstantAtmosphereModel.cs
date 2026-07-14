namespace TrajectoryLab.Core.Atmosphere;

/// Représente une atmosphère dont la température et la densité
/// ne varient pas avec l'altitude.
public sealed class ConstantAtmosphereModel : IAtmosphereModel
{
    public const double DefaultTemperature = 288.15;

    public double AirDensity { get; }

    public double Temperature { get; }

    public ConstantAtmosphereModel(double AirDensity)
        : this(
            AirDensity,
            DefaultTemperature)
    {
    }

    public ConstantAtmosphereModel(
        double AirDensity,
        double Temperature)
    {
        ValidateAirDensity(AirDensity);
        ValidateTemperature(Temperature);

        this.AirDensity = AirDensity;
        this.Temperature = Temperature;
    }

    public double GetTemperature(double Altitude)
    {
        ValidateAltitude(Altitude);

        return Temperature;
    }

    public double GetAirDensity(double Altitude)
    {
        ValidateAltitude(Altitude);

        return AirDensity;
    }

    private static void ValidateAirDensity(
        double AirDensity)
    {
        // Une densité nulle représente une simulation dans le vide.
        if (!double.IsFinite(AirDensity)
            || AirDensity < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(AirDensity),
                AirDensity,
                "La densité de l'air doit être finie et positive ou nulle.");
        }
    }

    private static void ValidateTemperature(
        double Temperature)
    {
        if (!double.IsFinite(Temperature)
            || Temperature <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Temperature),
                Temperature,
                "La température doit être finie et strictement positive.");
        }
    }

    private static void ValidateAltitude(
        double Altitude)
    {
        if (!double.IsFinite(Altitude))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Altitude),
                Altitude,
                "L'altitude doit être finie.");
        }
    }
}