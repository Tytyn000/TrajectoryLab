using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Simulation;

public sealed record AnalyticalEquationVariable(
    string Symbol,
    string Unit,
    string Description,
    double Value);

public sealed record AnalyticalEquationDescription(
    string Title,
    string Latex,
    IReadOnlyList<AnalyticalEquationVariable> Variables);

public sealed record ChiSquareComparisonResult(
    int PointCount,
    int DegreesOfFreedom,
    double StandardUncertainty,
    double ChiSquare,
    double ReducedChiSquare,
    double RootMeanSquareError,
    double MaximumAbsoluteError);

/// Fournit une référence analytique locale à accélération gravitationnelle constante.
public sealed class ConstantGravityAnalyticalModel
{
    private const double HorizontalSpeedTolerance =
        1.0e-12;

    public Vector3D InitialPosition { get; }

    public Vector3D InitialVelocity { get; }

    public double GravityAcceleration { get; }

    public string MotionName
    {
        get
        {
            double HorizontalSpeed =
                Math.Sqrt(
                    InitialVelocity.X *
                    InitialVelocity.X +
                    InitialVelocity.Y *
                    InitialVelocity.Y);

            return HorizontalSpeed <=
                HorizontalSpeedTolerance
                    ? "Chute libre verticale à gravité uniforme"
                    : "Mouvement balistique parabolique à gravité uniforme";
        }
    }

    public ConstantGravityAnalyticalModel(
        Vector3D InitialPosition,
        Vector3D InitialVelocity,
        double GravityAcceleration)
    {
        if (!IsFinite(InitialPosition))
        {
            throw new ArgumentOutOfRangeException(
                nameof(InitialPosition),
                "La position initiale doit être finie.");
        }

        if (!IsFinite(InitialVelocity))
        {
            throw new ArgumentOutOfRangeException(
                nameof(InitialVelocity),
                "La vitesse initiale doit être finie.");
        }

        if (
            !double.IsFinite(GravityAcceleration) ||
            GravityAcceleration <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(GravityAcceleration),
                "La gravité doit être finie et strictement positive.");
        }

        this.InitialPosition =
            InitialPosition;

        this.InitialVelocity =
            InitialVelocity;

