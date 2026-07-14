using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core;
using TrajectoryLab.Core.Atmosphere;

namespace TrajectoryLab.Application.Simulation.Factories;

public sealed class AtmosphereModelFactory :
    IAtmosphereModelFactory
{
    public IAtmosphereModel Create(
        AtmosphereInput Input,
        IdealGasModel GasModel)
    {
        ArgumentNullException.ThrowIfNull(Input);
        ArgumentNullException.ThrowIfNull(GasModel);

        return Input.ModelKind switch
        {
            AtmosphereModelKind.Vacuum =>
                new ConstantAtmosphereModel(
                    AirDensity: 0.0,
                    Temperature: Input.ConstantTemperature),

            AtmosphereModelKind.Constant =>
                new ConstantAtmosphereModel(
                    AirDensity:
                        GetConstantAirDensity(
                            Input,
                            GasModel),
                    Temperature:
                        Input.ConstantTemperature),

            AtmosphereModelKind.Standard1976 =>
                new StandardAtmosphere1976Model(),

            _ => throw new ArgumentOutOfRangeException(
                nameof(Input),
                Input.ModelKind,
                "Le modÃ¨le atmosphÃ©rique sÃ©lectionnÃ© est inconnu.")
        };
    }

    private static double GetConstantAirDensity(
        AtmosphereInput Input,
        IdealGasModel GasModel)
    {
        return Input.ConstantDefinition switch
        {
            ConstantAtmosphereDefinitionKind.DensityAndTemperature =>
                Input.ConstantAirDensity,

            ConstantAtmosphereDefinitionKind.PressureAndTemperature =>
                Input.ConstantPressure
                / (
                    GasModel.SpecificGasConstant
                    * Input.ConstantTemperature
                ),

            _ => throw new ArgumentOutOfRangeException(
                nameof(Input),
                Input.ConstantDefinition,
                "La dÃ©finition de l'atmosphÃ¨re constante est inconnue.")
        };
    }
}
