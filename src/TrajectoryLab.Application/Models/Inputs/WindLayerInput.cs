namespace TrajectoryLab.Application.Models.Inputs;

public sealed record WindLayerInput
{
    public double Altitude { get; init; }

    public double WindX { get; init; }

    public double WindY { get; init; }

    public double WindZ { get; init; }
}