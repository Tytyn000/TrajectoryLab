using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Physics;

/// Calcule l'accélération produite par la traînée aérodynamique quadratique.
public sealed class DragAccelerationModel
    : IAccelerationModel
{
    private const double MinimumRelativeSpeed =
        1.0e-12;

    private readonly MachNumberCalculator
        _MachNumberCalculator;

    public IDragCoefficientModel?
        DragCoefficientModel
    { get; }

    public IdealGasModel GasModel { get; }

    // Conserve le comportement historique :
    // le coefficient est lu dans ProjectileParameters.
    public DragAccelerationModel()
    {
        GasModel =
            new IdealGasModel();

        _MachNumberCalculator =
            new MachNumberCalculator(
                GasModel);
    }

    public DragAccelerationModel(
        IDragCoefficientModel DragCoefficientModel)
        : this(
            DragCoefficientModel,
            new IdealGasModel())
    {
    }

    public DragAccelerationModel(
        IDragCoefficientModel DragCoefficientModel,
        IdealGasModel GasModel)
    {
        if (DragCoefficientModel is null)
        {
            throw new ArgumentNullException(
                nameof(DragCoefficientModel),
                "Le modèle de coefficient de traînée ne peut pas être nul.");
        }

        if (GasModel is null)
        {
            throw new ArgumentNullException(
                nameof(GasModel),
                "Le modèle de gaz parfait ne peut pas être nul.");
        }

        this.DragCoefficientModel =
            DragCoefficientModel;

        this.GasModel =
            GasModel;

        _MachNumberCalculator =
            new MachNumberCalculator(
                GasModel);
    }

    public Vector3D GetAcceleration(
        SimulationState State,
        SimulationParameters Parameters)
    {
        Vector3D WindVelocity =
            Parameters.Environment.WindModel
                .GetWindVelocity(
                    State.Position,
                    State.Time);

        Vector3D RelativeVelocity =
            State.Velocity
            - WindVelocity;

        double RelativeSpeed =
            RelativeVelocity.Length();

        if (RelativeSpeed
            <= MinimumRelativeSpeed)
        {
            return Vector3D.Zero;
        }

        double AirDensity =
            Parameters.Environment.AtmosphereModel
                .GetAirDensity(
                    State.Position.Z);

        double DragCoefficient =
            GetDragCoefficient(
                State,
                Parameters,
                RelativeSpeed);

        double Factor =
            -0.5
            * AirDensity
            * DragCoefficient
            * Parameters.Projectile
                .CrossSectionalArea
            / Parameters.Projectile.Mass;

        return RelativeVelocity
            * Factor
            * RelativeSpeed;
    }

    private double GetDragCoefficient(
        SimulationState State,
        SimulationParameters Parameters,
        double RelativeSpeed)
    {
        // Sans modèle explicite, le comportement historique
        // utilisant ProjectileParameters est conservé.
        if (DragCoefficientModel is null)
        {
            return Parameters.Projectile
                .DragCoefficient;
        }

        double Temperature =
            Parameters.Environment.AtmosphereModel
                .GetTemperature(
                    State.Position.Z);

        double MachNumber =
            _MachNumberCalculator.GetMachNumber(
                RelativeSpeed,
                Temperature);

        double DragCoefficient =
            DragCoefficientModel
                .GetDragCoefficient(
                    MachNumber);

        if (!double.IsFinite(DragCoefficient)
            || DragCoefficient < 0.0)
        {
            throw new InvalidOperationException(
                "Le modèle de coefficient de traînée doit retourner une valeur finie et positive ou nulle.");
        }

        return DragCoefficient;
    }
}