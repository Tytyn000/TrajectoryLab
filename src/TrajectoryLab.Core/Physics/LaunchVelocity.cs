using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Physics;

/// Fournit des méthodes pour calculer un vecteur de vitesse initiale.
public static class LaunchVelocity
{
    /// Construit une vitesse initiale à partir d'une norme,
    /// d'un angle d'élévation et d'un azimut.
    public static Vector3D FromSpeedAndAngles(
        double Speed,
        double ElevationDegrees,
        double AzimuthDegrees = 0.0) //Azimuth = direction horizontale (0° = Nord, 90° = Est, 180° = Sud, 270° = Ouest)
    {
        if (Speed < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Speed),
                "La vitesse ne peut pas être négative."
            );
        }

        double ElevationRadians =
            DegreesToRadians(ElevationDegrees);

        double AzimuthRadians =
            DegreesToRadians(AzimuthDegrees);

        double HorizontalSpeed =
            Speed * Math.Cos(ElevationRadians);

        double VelocityX = HorizontalSpeed * Math.Cos(AzimuthRadians);

        double VelocityY = HorizontalSpeed * Math.Sin(AzimuthRadians);

        double VelocityZ = Speed * Math.Sin(ElevationRadians);

        return new Vector3D
        (
            VelocityX,
            VelocityY,
            VelocityZ
        );
    }

    private static double DegreesToRadians(double Degrees)
    {
        return Degrees * Math.PI / 180.0;
    }
}