namespace TrajectoryLab.Desktop.ViewModels;

public sealed class InitialConditionsParametersViewModel
{
    public string InitialX { get; set; } = "0";
    public string InitialY { get; set; } = "0";
    public string InitialZ { get; set; } = "0";
    public string InitialSpeed { get; set; } = "50";
    public string ElevationDegrees { get; set; } = "45";
    public string AzimuthDegrees { get; set; } = "0";
}