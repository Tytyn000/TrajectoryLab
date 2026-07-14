using TrajectoryLab.Application.Models.Inputs;

namespace TrajectoryLab.Desktop.Services;

public interface IDragCoefficientPointTextParser
{
    IReadOnlyList<DragCoefficientPointInput> Parse(string Text);
}