namespace TrajectoryLab.Desktop.Parsing;

public interface IDoubleTextParser
{
    double Parse(
        string Value,
        string FieldName);
}