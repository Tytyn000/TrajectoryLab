namespace TrajectoryLab.Application.Validation;

public sealed class SimulationInputValidationException :
    ArgumentException
{
    public IReadOnlyList<string> Errors { get; }

    public SimulationInputValidationException(
        IReadOnlyList<string> Errors)
        : base(CreateMessage(Errors))
    {
        ArgumentNullException.ThrowIfNull(
            Errors);

        this.Errors =
            Errors.ToArray();
    }

    private static string CreateMessage(
        IReadOnlyList<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(
            Errors);

        return Errors.Count == 0
            ? "Les param\u00e8tres de simulation sont invalides."
            : string.Join(
                Environment.NewLine,
                Errors);
    }
}