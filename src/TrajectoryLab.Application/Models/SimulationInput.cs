using TrajectoryLab.Application.Models.Inputs;

namespace TrajectoryLab.Application.Models;

public sealed record SimulationInput
{
    public InitialConditionsInput InitialConditions { get; init; } =
        new();

    public ProjectileInput Projectile { get; init; } =
        new();

    public CelestialBodyInput CelestialBody { get; init; } =
        new();

    public AtmosphereInput Atmosphere { get; init; } =
        new();

    public GasInput Gas { get; init; } =
        new();

    public WindInput Wind { get; init; } =
        new();

    public DragInput Drag { get; init; } =
        new();

    public RotationInput Rotation { get; init; } =
        new();

    public MagnusInput Magnus { get; init; } =
        new();

    public SolverInput Solver { get; init; } =
        new();

    public SimulationLimitsInput Limits { get; init; } =
        new();
}
