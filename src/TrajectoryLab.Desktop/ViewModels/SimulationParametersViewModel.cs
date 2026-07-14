namespace TrajectoryLab.Desktop.ViewModels;

public sealed class SimulationParametersViewModel
{
    public InitialConditionsParametersViewModel InitialConditions { get; } =
        new();

    public ProjectileParametersViewModel Projectile { get; } =
        new();

    public CelestialBodyParametersViewModel CelestialBody { get; } =
        new();

    public AtmosphereParametersViewModel Atmosphere { get; } =
        new();

    public GasParametersViewModel Gas { get; } =
        new();

    public WindParametersViewModel Wind { get; } =
        new();

    public DragParametersViewModel Drag { get; } =
        new();

    public RotationParametersViewModel Rotation { get; } =
        new();

    public MagnusParametersViewModel Magnus { get; } =
        new();

    public SolverParametersViewModel Solver { get; } =
        new();

    public SimulationLimitsParametersViewModel Limits { get; } =
        new();
}