using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Desktop.Parsing;

namespace TrajectoryLab.Desktop.Services;

public sealed class DragCoefficientPointTextParser :
    IDragCoefficientPointTextParser
{
    private readonly IDoubleTextParser DoubleTextParser;

    public DragCoefficientPointTextParser(
        IDoubleTextParser DoubleTextParser)
    {
        ArgumentNullException.ThrowIfNull(DoubleTextParser);
        this.DoubleTextParser = DoubleTextParser;
    }

    public IReadOnlyList<DragCoefficientPointInput> Parse(
        string Text)
    {
        List<DragCoefficientPointInput> Points = [];

        string[] Lines =
            Text.Split(
                ["\r\n", "\n", "\r"],
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

        for (int Index = 0; Index < Lines.Length; Index++)
        {
            string Line = Lines[Index];

            if (Line.StartsWith('#'))
            {
                continue;
            }

            string[] Values =
                Line.Split(
                    ';',
                    StringSplitOptions.TrimEntries);

            if (Values.Length != 2)
            {
                throw new FormatException(
                    $"Traînée tabulée, ligne {Index + 1} : format attendu Mach;Cd.");
            }

            Points.Add(
                new DragCoefficientPointInput
                {
                    MachNumber = DoubleTextParser.Parse(
                        Values[0],
                        $"Mach du point {Index + 1}"),
                    DragCoefficient = DoubleTextParser.Parse(
                        Values[1],
                        $"Coefficient du point {Index + 1}")
                });
        }

        return Points;
    }
}