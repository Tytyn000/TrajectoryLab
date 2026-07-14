namespace TrajectoryLab.Desktop.ViewModels;

public sealed class ProjectileParametersViewModel
{
    public string Mass { get; set; } = "1";
    public string CrossSectionalArea { get; set; } = "0.01";
    public string ReferenceDragCoefficient { get; set; } = "0.47";
    public string Radius { get; set; } = "0.05";
}