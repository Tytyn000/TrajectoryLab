using TrajectoryLab.Core.Mathematics;

namespace TrajectoryLab.Core.Models;

public sealed record SimulationResult
{
    /// Ensemble des états successifs calculés pendant la simulation.
    public required IReadOnlyList<SimulationState> States { get; init; }

    /// État interpolé au moment de l'impact avec le sol.
    public required SimulationState ImpactState { get; init; }

    public double FlightTime => ImpactState.Time; //Temps de vol

    /// Distance horizontale entre le point de départ et l'impact.
    public double Range
    {
        get
        {
            Vector3D Start = States[0].Position;
            Vector3D End = ImpactState.Position;

            double DeltaX = End.X - Start.X;
            double DeltaY = End.Y - Start.Y;

            return Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);
        }
    }

    public double MaximumAltitude => States.Max(State => State.Position.Z); //Altitude max pendant la simulation

    public double ImpactSpeed => ImpactState.Velocity.Length(); // Vitesse au moment de l'impact
}