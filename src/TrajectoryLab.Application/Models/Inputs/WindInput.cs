namespace TrajectoryLab.Application.Models.Inputs;

public sealed record WindInput
{
    public WindModelKind ModelKind { get; init; } =
        WindModelKind.None;

    public double ConstantWindX { get; init; }

    public double ConstantWindY { get; init; }

    public double ConstantWindZ { get; init; }

    public double LowerAltitude { get; init; } = 0.0;

    public double LowerWindX { get; init; }

    public double LowerWindY { get; init; }

    public double LowerWindZ { get; init; }

    public double UpperAltitude { get; init; } = 1_000.0;

    public double UpperWindX { get; init; }

    public double UpperWindY { get; init; }

    public double UpperWindZ { get; init; }

    public IReadOnlyList<WindLayerInput> Layers { get; init; } =
        Array.Empty<WindLayerInput>();
}