        this.GravityAcceleration =
            GravityAcceleration;
    }

    public SimulationState Evaluate(
        double Time)
    {
        if (!double.IsFinite(Time) || Time < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Time),
                "Le temps doit être fini et positif ou nul.");
        }

        Vector3D Position =
            new(
                InitialPosition.X +
                InitialVelocity.X * Time,

                InitialPosition.Y +
                InitialVelocity.Y * Time,

                InitialPosition.Z +
                InitialVelocity.Z * Time -
                0.5 *
                GravityAcceleration *
                Time *
                Time);

        Vector3D Velocity =
            new(
                InitialVelocity.X,
                InitialVelocity.Y,
                InitialVelocity.Z -
                GravityAcceleration * Time);

        return new SimulationState(
            Time,
            Position,
            Velocity);
    }

    public bool SupportsQuantity(
        TrajectoryQuantity Quantity)
    {
        return Quantity is
            TrajectoryQuantity.Time or
            TrajectoryQuantity.PositionX or
            TrajectoryQuantity.PositionY or
            TrajectoryQuantity.Altitude or
            TrajectoryQuantity.HorizontalDistance or
            TrajectoryQuantity.Speed;
    }

    public double GetValue(
        TrajectoryQuantity Quantity,
        SimulationState State)
    {
        return Quantity switch
        {
            TrajectoryQuantity.Time =>
                State.Time,

            TrajectoryQuantity.PositionX =>
                State.Position.X,

            TrajectoryQuantity.PositionY =>
                State.Position.Y,

            TrajectoryQuantity.Altitude =>
                State.Position.Z,

            TrajectoryQuantity.HorizontalDistance =>
                GetHorizontalDistance(
                    State.Position),

            TrajectoryQuantity.Speed =>
                State.Velocity.Length(),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(Quantity),
                    "Cette grandeur ne possède pas de référence analytique à gravité uniforme.")
        };
    }

    public AnalyticalEquationDescription GetEquation(
        TrajectoryQuantity Quantity)
    {
        if (!SupportsQuantity(Quantity))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Quantity),
                "Cette grandeur ne possède pas d'équation analytique dans ce modèle.");
        }

        return Quantity switch
        {
            TrajectoryQuantity.Time =>
                new AnalyticalEquationDescription(
                    "Temps",
                    "q(t)=t",
                    Array.AsReadOnly(
                        new[]
                        {
                            TimeVariable()
                        })),

            TrajectoryQuantity.PositionX =>
                new AnalyticalEquationDescription(
                    "Position Est",
                    "x(t)=x_0+v_{x0}t",
                    Array.AsReadOnly(
                        new[]
                        {
                            TimeVariable(),
                            Variable(
                                "x_0",
                                "m",
                                "Position Est initiale",
                                InitialPosition.X),
                            Variable(
                                "v_{x0}",
                                "m/s",
                                "Vitesse Est initiale",
                                InitialVelocity.X)
                        })),

            TrajectoryQuantity.PositionY =>
                new AnalyticalEquationDescription(
                    "Position Nord",
                    "y(t)=y_0+v_{y0}t",
                    Array.AsReadOnly(
                        new[]
                        {
                            TimeVariable(),
                            Variable(
                                "y_0",
                                "m",
                                "Position Nord initiale",
                                InitialPosition.Y),
                            Variable(
                                "v_{y0}",
                                "m/s",
                                "Vitesse Nord initiale",
                                InitialVelocity.Y)
                        })),

            TrajectoryQuantity.Altitude =>
                new AnalyticalEquationDescription(
                    "Altitude",
                    "z(t)=z_0+v_{z0}t-\\frac{1}{2}g_0t^2",
                    Array.AsReadOnly(
                        new[]
                        {
                            TimeVariable(),
                            Variable(
                                "z_0",
                                "m",
                                "Altitude initiale",
                                InitialPosition.Z),
                            Variable(
                                "v_{z0}",
                                "m/s",
                                "Vitesse verticale initiale",
                                InitialVelocity.Z),
                            GravityVariable()
                        })),

            TrajectoryQuantity.HorizontalDistance =>
                new AnalyticalEquationDescription(
                    "Distance horizontale",
                    "d_h(t)=t\\sqrt{v_{x0}^2+v_{y0}^2}",
                    Array.AsReadOnly(
                        new[]
                        {
                            TimeVariable(),
                            Variable(
                                "v_{x0}",
                                "m/s",
                                "Vitesse Est initiale",
                                InitialVelocity.X),
                            Variable(
                                "v_{y0}",
                                "m/s",
                                "Vitesse Nord initiale",
                                InitialVelocity.Y)
                        })),

            TrajectoryQuantity.Speed =>
                new AnalyticalEquationDescription(
                    "Norme de la vitesse",
                    "v(t)=\\sqrt{v_{x0}^2+v_{y0}^2+\\left(v_{z0}-g_0t\\right)^2}",
                    Array.AsReadOnly(
                        new[]
                        {
                            TimeVariable(),
                            Variable(
                                "v_{x0}",
                                "m/s",
                                "Vitesse Est initiale",
                                InitialVelocity.X),
                            Variable(
                                "v_{y0}",
                                "m/s",
                                "Vitesse Nord initiale",
                                InitialVelocity.Y),
                            Variable(
                                "v_{z0}",
                                "m/s",
                                "Vitesse verticale initiale",
                                InitialVelocity.Z),
                            GravityVariable()
                        })),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(Quantity))
        };
    }

    public ChiSquareComparisonResult Compare(
        IReadOnlyList<SimulationState> ObservedStates,
        TrajectoryQuantity ComparedQuantity,
        double StandardUncertainty)
    {
        ArgumentNullException.ThrowIfNull(
            ObservedStates);

        if (ObservedStates.Count == 0)
        {
            throw new ArgumentException(
                "La comparaison nécessite au moins un état.",
                nameof(ObservedStates));
        }

        if (!SupportsQuantity(ComparedQuantity))
        {
            throw new ArgumentOutOfRangeException(
                nameof(ComparedQuantity),
                "Cette grandeur ne peut pas être comparée au modèle analytique.");
        }

        if (
            !double.IsFinite(StandardUncertainty) ||
            StandardUncertainty <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(StandardUncertainty),
                "L'incertitude-type doit être finie et strictement positive.");
        }

        double ChiSquare =
            0.0;

        double SquaredErrorSum =
            0.0;

        double MaximumAbsoluteError =
            0.0;

        foreach (
            SimulationState ObservedState
            in ObservedStates)
        {
            SimulationState ExpectedState =
                Evaluate(
                    ObservedState.Time);

            double ObservedValue =
                GetValue(
                    ComparedQuantity,
                    ObservedState);

            double ExpectedValue =
                GetValue(
                    ComparedQuantity,
                    ExpectedState);

            double Residual =
                ObservedValue -
                ExpectedValue;

            double NormalizedResidual =
                Residual /
                StandardUncertainty;

            ChiSquare +=
                NormalizedResidual *
                NormalizedResidual;

            SquaredErrorSum +=
                Residual *
                Residual;

            MaximumAbsoluteError =
                Math.Max(
                    MaximumAbsoluteError,
                    Math.Abs(Residual));
        }

        int DegreesOfFreedom =
            ObservedStates.Count;

        return new ChiSquareComparisonResult(
            ObservedStates.Count,
            DegreesOfFreedom,
            StandardUncertainty,
            ChiSquare,
            ChiSquare / DegreesOfFreedom,
            Math.Sqrt(
                SquaredErrorSum /
                ObservedStates.Count),
            MaximumAbsoluteError);
    }

    private double GetHorizontalDistance(
        Vector3D Position)
    {
        double DeltaX =
            Position.X -
            InitialPosition.X;

        double DeltaY =
            Position.Y -
            InitialPosition.Y;

        return Math.Sqrt(
            DeltaX * DeltaX +
            DeltaY * DeltaY);
    }

    private AnalyticalEquationVariable
        TimeVariable()
    {
        return Variable(
            "t",
            "s",
            "Temps écoulé depuis le lancement",
            0.0);
    }

    private AnalyticalEquationVariable
        GravityVariable()
    {
        return Variable(
            "g_0",
            "m/s²",
            "Gravité uniforme locale de référence",
            GravityAcceleration);
    }

    private static AnalyticalEquationVariable Variable(
        string Symbol,
        string Unit,
        string Description,
        double Value)
    {
        return new AnalyticalEquationVariable(
            Symbol,
            Unit,
            Description,
            Value);
    }

    private static bool IsFinite(
        Vector3D Vector)
    {
        return
            double.IsFinite(Vector.X) &&
            double.IsFinite(Vector.Y) &&
            double.IsFinite(Vector.Z);
    }
}
