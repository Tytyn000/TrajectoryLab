using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core;

public sealed class FlightCondition
{
    public double Time { get; }

    public Vector3D Position { get; }

    public Vector3D WindVelocity { get; }

    public Vector3D RelativeVelocity { get; }

    public double RelativeSpeed { get; }

    public double Temperature { get; }

    public double SpeedOfSound { get; }

    public double MachNumber { get; }

    internal FlightCondition(
        double Time,
        Vector3D Position,
        Vector3D WindVelocity,
        Vector3D RelativeVelocity,
        double RelativeSpeed,
        double Temperature,
        double SpeedOfSound,
        double MachNumber)
    {
        this.Time = Time;
        this.Position = Position;
        this.WindVelocity = WindVelocity;
        this.RelativeVelocity = RelativeVelocity;
        this.RelativeSpeed = RelativeSpeed;
        this.Temperature = Temperature;
        this.SpeedOfSound = SpeedOfSound;
        this.MachNumber = MachNumber;
    }
}