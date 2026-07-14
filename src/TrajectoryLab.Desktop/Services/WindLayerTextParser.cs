using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Desktop.Parsing;

namespace TrajectoryLab.Desktop.Services;

public sealed class WindLayerTextParser :
    IWindLayerTextParser
{
    private readonly IDoubleTextParser DoubleTextParser;

    public WindLayerTextParser(
        IDoubleTextParser DoubleTextParser)
    {
        ArgumentNullException.ThrowIfNull(DoubleTextParser);
        this.DoubleTextParser = DoubleTextParser;
    }

    public IReadOnlyList<WindLayerInput> Parse(string Text)
    {
        List<WindLayerInput> Layers = [];

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

            if (Values.Length != 4)
            {
                throw new FormatException(
                    $"Vent par couches, ligne {Index + 1} : format attendu altitude;X;Y;Z.");
            }

            Layers.Add(
                new WindLayerInput
                {
                    Altitude = DoubleTextParser.Parse(
                        Values[0],
                        $"Altitude de la couche {Index + 1}"),
                    WindX = DoubleTextParser.Parse(
                        Values[1],
                        $"Vent X de la couche {Index + 1}"),
                    WindY = DoubleTextParser.Parse(
                        Values[2],
                        $"Vent Y de la couche {Index + 1}"),
                    WindZ = DoubleTextParser.Parse(
                        Values[3],
                        $"Vent Z de la couche {Index + 1}")
                });
        }

        return Layers;
    }
}