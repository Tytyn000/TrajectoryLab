using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Core.Solvers;

/// Méthode adaptative de Dormand-Prince d'ordres 5 et 4.
public sealed class RungeKutta45Solver :
    IAdaptiveNumericalSolver
{
    private const double SafetyFactor = 0.9;
    private const double MinimumScaleFactor = 0.2;
    private const double MaximumScaleFactor = 5.0;
    private const double MinimumStepComparisonTolerance = 1.0e-12;

    public string Name => "Runge-Kutta 4(5) adaptatif";

    public double AbsoluteTolerance { get; }
    public double RelativeTolerance { get; }
    public double MinimumTimeStep { get; }
    public double MaximumTimeStep { get; }

    public RungeKutta45Solver(
        double AbsoluteTolerance = 1.0e-8,
        double RelativeTolerance = 1.0e-8,
        double MinimumTimeStep = 1.0e-6,
        double MaximumTimeStep = 1.0)
    {
        if (!double.IsFinite(AbsoluteTolerance) ||
            AbsoluteTolerance <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(AbsoluteTolerance),
                "La tolérance absolue doit être finie et strictement positive.");
        }

        if (!double.IsFinite(RelativeTolerance) ||
            RelativeTolerance <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(RelativeTolerance),
                "La tolérance relative doit être finie et strictement positive.");
        }

        if (!double.IsFinite(MinimumTimeStep) ||
            MinimumTimeStep <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MinimumTimeStep),
                "Le pas minimal doit être fini et strictement positif.");
        }

        if (!double.IsFinite(MaximumTimeStep) ||
            MaximumTimeStep <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaximumTimeStep),
                "Le pas maximal doit être fini et strictement positif.");
        }

        if (MinimumTimeStep > MaximumTimeStep)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MinimumTimeStep),
                "Le pas minimal ne peut pas être supérieur au pas maximal.");
        }

        this.AbsoluteTolerance = AbsoluteTolerance;
        this.RelativeTolerance = RelativeTolerance;
        this.MinimumTimeStep = MinimumTimeStep;
        this.MaximumTimeStep = MaximumTimeStep;
    }

    public SimulationState Step(
        SimulationState State,
        double TimeStep,
        IAccelerationModel AccelerationModel,
        SimulationParameters Parameters)
    {
        double CurrentTimeStep = TimeStep;

        while (true)
        {
            AdaptiveStepResult Result = StepAdaptive(
                State,
                CurrentTimeStep,
                AccelerationModel,
                Parameters);

            if (Result.IsAccepted)
            {
                return Result.State;
            }

            CurrentTimeStep = Result.SuggestedTimeStep;
        }
    }

    public AdaptiveStepResult StepAdaptive(
        SimulationState State,
        double TimeStep,
        IAccelerationModel AccelerationModel,
        SimulationParameters Parameters)
    {
        ArgumentNullException.ThrowIfNull(AccelerationModel);
        ArgumentNullException.ThrowIfNull(Parameters);

        if (!double.IsFinite(TimeStep) || TimeStep <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(TimeStep),
                "Le pas proposé doit être fini et strictement positif.");
        }

        double UsedTimeStep = Math.Clamp(
            TimeStep,
            MinimumTimeStep,
            MaximumTimeStep);

        StateDerivative K1 = Evaluate(
            State,
            AccelerationModel,
            Parameters);

        SimulationState K2State = CreateState(
            State,
            UsedTimeStep,
            1.0 / 5.0,
            (1.0 / 5.0) * K1.PositionDerivative,
            (1.0 / 5.0) * K1.VelocityDerivative);

        StateDerivative K2 = Evaluate(
            K2State,
            AccelerationModel,
            Parameters);

        SimulationState K3State = CreateState(
            State,
            UsedTimeStep,
            3.0 / 10.0,
            (3.0 / 40.0) * K1.PositionDerivative +
            (9.0 / 40.0) * K2.PositionDerivative,
            (3.0 / 40.0) * K1.VelocityDerivative +
            (9.0 / 40.0) * K2.VelocityDerivative);

        StateDerivative K3 = Evaluate(
            K3State,
            AccelerationModel,
            Parameters);

        SimulationState K4State = CreateState(
            State,
            UsedTimeStep,
            4.0 / 5.0,
            (44.0 / 45.0) * K1.PositionDerivative -
            (56.0 / 15.0) * K2.PositionDerivative +
            (32.0 / 9.0) * K3.PositionDerivative,
            (44.0 / 45.0) * K1.VelocityDerivative -
            (56.0 / 15.0) * K2.VelocityDerivative +
            (32.0 / 9.0) * K3.VelocityDerivative);

        StateDerivative K4 = Evaluate(
            K4State,
            AccelerationModel,
            Parameters);

        SimulationState K5State = CreateState(
            State,
            UsedTimeStep,
            8.0 / 9.0,
            (19372.0 / 6561.0) * K1.PositionDerivative -
            (25360.0 / 2187.0) * K2.PositionDerivative +
            (64448.0 / 6561.0) * K3.PositionDerivative -
            (212.0 / 729.0) * K4.PositionDerivative,
            (19372.0 / 6561.0) * K1.VelocityDerivative -
            (25360.0 / 2187.0) * K2.VelocityDerivative +
            (64448.0 / 6561.0) * K3.VelocityDerivative -
            (212.0 / 729.0) * K4.VelocityDerivative);

        StateDerivative K5 = Evaluate(
            K5State,
            AccelerationModel,
            Parameters);

        SimulationState K6State = CreateState(
            State,
            UsedTimeStep,
            1.0,
            (9017.0 / 3168.0) * K1.PositionDerivative -
            (355.0 / 33.0) * K2.PositionDerivative +
            (46732.0 / 5247.0) * K3.PositionDerivative +
            (49.0 / 176.0) * K4.PositionDerivative -
            (5103.0 / 18656.0) * K5.PositionDerivative,
            (9017.0 / 3168.0) * K1.VelocityDerivative -
            (355.0 / 33.0) * K2.VelocityDerivative +
            (46732.0 / 5247.0) * K3.VelocityDerivative +
            (49.0 / 176.0) * K4.VelocityDerivative -
            (5103.0 / 18656.0) * K5.VelocityDerivative);

        StateDerivative K6 = Evaluate(
            K6State,
            AccelerationModel,
            Parameters);

        SimulationState FifthOrderState = CreateState(
            State,
            UsedTimeStep,
            1.0,
            (35.0 / 384.0) * K1.PositionDerivative +
            (500.0 / 1113.0) * K3.PositionDerivative +
            (125.0 / 192.0) * K4.PositionDerivative -
            (2187.0 / 6784.0) * K5.PositionDerivative +
            (11.0 / 84.0) * K6.PositionDerivative,
            (35.0 / 384.0) * K1.VelocityDerivative +
            (500.0 / 1113.0) * K3.VelocityDerivative +
            (125.0 / 192.0) * K4.VelocityDerivative -
            (2187.0 / 6784.0) * K5.VelocityDerivative +
            (11.0 / 84.0) * K6.VelocityDerivative);

        StateDerivative K7 = Evaluate(
            FifthOrderState,
            AccelerationModel,
            Parameters);

        SimulationState FourthOrderState = CreateState(
            State,
            UsedTimeStep,
            1.0,
            (5179.0 / 57600.0) * K1.PositionDerivative +
            (7571.0 / 16695.0) * K3.PositionDerivative +
            (393.0 / 640.0) * K4.PositionDerivative -
            (92097.0 / 339200.0) * K5.PositionDerivative +
            (187.0 / 2100.0) * K6.PositionDerivative +
            (1.0 / 40.0) * K7.PositionDerivative,
            (5179.0 / 57600.0) * K1.VelocityDerivative +
            (7571.0 / 16695.0) * K3.VelocityDerivative +
            (393.0 / 640.0) * K4.VelocityDerivative -
            (92097.0 / 339200.0) * K5.VelocityDerivative +
            (187.0 / 2100.0) * K6.VelocityDerivative +
            (1.0 / 40.0) * K7.VelocityDerivative);

        double ErrorNorm = CalculateErrorNorm(
            State,
            FifthOrderState,
            FourthOrderState);

        if (!double.IsFinite(ErrorNorm))
        {
            throw new InvalidOperationException(
                "Le calcul RK45 a produit une erreur numérique non finie.");
        }

        bool IsAccepted = ErrorNorm <= 1.0;

        double SuggestedTimeStep =
            CalculateSuggestedTimeStep(
                UsedTimeStep,
                ErrorNorm);

        if (!IsAccepted &&
            UsedTimeStep <=
            MinimumTimeStep *
            (1.0 + MinimumStepComparisonTolerance))
        {
            throw new InvalidOperationException(
                "La tolérance demandée ne peut pas être respectée avec le pas minimal.");
        }

        return new AdaptiveStepResult(
            FifthOrderState,
            IsAccepted,
            UsedTimeStep,
            SuggestedTimeStep,
            ErrorNorm);
    }

    private static StateDerivative Evaluate(
        SimulationState State,
        IAccelerationModel AccelerationModel,
        SimulationParameters Parameters)
    {
        return new StateDerivative(
            State.Velocity,
            AccelerationModel.GetAcceleration(
                State,
                Parameters));
    }

    private static SimulationState CreateState(
        SimulationState InitialState,
        double TimeStep,
        double TimeCoefficient,
        Vector3D PositionDerivativeCombination,
        Vector3D VelocityDerivativeCombination)
    {
        return new SimulationState(
            InitialState.Time +
            TimeCoefficient * TimeStep,
            InitialState.Position +
            PositionDerivativeCombination * TimeStep,
            InitialState.Velocity +
            VelocityDerivativeCombination * TimeStep);
    }

    private double CalculateErrorNorm(
        SimulationState InitialState,
        SimulationState FifthOrderState,
        SimulationState FourthOrderState)
    {
        double ErrorSum = 0.0;

        ErrorSum += CalculateSquaredNormalizedError(
            InitialState.Position.X,
            FifthOrderState.Position.X,
            FourthOrderState.Position.X);

        ErrorSum += CalculateSquaredNormalizedError(
            InitialState.Position.Y,
            FifthOrderState.Position.Y,
            FourthOrderState.Position.Y);

        ErrorSum += CalculateSquaredNormalizedError(
            InitialState.Position.Z,
            FifthOrderState.Position.Z,
            FourthOrderState.Position.Z);

        ErrorSum += CalculateSquaredNormalizedError(
            InitialState.Velocity.X,
            FifthOrderState.Velocity.X,
            FourthOrderState.Velocity.X);

        ErrorSum += CalculateSquaredNormalizedError(
            InitialState.Velocity.Y,
            FifthOrderState.Velocity.Y,
            FourthOrderState.Velocity.Y);

        ErrorSum += CalculateSquaredNormalizedError(
            InitialState.Velocity.Z,
            FifthOrderState.Velocity.Z,
            FourthOrderState.Velocity.Z);

        return Math.Sqrt(ErrorSum / 6.0);
    }

    private double CalculateSquaredNormalizedError(
        double InitialValue,
        double FifthOrderValue,
        double FourthOrderValue)
    {
        double Scale =
            AbsoluteTolerance +
            RelativeTolerance *
            Math.Max(
                Math.Abs(InitialValue),
                Math.Abs(FifthOrderValue));

        double NormalizedError =
            (FifthOrderValue -
             FourthOrderValue) /
            Scale;

        return
            NormalizedError *
            NormalizedError;
    }

    private double CalculateSuggestedTimeStep(
        double UsedTimeStep,
        double ErrorNorm)
    {
        double ScaleFactor = ErrorNorm == 0.0
            ? MaximumScaleFactor
            : SafetyFactor *
              Math.Pow(
                  ErrorNorm,
                  -1.0 / 5.0);

        ScaleFactor = Math.Clamp(
            ScaleFactor,
            MinimumScaleFactor,
            MaximumScaleFactor);

        return Math.Clamp(
            UsedTimeStep * ScaleFactor,
            MinimumTimeStep,
            MaximumTimeStep);
    }
}
