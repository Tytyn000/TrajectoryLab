using TrajectoryLab.Core.Atmosphere;
using TrajectoryLab.Core.Mathematics;
using TrajectoryLab.Core.Wind;

namespace TrajectoryLab.Core;

public sealed class EnvironmentParameters
{
    public double GravityAcceleration { get; }

    public IAtmosphereModel AtmosphereModel { get; }

    public IWindModel WindModel { get; }

    public EnvironmentParameters(
        double GravityAcceleration,
        IAtmosphereModel AtmosphereModel,
        IWindModel WindModel)
    {
        // La valeur représente la norme positive
        // de l'accélération gravitationnelle.
        if (!double.IsFinite(GravityAcceleration) ||
            GravityAcceleration <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(GravityAcceleration),
                "L'accélération gravitationnelle doit être finie et strictement positive."
            );
        }

        ArgumentNullException.ThrowIfNull(
            AtmosphereModel
        );

        ArgumentNullException.ThrowIfNull(
            WindModel
        );

        this.GravityAcceleration =
            GravityAcceleration;

        this.AtmosphereModel =
            AtmosphereModel;

        this.WindModel =
            WindModel;
    }

    // Ce constructeur conserve la compatibilité
    // avec les scénarios utilisant directement un vecteur constant.
    public EnvironmentParameters(
        double GravityAcceleration,
        IAtmosphereModel AtmosphereModel,
        Vector3D WindVelocity)
        : this(
            GravityAcceleration,
            AtmosphereModel,
            new ConstantWindModel(
                WindVelocity
            )
        )
    {
    }
}