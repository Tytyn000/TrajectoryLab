namespace TrajectoryLab.Application.Models.Inputs;

public sealed record DragInput
{
    public bool IsEnabled { get; init; }

    public DragCoefficientModelKind ModelKind { get; init; } =
        DragCoefficientModelKind.Constant;

    public IReadOnlyList<DragCoefficientPointInput> Points { get; init; } =
        Array.Empty<DragCoefficientPointInput>();
}