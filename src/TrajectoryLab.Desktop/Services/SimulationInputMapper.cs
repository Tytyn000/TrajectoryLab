using TrajectoryLab.Application.Models;
using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Desktop.Parsing;
using TrajectoryLab.Desktop.ViewModels;

namespace TrajectoryLab.Desktop.Services;

public sealed class SimulationInputMapper :
    ISimulationInputMapper
{
    private readonly IDoubleTextParser DoubleTextParser;
    private readonly IWindLayerTextParser WindLayerTextParser;
    private readonly IDragCoefficientPointTextParser
        DragCoefficientPointTextParser;

    public SimulationInputMapper(
        IDoubleTextParser DoubleTextParser,
        IWindLayerTextParser WindLayerTextParser,
        IDragCoefficientPointTextParser
            DragCoefficientPointTextParser)
    {
        ArgumentNullException.ThrowIfNull(DoubleTextParser);
        ArgumentNullException.ThrowIfNull(WindLayerTextParser);
        ArgumentNullException.ThrowIfNull(
            DragCoefficientPointTextParser);

        this.DoubleTextParser = DoubleTextParser;
        this.WindLayerTextParser = WindLayerTextParser;
        this.DragCoefficientPointTextParser =
            DragCoefficientPointTextParser;
    }

    public SimulationInput Map(
        SimulationParametersViewModel Source)
    {
        ArgumentNullException.ThrowIfNull(Source);

        return new SimulationInput
        {
            InitialConditions = MapInitialConditions(
                Source.InitialConditions),
            Projectile = MapProjectile(Source.Projectile),
            CelestialBody = MapCelestialBody(
                Source.CelestialBody),
            Atmosphere = MapAtmosphere(Source.Atmosphere),
            Gas = MapGas(Source.Gas),
            Wind = MapWind(Source.Wind),
            Drag = MapDrag(Source.Drag),
            Rotation = MapRotation(Source.Rotation),
            Magnus = MapMagnus(Source.Magnus),
            Solver = MapSolver(Source.Solver),
            Limits = MapLimits(Source.Limits)
        };
    }

    private InitialConditionsInput MapInitialConditions(
        InitialConditionsParametersViewModel Source)
    {
        return new InitialConditionsInput
        {
            InitialX = Parse(Source.InitialX, "Position initiale X"),
            InitialY = Parse(Source.InitialY, "Position initiale Y"),
            InitialZ = Parse(Source.InitialZ, "Position initiale Z"),
            InitialSpeed = Parse(
                Source.InitialSpeed,
                "Vitesse initiale"),
            ElevationDegrees = Parse(
                Source.ElevationDegrees,
                "Élévation"),
            AzimuthDegrees = Parse(
                Source.AzimuthDegrees,
                "Azimut")
        };
    }

    private ProjectileInput MapProjectile(
        ProjectileParametersViewModel Source)
    {
        return new ProjectileInput
        {
            Mass = Parse(Source.Mass, "Masse du projectile"),
            CrossSectionalArea = Parse(
                Source.CrossSectionalArea,
                "Surface frontale"),
            ReferenceDragCoefficient = Parse(
                Source.ReferenceDragCoefficient,
                "Coefficient de traînée de référence"),
            Radius = Parse(Source.Radius, "Rayon du projectile")
        };
    }

    private CelestialBodyInput MapCelestialBody(
        CelestialBodyParametersViewModel Source)
    {
        CelestialBodyInput Defaults = new();

        double BodyRadius = Defaults.BodyRadius;
        double SurfaceGravityAcceleration =
            Defaults.SurfaceGravityAcceleration;
        double BodyDensity = Defaults.BodyDensity;
        double BodyMass = Defaults.BodyMass;

        if (Source.SelectedGravityModel ==
            GravityModelKind.Constant)
        {
            SurfaceGravityAcceleration = Parse(
                Source.SurfaceGravityAcceleration,
                "Gravité locale constante");
        }
        else
        {
            BodyRadius = Parse(
                Source.BodyRadius,
                "Rayon de l'astre");

            switch (Source.SelectedUniformSphereDefinition)
            {
                case UniformSphereDefinitionKind.SurfaceGravity:
                    SurfaceGravityAcceleration = Parse(
                        Source.SurfaceGravityAcceleration,
                        "Gravité de surface");
                    break;

                case UniformSphereDefinitionKind.Density:
                    BodyDensity = Parse(
                        Source.BodyDensity,
                        "Masse volumique de l'astre");
                    break;

                case UniformSphereDefinitionKind.Mass:
                    BodyMass = Parse(
                        Source.BodyMass,
                        "Masse totale de l'astre");
                    break;
            }
        }

        return new CelestialBodyInput
        {
            ModelKind = Source.SelectedGravityModel,
            UniformSphereDefinition =
                Source.SelectedUniformSphereDefinition,
            BodyRadius = BodyRadius,
            SurfaceGravityAcceleration =
                SurfaceGravityAcceleration,
            BodyDensity = BodyDensity,
            BodyMass = BodyMass
        };
    }

    private AtmosphereInput MapAtmosphere(
        AtmosphereParametersViewModel Source)
    {
        AtmosphereInput Defaults = new();

        double ConstantAirDensity =
            Defaults.ConstantAirDensity;
        double ConstantPressure =
            Defaults.ConstantPressure;

        double ConstantTemperature = Parse(
            Source.ConstantTemperature,
            "Température atmosphérique");

        if (Source.SelectedAtmosphereModel ==
            AtmosphereModelKind.Constant)
        {
            if (Source.SelectedConstantAtmosphereDefinition ==
                ConstantAtmosphereDefinitionKind
                    .DensityAndTemperature)
            {
                ConstantAirDensity = Parse(
                    Source.ConstantAirDensity,
                    "Densité atmosphérique");
            }
            else
            {
                ConstantPressure = Parse(
                    Source.ConstantPressure,
                    "Pression atmosphérique");
            }
        }

        return new AtmosphereInput
        {
            ModelKind = Source.SelectedAtmosphereModel,
            ConstantDefinition =
                Source.SelectedConstantAtmosphereDefinition,
            ConstantAirDensity = ConstantAirDensity,
            ConstantPressure = ConstantPressure,
            ConstantTemperature = ConstantTemperature
        };
    }

    private GasInput MapGas(
        GasParametersViewModel Source)
    {
        return new GasInput
        {
            SpecificHeatRatio = Parse(
                Source.SpecificHeatRatio,
                "Rapport des capacités thermiques"),
            SpecificGasConstant = Parse(
                Source.SpecificGasConstant,
                "Constante spécifique du gaz")
        };
    }

    private WindInput MapWind(
        WindParametersViewModel Source)
    {
        WindInput Defaults = new();

        double ConstantWindX = Defaults.ConstantWindX;
        double ConstantWindY = Defaults.ConstantWindY;
        double ConstantWindZ = Defaults.ConstantWindZ;

        double LowerAltitude = Defaults.LowerAltitude;
        double LowerWindX = Defaults.LowerWindX;
        double LowerWindY = Defaults.LowerWindY;
        double LowerWindZ = Defaults.LowerWindZ;

        double UpperAltitude = Defaults.UpperAltitude;
        double UpperWindX = Defaults.UpperWindX;
        double UpperWindY = Defaults.UpperWindY;
        double UpperWindZ = Defaults.UpperWindZ;

        IReadOnlyList<WindLayerInput> Layers =
            Array.Empty<WindLayerInput>();

        switch (Source.SelectedWindModel)
        {
            case WindModelKind.Constant:
                ConstantWindX = Parse(
                    Source.ConstantWindX,
                    "Vent constant X");
                ConstantWindY = Parse(
                    Source.ConstantWindY,
                    "Vent constant Y");
                ConstantWindZ = Parse(
                    Source.ConstantWindZ,
                    "Vent constant Z");
                break;

            case WindModelKind.Linear:
                LowerAltitude = Parse(
                    Source.LowerAltitude,
                    "Altitude basse du vent");
                LowerWindX = Parse(
                    Source.LowerWindX,
                    "Vent bas X");
                LowerWindY = Parse(
                    Source.LowerWindY,
                    "Vent bas Y");
                LowerWindZ = Parse(
                    Source.LowerWindZ,
                    "Vent bas Z");
                UpperAltitude = Parse(
                    Source.UpperAltitude,
                    "Altitude haute du vent");
                UpperWindX = Parse(
                    Source.UpperWindX,
                    "Vent haut X");
                UpperWindY = Parse(
                    Source.UpperWindY,
                    "Vent haut Y");
                UpperWindZ = Parse(
                    Source.UpperWindZ,
                    "Vent haut Z");
                break;

            case WindModelKind.Layered:
                Layers = WindLayerTextParser.Parse(
                    Source.LayeredWindProfile);
                break;
        }

        return new WindInput
        {
            ModelKind = Source.SelectedWindModel,
            ConstantWindX = ConstantWindX,
            ConstantWindY = ConstantWindY,
            ConstantWindZ = ConstantWindZ,
            LowerAltitude = LowerAltitude,
            LowerWindX = LowerWindX,
            LowerWindY = LowerWindY,
            LowerWindZ = LowerWindZ,
            UpperAltitude = UpperAltitude,
            UpperWindX = UpperWindX,
            UpperWindY = UpperWindY,
            UpperWindZ = UpperWindZ,
            Layers = Layers
        };
    }

    private DragInput MapDrag(
        DragParametersViewModel Source)
    {
        IReadOnlyList<DragCoefficientPointInput> Points =
            Array.Empty<DragCoefficientPointInput>();

        if (Source.IsEnabled &&
            Source.SelectedDragCoefficientModel ==
                DragCoefficientModelKind.Tabulated)
        {
            Points = DragCoefficientPointTextParser.Parse(
                Source.TabulatedDragProfile);
        }

        return new DragInput
        {
            IsEnabled = Source.IsEnabled,
            ModelKind =
                Source.SelectedDragCoefficientModel,
            Points = Points
        };
    }

    private RotationInput MapRotation(
        RotationParametersViewModel Source)
    {
        RotationInput Defaults = new();

        double LatitudeDegrees = Defaults.LatitudeDegrees;
        double AngularVelocity = Defaults.AngularVelocity;
        double AngularVelocityX = Defaults.AngularVelocityX;
        double AngularVelocityY = Defaults.AngularVelocityY;
        double AngularVelocityZ = Defaults.AngularVelocityZ;

        if (Source.IsCoriolisEnabled)
        {
            if (Source.SelectedCoriolisDefinition ==
                CoriolisDefinitionKind.Latitude)
            {
                LatitudeDegrees = Parse(
                    Source.LatitudeDegrees,
                    "Latitude");
                AngularVelocity = Parse(
                    Source.AngularVelocity,
                    "Vitesse angulaire de l'astre");
            }
            else
            {
                AngularVelocityX = Parse(
                    Source.AngularVelocityX,
                    "Rotation de l'astre X");
                AngularVelocityY = Parse(
                    Source.AngularVelocityY,
                    "Rotation de l'astre Y");
                AngularVelocityZ = Parse(
                    Source.AngularVelocityZ,
                    "Rotation de l'astre Z");
            }
        }

        return new RotationInput
        {
            IsCoriolisEnabled = Source.IsCoriolisEnabled,
            DefinitionKind =
                Source.SelectedCoriolisDefinition,
            LatitudeDegrees = LatitudeDegrees,
            AngularVelocity = AngularVelocity,
            AngularVelocityX = AngularVelocityX,
            AngularVelocityY = AngularVelocityY,
            AngularVelocityZ = AngularVelocityZ
        };
    }

    private MagnusInput MapMagnus(
        MagnusParametersViewModel Source)
    {
        MagnusInput Defaults = new();

        if (!Source.IsEnabled)
        {
            return Defaults;
        }

        return new MagnusInput
        {
            IsEnabled = true,
            AngularVelocityX = Parse(
                Source.AngularVelocityX,
                "Rotation du projectile X"),
            AngularVelocityY = Parse(
                Source.AngularVelocityY,
                "Rotation du projectile Y"),
            AngularVelocityZ = Parse(
                Source.AngularVelocityZ,
                "Rotation du projectile Z"),
            MagnusCoefficient = Parse(
                Source.MagnusCoefficient,
                "Coefficient de Magnus")
        };
    }

    private SolverInput MapSolver(
        SolverParametersViewModel Source)
    {
        SolverInput Defaults = new();

        double AbsoluteTolerance = Defaults.AbsoluteTolerance;
        double RelativeTolerance = Defaults.RelativeTolerance;
        double MinimumTimeStep = Defaults.MinimumTimeStep;
        double MaximumTimeStep = Defaults.MaximumTimeStep;

        if (Source.SelectedSolver == SolverKind.RungeKutta45)
        {
            AbsoluteTolerance = Parse(
                Source.AbsoluteTolerance,
                "Tolérance absolue");
            RelativeTolerance = Parse(
                Source.RelativeTolerance,
                "Tolérance relative");
            MinimumTimeStep = Parse(
                Source.MinimumTimeStep,
                "Pas minimal");
            MaximumTimeStep = Parse(
                Source.MaximumTimeStep,
                "Pas maximal");
        }

        return new SolverInput
        {
            ModelKind = Source.SelectedSolver,
            TimeStep = Parse(
                Source.TimeStep,
                "Pas de temps"),
            AbsoluteTolerance = AbsoluteTolerance,
            RelativeTolerance = RelativeTolerance,
            MinimumTimeStep = MinimumTimeStep,
            MaximumTimeStep = MaximumTimeStep
        };
    }

    private SimulationLimitsInput MapLimits(
        SimulationLimitsParametersViewModel Source)
    {
        return new SimulationLimitsInput
        {
            MaximumSimulationTime = Parse(
                Source.MaximumSimulationTime,
                "Durée maximale"),
            GroundAltitude = Parse(
                Source.GroundAltitude,
                "Altitude du sol")
        };
    }

    private double Parse(
        string Value,
        string FieldName)
    {
        return DoubleTextParser.Parse(
            Value,
            FieldName);
    }
}