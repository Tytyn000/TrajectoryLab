namespace TrajectoryLab.Desktop.ViewModels;

public sealed class MagnusParametersViewModel
{
    public bool IsEnabled { get; set; }
    public string AngularVelocityX { get; set; } = "0";
    public string AngularVelocityY { get; set; } = "0";
    public string AngularVelocityZ { get; set; } = "0";
    public string MagnusCoefficient { get; set; } = "0";
}