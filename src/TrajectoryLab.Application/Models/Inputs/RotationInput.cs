using TrajectoryLab.Core.Physics;

namespace TrajectoryLab.Application.Models.Inputs;

public sealed record RotationInput
{
    public bool IsCoriolisEnabled { get; init; }

    public CoriolisDefinitionKind DefinitionKind { get; init; } =
        CoriolisDefinitionKind.Latitude;

    public double LatitudeDegrees { get; init; } =
        48.8566;

    public double AngularVelocity { get; init; } =
        CoriolisAccelerationModel.EarthAngularVelocity;

    public double AngularVelocityX { get; init; }

    public double AngularVelocityY { get; init; }

    public double AngularVelocityZ { get; init; } =
        CoriolisAccelerationModel.EarthAngularVelocity;
}
