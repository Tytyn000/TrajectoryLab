namespace TrajectoryLab.Core.Simulation;

public enum TrajectoryQuantity
{
    Time,
    PositionX,
    PositionY,
    Altitude,
    HorizontalDistance,
    Speed,
    RelativeSpeed,
    MachNumber,
    Temperature
}

public sealed record TrajectoryQuantityInfo(
    TrajectoryQuantity Quantity,
    string Label,
    string Unit)
{
    public string AxisLabel =>
        string.IsNullOrWhiteSpace(Unit)
            ? Label
            : $"{Label} ({Unit})";

    public override string ToString()
    {
        return AxisLabel;
    }
}

public static class TrajectoryQuantityCatalog
{
    private static readonly
        IReadOnlyList<TrajectoryQuantityInfo>
        Quantities =
            Array.AsReadOnly(
                new[]
                {
                    new TrajectoryQuantityInfo(
                        TrajectoryQuantity.Time,
                        "Temps",
                        "s"),

                    new TrajectoryQuantityInfo(
                        TrajectoryQuantity.PositionX,
                        "Position X — Est",
                        "m"),

                    new TrajectoryQuantityInfo(
                        TrajectoryQuantity.PositionY,
                        "Position Y — Nord",
                        "m"),

                    new TrajectoryQuantityInfo(
                        TrajectoryQuantity.Altitude,
                        "Position Z — Altitude",
                        "m"),

                    new TrajectoryQuantityInfo(
                        TrajectoryQuantity.HorizontalDistance,
                        "Distance horizontale",
                        "m"),

                    new TrajectoryQuantityInfo(
                        TrajectoryQuantity.Speed,
                        "Vitesse",
                        "m/s"),

                    new TrajectoryQuantityInfo(
                        TrajectoryQuantity.RelativeSpeed,
                        "Vitesse relative à l'air",
                        "m/s"),

                    new TrajectoryQuantityInfo(
                        TrajectoryQuantity.MachNumber,
                        "Nombre de Mach",
                        string.Empty),

                    new TrajectoryQuantityInfo(
                        TrajectoryQuantity.Temperature,
                        "Température",
                        "K")
                });

    public static IReadOnlyList<TrajectoryQuantityInfo>
        All =>
            Quantities;

    public static TrajectoryQuantityInfo Get(
        TrajectoryQuantity Quantity)
    {
        foreach (
            TrajectoryQuantityInfo Information
            in Quantities)
        {
            if (Information.Quantity == Quantity)
            {
                return Information;
            }
        }

        throw new ArgumentOutOfRangeException(
            nameof(Quantity),
            "La grandeur de trajectoire est inconnue.");
    }
}

public sealed record TrajectorySeries(
    double[] HorizontalValues,
    double[] VerticalValues,
    TrajectoryQuantityInfo HorizontalQuantity,
    TrajectoryQuantityInfo VerticalQuantity);
