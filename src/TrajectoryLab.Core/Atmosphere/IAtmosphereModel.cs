namespace TrajectoryLab.Core.Atmosphere;

// Fournit les propriétés atmosphériques nécessaires aux modèles physiques.
public interface IAtmosphereModel
{
    double GetTemperature(double Altitude);

    double GetAirDensity(double Altitude);
}