namespace TrajectoryLab.Application.Models.Inputs;

public sealed record CelestialBodyInput
{
    public GravityModelKind ModelKind { get; init; } =
        GravityModelKind.UniformSphere;

    public UniformSphereDefinitionKind UniformSphereDefinition { get; init; } =
        UniformSphereDefinitionKind.SurfaceGravity;

    public double BodyRadius { get; init; } =
        6_371_000.0;

    public double SurfaceGravityAcceleration { get; init; } =
        9.80665;

    public double BodyDensity { get; init; } =
        5_513.258738589093;

    public double BodyMass { get; init; } =
        5.972168494074286e24;
}
