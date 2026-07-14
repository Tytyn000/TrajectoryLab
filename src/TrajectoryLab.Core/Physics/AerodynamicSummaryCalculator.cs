namespace TrajectoryLab.Core;

public sealed class AerodynamicSummaryCalculator
{
    public AerodynamicSummary Calculate(
        IReadOnlyList<FlightCondition> Conditions)
    {
        if (Conditions is null)
        {
            throw new ArgumentNullException(
                nameof(Conditions),
                "La collection de conditions de vol ne peut pas être nulle.");
        }

        if (Conditions.Count == 0)
        {
            throw new ArgumentException(
                "La collection de conditions de vol doit contenir au moins un élément.",
                nameof(Conditions));
        }

        FlightCondition InitialCondition =
            GetCondition(
                Conditions,
                0);

        FlightCondition ImpactCondition =
            InitialCondition;

        FlightCondition MaximumMachCondition =
            InitialCondition;

        double MaximumRelativeSpeed =
            InitialCondition.RelativeSpeed;

        for (int Index = 1;
             Index < Conditions.Count;
             Index++)
        {
            FlightCondition Condition =
                GetCondition(
                    Conditions,
                    Index);

            ImpactCondition = Condition;

            if (Condition.MachNumber
                > MaximumMachCondition.MachNumber)
            {
                MaximumMachCondition = Condition;
            }

            if (Condition.RelativeSpeed
                > MaximumRelativeSpeed)
            {
                MaximumRelativeSpeed =
                    Condition.RelativeSpeed;
            }
        }

        return new AerodynamicSummary(
            InitialMachNumber:
                InitialCondition.MachNumber,
            MaximumMachNumber:
                MaximumMachCondition.MachNumber,
            ImpactMachNumber:
                ImpactCondition.MachNumber,
            InitialRelativeSpeed:
                InitialCondition.RelativeSpeed,
            MaximumRelativeSpeed:
                MaximumRelativeSpeed,
            ImpactRelativeSpeed:
                ImpactCondition.RelativeSpeed,
            MaximumMachTime:
                MaximumMachCondition.Time,
            MaximumMachAltitude:
                MaximumMachCondition.Position.Z);
    }

    private static FlightCondition GetCondition(
        IReadOnlyList<FlightCondition> Conditions,
        int Index)
    {
        FlightCondition? Condition =
            Conditions[Index];

        if (Condition is null)
        {
            throw new ArgumentException(
                $"La condition de vol à l'index {Index} ne peut pas être nulle.",
                nameof(Conditions));
        }

        return Condition;
    }
}