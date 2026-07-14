using TrajectoryLab.Application.Models.Inputs;

namespace TrajectoryLab.Desktop.Services;

public interface IWindLayerTextParser
{
    IReadOnlyList<WindLayerInput> Parse(string Text);
}