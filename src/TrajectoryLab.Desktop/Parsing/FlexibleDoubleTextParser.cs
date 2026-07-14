using System.Globalization;

namespace TrajectoryLab.Desktop.Parsing;

public sealed class FlexibleDoubleTextParser :
    IDoubleTextParser
{
    public double Parse(
        string Value,
        string FieldName)
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new FormatException(
                $"{FieldName} : une valeur est requise.");
        }

        const NumberStyles Styles =
            NumberStyles.Float;

        if (double.TryParse(
                Value,
                Styles,
                CultureInfo.CurrentCulture,
                out double ParsedValue)
            || double.TryParse(
                Value,
                Styles,
                CultureInfo.InvariantCulture,
                out ParsedValue))
        {
            return ParsedValue;
        }

        string NormalizedValue =
            Value.Trim().Replace(',', '.');

        if (double.TryParse(
                NormalizedValue,
                Styles,
                CultureInfo.InvariantCulture,
                out ParsedValue))
        {
            return ParsedValue;
        }

        throw new FormatException(
            $"{FieldName} : '{Value}' n'est pas un nombre valide.");
    }
}