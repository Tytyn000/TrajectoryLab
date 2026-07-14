using System.Collections.ObjectModel;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core;

public sealed class FlightConditionCalculator
{
    private readonly MachNumberCalculator _MachNumberCalculator;

    public IdealGasModel GasModel { get; }

    public FlightConditionCalculator(
        IdealGasModel GasModel)
    {
        if (GasModel is null)
        {
            throw new ArgumentNullException(
                nameof(GasModel),
                "Le modèle de gaz parfait ne peut pas être nul.");
        }

        this.GasModel = GasModel;

        _MachNumberCalculator =
            new MachNumberCalculator(GasModel);
    }

    public FlightCondition Calculate(
        SimulationState State,
        SimulationParameters Parameters)
    {
        if (ReferenceEquals(Parameters, null))
        {
            throw new ArgumentNullException(
                nameof(Parameters),
                "Les paramètres de simulation ne peuvent pas être nuls.");
        }

        return Calculate(
            State.Position,
            State.Velocity,
            State.Time,
            Parameters.Environment);
    }

    public FlightCondition Calculate(
        Vector3D Position,
        Vector3D Velocity,
        double Time,
        EnvironmentParameters Environment)
    {
        ValidateVector(
            Position,
            nameof(Position));

        ValidateVector(
            Velocity,
            nameof(Velocity));

        ValidateTime(Time);

        if (ReferenceEquals(Environment, null))
        {
            throw new ArgumentNullException(
                nameof(Environment),
                "Les paramètres d'environnement ne peuvent pas être nuls.");
        }

        Vector3D WindVelocity =
            Environment.WindModel.GetWindVelocity(
                Position,
                Time);

        ValidateVector(
            WindVelocity,
            nameof(WindVelocity));

        Vector3D RelativeVelocity =
            Velocity - WindVelocity;

        ValidateVector(
            RelativeVelocity,
            nameof(RelativeVelocity));

        double RelativeSpeed =
            GetMagnitude(RelativeVelocity);

        if (!double.IsFinite(RelativeSpeed))
        {
            throw new InvalidOperationException(
                "La norme de la vitesse relative calculée doit être finie.");
        }

        double Temperature =
            Environment.AtmosphereModel.GetTemperature(
                Position.Z);

        double SpeedOfSound =
            GasModel.GetSpeedOfSound(Temperature);

        double MachNumber =
            _MachNumberCalculator.GetMachNumber(
                RelativeSpeed,
                Temperature);

        if (!double.IsFinite(MachNumber)
            || MachNumber < 0.0)
        {
            throw new InvalidOperationException(
                "Le nombre de Mach calculé doit être fini et positif ou nul.");
        }

        return new FlightCondition(
            Time,
            Position,
            WindVelocity,
            RelativeVelocity,
            RelativeSpeed,
            Temperature,
            SpeedOfSound,
            MachNumber);
    }

    public IReadOnlyList<FlightCondition> CalculateAll(
        SimulationResult Result,
        SimulationParameters Parameters)
    {
        if (ReferenceEquals(Result, null))
        {
            throw new ArgumentNullException(
                nameof(Result),
                "Le résultat de simulation ne peut pas être nul.");
        }

        if (ReferenceEquals(Parameters, null))
        {
            throw new ArgumentNullException(
                nameof(Parameters),
                "Les paramètres de simulation ne peuvent pas être nuls.");
        }

        FlightCondition[] Conditions =
            new FlightCondition[Result.States.Count];

        for (int Index = 0;
             Index < Result.States.Count;
             Index++)
        {
            Conditions[Index] = Calculate(
                Result.States[Index],
                Parameters);
        }

        ReadOnlyCollection<FlightCondition>
            ReadOnlyConditions =
                Array.AsReadOnly(Conditions);

        return ReadOnlyConditions;
    }

    private static double GetMagnitude(
        Vector3D Vector)
    {
        double Scale = Math.Max(
            Math.Abs(Vector.X),
            Math.Max(
                Math.Abs(Vector.Y),
                Math.Abs(Vector.Z)));

        if (Scale == 0.0)
        {
            return 0.0;
        }

        double ScaledX = Vector.X / Scale;
        double ScaledY = Vector.Y / Scale;
        double ScaledZ = Vector.Z / Scale;

        return Scale * Math.Sqrt(
            ScaledX * ScaledX
            + ScaledY * ScaledY
            + ScaledZ * ScaledZ);
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

    private static void ValidateTime(double Time)
    {
        if (!double.IsFinite(Time)
            || Time < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Time),
                Time,
                "Le temps doit être un nombre fini supérieur ou égal à zéro.");
        }
    }
}