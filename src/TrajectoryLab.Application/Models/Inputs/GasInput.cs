namespace TrajectoryLab.Application.Models.Inputs;

public sealed record GasInput
{
    public double SpecificHeatRatio { get; init; } =
        1.4;

    public double SpecificGasConstant { get; init; } =
        287.05287;
}
