using System.Windows.Input;
using TrajectoryLab.Application.Models;
using TrajectoryLab.Application.Validation;
using TrajectoryLab.Desktop.Infrastructure;
using TrajectoryLab.Desktop.Services;

namespace TrajectoryLab.Desktop.ViewModels;

public sealed class MainWindowViewModel :
    ObservableObject
{
    private readonly ISimulationInputMapper SimulationInputMapper;
    private readonly ISimulationInputValidator SimulationInputValidator;

    private string StatusValue =
        "Modifie les paramètres puis lance leur validation.";

    public MainWindowViewModel(
        ISimulationInputMapper SimulationInputMapper,
        ISimulationInputValidator SimulationInputValidator)
    {
        ArgumentNullException.ThrowIfNull(
            SimulationInputMapper);
        ArgumentNullException.ThrowIfNull(
            SimulationInputValidator);

        this.SimulationInputMapper =
            SimulationInputMapper;

        this.SimulationInputValidator =
            SimulationInputValidator;

        ValidateCommand =
            new RelayCommand(ValidateParameters);
    }

    public SimulationParametersViewModel Parameters { get; } =
        new();

    public ICommand ValidateCommand { get; }

    public string Status
    {
        get => StatusValue;
        private set => SetProperty(
            ref StatusValue,
            value);
    }

    private void ValidateParameters()
    {
        try
        {
            SimulationInput Input =
                SimulationInputMapper.Map(Parameters);

            SimulationInputValidator.Validate(Input);

            Status =
                "Tous les paramètres sont valides. Aucun calcul de trajectoire n'est encore lancé.";
        }
        catch (SimulationInputValidationException Exception)
        {
            Status = string.Join(
                " | ",
                Exception.Errors);
        }
        catch (Exception Exception)
        {
            Status = Exception.Message;
        }
    }
}