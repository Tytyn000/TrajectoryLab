using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core;

public sealed class MachNumberCalculator
{
    public IdealGasModel GasModel { get; }

    public MachNumberCalculator(IdealGasModel GasModel)
    {
        this.GasModel = GasModel
            ?? throw new ArgumentNullException(
                nameof(GasModel),
                "Le modèle de gaz parfait ne peut pas être nul.");
    }

    public double GetMachNumber(
        double RelativeSpeed,
        double Temperature)
    {
        ValidateRelativeSpeed(RelativeSpeed);

        double SpeedOfSound =
            GasModel.GetSpeedOfSound(Temperature);

        return RelativeSpeed / SpeedOfSound;
    }

    public double GetMachNumber(
        Vector3D BodyVelocity,
        Vector3D FluidVelocity,
        double Temperature)
    {
        ValidateVector(
            BodyVelocity,
            nameof(BodyVelocity));

        ValidateVector(
            FluidVelocity,
            nameof(FluidVelocity));

        double RelativeVelocityX =
            BodyVelocity.X - FluidVelocity.X;

        double RelativeVelocityY =
            BodyVelocity.Y - FluidVelocity.Y;

        double RelativeVelocityZ =
            BodyVelocity.Z - FluidVelocity.Z;

        if (!double.IsFinite(RelativeVelocityX)
            || !double.IsFinite(RelativeVelocityY)
            || !double.IsFinite(RelativeVelocityZ))
        {
            throw new ArgumentOutOfRangeException(
                nameof(BodyVelocity),
                "La vitesse relative calculée doit contenir uniquement des composantes finies.");
        }

        double RelativeSpeed = GetMagnitude(
            RelativeVelocityX,
            RelativeVelocityY,
            RelativeVelocityZ);

        if (!double.IsFinite(RelativeSpeed))
        {
            throw new ArgumentOutOfRangeException(
                nameof(BodyVelocity),
                "La norme de la vitesse relative doit être un nombre fini.");
        }

        return GetMachNumber(
            RelativeSpeed,
            Temperature);
    }

    private static double GetMagnitude(
        double X,
        double Y,
        double Z)
    {
        double Scale = Math.Max(
            Math.Abs(X),
            Math.Max(
                Math.Abs(Y),
                Math.Abs(Z)));

        if (Scale == 0.0)
        {
            return 0.0;
        }

        double ScaledX = X / Scale;
        double ScaledY = Y / Scale;
        double ScaledZ = Z / Scale;

        return Scale * Math.Sqrt(
            ScaledX * ScaledX
            + ScaledY * ScaledY
            + ScaledZ * ScaledZ);
    }

    private static void ValidateRelativeSpeed(
        double RelativeSpeed)
    {
        if (!double.IsFinite(RelativeSpeed)
            || RelativeSpeed < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(RelativeSpeed),
                RelativeSpeed,
                "La vitesse relative doit être un nombre fini supérieur ou égal à zéro.");
        }
    }

    private static void ValidateVector(
        Vector3D Vector,
        string ParameterName)
    {
        if (!double.IsFinite(Vector.X)
            || !double.IsFinite(Vector.Y)
            || !double.IsFinite(Vector.Z))
        {
            throw new ArgumentException(
                "Le vecteur doit contenir uniquement des composantes finies.",
                ParameterName);
        }
    }
}