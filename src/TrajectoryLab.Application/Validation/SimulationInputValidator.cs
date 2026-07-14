using TrajectoryLab.Application.Models;
using TrajectoryLab.Application.Models.Inputs;
using TrajectoryLab.Core.Atmosphere;

namespace TrajectoryLab.Application.Validation;

public sealed class SimulationInputValidator :
    ISimulationInputValidator
{
    public void Validate(SimulationInput Input)
    {
        ArgumentNullException.ThrowIfNull(Input);

        List<string> Errors = [];

        ValidateInitialConditions(Input.InitialConditions, Errors);
        ValidateProjectile(Input.Projectile, Input.Magnus, Errors);
        ValidateCelestialBody(Input.CelestialBody, Errors);
        ValidateAtmosphere(
            Input.Atmosphere,
            Input.InitialConditions,
            Input.Limits,
            Errors);
        ValidateGas(Input.Gas, Errors);
        ValidateWind(Input.Wind, Errors);
        ValidateDrag(Input.Drag, Errors);
        ValidateRotation(Input.Rotation, Errors);
        ValidateMagnus(Input.Magnus, Errors);
        ValidateSolver(Input.Solver, Input.Limits, Errors);
        ValidateLimits(
            Input.InitialConditions,
            Input.CelestialBody,
            Input.Limits,
            Errors);

        if (Errors.Count > 0)
        {
            throw new SimulationInputValidationException(Errors);
        }
    }

    private static void ValidateInitialConditions(
        InitialConditionsInput Input,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        RequireFinite(Input.InitialX, "La position X initiale", Errors);
        RequireFinite(Input.InitialY, "La position Y initiale", Errors);
        RequireFinite(Input.InitialZ, "La position Z initiale", Errors);
        RequireFinite(Input.InitialSpeed, "La vitesse initiale", Errors);
        RequireFinite(Input.ElevationDegrees, "L'Ã©lÃ©vation", Errors);
        RequireFinite(Input.AzimuthDegrees, "L'azimut", Errors);

        if (Input.InitialSpeed < 0.0)
        {
            Errors.Add("La vitesse initiale doit Ãªtre positive ou nulle.");
        }

        if (Input.ElevationDegrees < -90.0 ||
            Input.ElevationDegrees > 90.0)
        {
            Errors.Add(
                "L'Ã©lÃ©vation doit Ãªtre comprise entre -90Â° et 90Â°.");
        }
    }

    private static void ValidateProjectile(
        ProjectileInput Input,
        MagnusInput Magnus,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);
        ArgumentNullException.ThrowIfNull(Magnus);

        RequireFinite(Input.Mass, "La masse", Errors);
        RequireFinite(
            Input.CrossSectionalArea,
            "La surface frontale",
            Errors);
        RequireFinite(
            Input.ReferenceDragCoefficient,
            "Le coefficient de traÃ®nÃ©e de rÃ©fÃ©rence",
            Errors);
        RequireFinite(Input.Radius, "Le rayon du projectile", Errors);

        if (Input.Mass <= 0.0)
        {
            Errors.Add("La masse doit Ãªtre strictement positive.");
        }

        if (Input.CrossSectionalArea < 0.0)
        {
            Errors.Add("La surface frontale doit Ãªtre positive ou nulle.");
        }

        if (Input.ReferenceDragCoefficient < 0.0)
        {
            Errors.Add(
                "Le coefficient de traÃ®nÃ©e de rÃ©fÃ©rence doit Ãªtre positif ou nul.");
        }

        if (Magnus.IsEnabled && Input.Radius <= 0.0)
        {
            Errors.Add(
                "Le rayon du projectile doit Ãªtre strictement positif lorsque l'effet Magnus est activÃ©.");
        }
    }

    private static void ValidateCelestialBody(
        CelestialBodyInput Input,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        if (!Enum.IsDefined(typeof(GravityModelKind), Input.ModelKind))
        {
            Errors.Add("Le modÃ¨le de gravitÃ© sÃ©lectionnÃ© est inconnu.");
            return;
        }

        if (Input.ModelKind == GravityModelKind.Constant)
        {
            RequireFinite(
                Input.SurfaceGravityAcceleration,
                "L'accÃ©lÃ©ration de la gravitÃ© constante",
                Errors);

            if (Input.SurfaceGravityAcceleration <= 0.0)
            {
                Errors.Add(
                    "L'accÃ©lÃ©ration de la gravitÃ© constante doit Ãªtre strictement positive.");
            }

            return;
        }

        if (!Enum.IsDefined(
                typeof(UniformSphereDefinitionKind),
                Input.UniformSphereDefinition))
        {
            Errors.Add(
                "La dÃ©finition de la sphÃ¨re uniforme est inconnue.");
            return;
        }

        RequireFinite(
            Input.BodyRadius,
            "Le rayon de l'astre",
            Errors);

        if (Input.BodyRadius <= 0.0)
        {
            Errors.Add("Le rayon de l'astre doit Ãªtre strictement positif.");
        }

        switch (Input.UniformSphereDefinition)
        {
            case UniformSphereDefinitionKind.SurfaceGravity:
                RequireFinite(
                    Input.SurfaceGravityAcceleration,
                    "La gravitÃ© de surface",
                    Errors);

                if (Input.SurfaceGravityAcceleration <= 0.0)
                {
                    Errors.Add(
                        "La gravitÃ© de surface doit Ãªtre strictement positive.");
                }

                break;

            case UniformSphereDefinitionKind.Density:
                RequireFinite(
                    Input.BodyDensity,
                    "La masse volumique de l'astre",
                    Errors);

                if (Input.BodyDensity <= 0.0)
                {
                    Errors.Add(
                        "La masse volumique de l'astre doit Ãªtre strictement positive.");
                }

                break;

            case UniformSphereDefinitionKind.Mass:
                RequireFinite(
                    Input.BodyMass,
                    "La masse de l'astre",
                    Errors);

                if (Input.BodyMass <= 0.0)
                {
                    Errors.Add(
                        "La masse de l'astre doit Ãªtre strictement positive.");
                }

                break;
        }
    }

    private static void ValidateAtmosphere(
        AtmosphereInput Input,
        InitialConditionsInput InitialConditions,
        SimulationLimitsInput Limits,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        if (!Enum.IsDefined(typeof(AtmosphereModelKind), Input.ModelKind))
        {
            Errors.Add("Le modÃ¨le atmosphÃ©rique sÃ©lectionnÃ© est inconnu.");
            return;
        }

        if (Input.ModelKind == AtmosphereModelKind.Standard1976)
        {
            ValidateStandardAtmosphereAltitude(
                InitialConditions.InitialZ,
                "La position initiale",
                Errors);
            ValidateStandardAtmosphereAltitude(
                Limits.GroundAltitude,
                "L'altitude du sol",
                Errors);
            return;
        }

        RequireFinite(
            Input.ConstantTemperature,
            "La tempÃ©rature atmosphÃ©rique constante",
            Errors);

        if (Input.ConstantTemperature <= 0.0)
        {
            Errors.Add(
                "La tempÃ©rature atmosphÃ©rique constante doit Ãªtre strictement positive.");
        }

        if (Input.ModelKind == AtmosphereModelKind.Vacuum)
        {
            return;
        }

        if (!Enum.IsDefined(
                typeof(ConstantAtmosphereDefinitionKind),
                Input.ConstantDefinition))
        {
            Errors.Add(
                "La dÃ©finition de l'atmosphÃ¨re constante est inconnue.");
            return;
        }

        if (
            Input.ConstantDefinition ==
            ConstantAtmosphereDefinitionKind.DensityAndTemperature)
        {
            RequireFinite(
                Input.ConstantAirDensity,
                "La densitÃ© atmosphÃ©rique constante",
                Errors);

            if (Input.ConstantAirDensity < 0.0)
            {
                Errors.Add(
                    "La densitÃ© atmosphÃ©rique constante doit Ãªtre positive ou nulle.");
            }

            return;
        }

        RequireFinite(
            Input.ConstantPressure,
            "La pression atmosphÃ©rique constante",
            Errors);

        if (Input.ConstantPressure < 0.0)
        {
            Errors.Add(
                "La pression atmosphÃ©rique constante doit Ãªtre positive ou nulle.");
        }
    }

    private static void ValidateGas(
        GasInput Input,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        RequireFinite(
            Input.SpecificHeatRatio,
            "Le rapport des capacitÃ©s thermiques",
            Errors);
        RequireFinite(
            Input.SpecificGasConstant,
            "La constante spÃ©cifique du gaz",
            Errors);

        if (Input.SpecificHeatRatio <= 1.0)
        {
            Errors.Add(
                "Le rapport des capacitÃ©s thermiques doit Ãªtre strictement supÃ©rieur Ã  1.");
        }

        if (Input.SpecificGasConstant <= 0.0)
        {
            Errors.Add(
                "La constante spÃ©cifique du gaz doit Ãªtre strictement positive.");
        }
    }

    private static void ValidateStandardAtmosphereAltitude(
        double Altitude,
        string Name,
        ICollection<string> Errors)
    {
        if (Altitude < StandardAtmosphere1976Model.MinimumSupportedAltitude ||
            Altitude > StandardAtmosphere1976Model.MaximumSupportedAltitude)
        {
            Errors.Add(
                $"{Name} doit rester comprise entre " +
                $"{StandardAtmosphere1976Model.MinimumSupportedAltitude} m et " +
                $"{StandardAtmosphere1976Model.MaximumSupportedAltitude} m " +
                "avec l'atmosphÃ¨re standard 1976.");
        }
    }

    private static void ValidateWind(
        WindInput Input,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        if (!Enum.IsDefined(typeof(WindModelKind), Input.ModelKind))
        {
            Errors.Add("Le modÃ¨le de vent sÃ©lectionnÃ© est inconnu.");
            return;
        }

        switch (Input.ModelKind)
        {
            case WindModelKind.None:
                return;

            case WindModelKind.Constant:
                ValidateVector(
                    Input.ConstantWindX,
                    Input.ConstantWindY,
                    Input.ConstantWindZ,
                    "Le vent constant",
                    Errors);
                return;

            case WindModelKind.Linear:
                RequireFinite(Input.LowerAltitude, "L'altitude basse", Errors);
                RequireFinite(Input.UpperAltitude, "L'altitude haute", Errors);
                ValidateVector(
                    Input.LowerWindX,
                    Input.LowerWindY,
                    Input.LowerWindZ,
                    "Le vent Ã  l'altitude basse",
                    Errors);
                ValidateVector(
                    Input.UpperWindX,
                    Input.UpperWindY,
                    Input.UpperWindZ,
                    "Le vent Ã  l'altitude haute",
                    Errors);

                if (Input.UpperAltitude <= Input.LowerAltitude)
                {
                    Errors.Add(
                        "L'altitude haute du vent linÃ©aire doit Ãªtre strictement supÃ©rieure Ã  l'altitude basse.");
                }

                return;

            case WindModelKind.Layered:
                ValidateWindLayers(Input.Layers, Errors);
                return;
        }
    }

    private static void ValidateWindLayers(
        IReadOnlyList<WindLayerInput> Layers,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Layers);

        if (Layers.Count < 2)
        {
            Errors.Add(
                "Le vent par couches doit contenir au moins deux couches.");
            return;
        }

        double PreviousAltitude = double.NegativeInfinity;

        for (int Index = 0; Index < Layers.Count; Index++)
        {
            WindLayerInput Layer = Layers[Index];

            if (Layer is null)
            {
                Errors.Add("Une couche de vent ne peut pas Ãªtre nulle.");
                continue;
            }

            RequireFinite(
                Layer.Altitude,
                $"L'altitude de la couche {Index + 1}",
                Errors);
            ValidateVector(
                Layer.WindX,
                Layer.WindY,
                Layer.WindZ,
                $"Le vent de la couche {Index + 1}",
                Errors);

            if (Index > 0 && Layer.Altitude <= PreviousAltitude)
            {
                Errors.Add(
                    "Les altitudes des couches de vent doivent Ãªtre strictement croissantes.");
            }

            PreviousAltitude = Layer.Altitude;
        }
    }

    private static void ValidateDrag(
        DragInput Input,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        if (!Enum.IsDefined(
                typeof(DragCoefficientModelKind),
                Input.ModelKind))
        {
            Errors.Add(
                "Le modÃ¨le de coefficient de traÃ®nÃ©e sÃ©lectionnÃ© est inconnu.");
            return;
        }

        if (!Input.IsEnabled ||
            Input.ModelKind == DragCoefficientModelKind.Constant)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(Input.Points);

        if (Input.Points.Count < 2)
        {
            Errors.Add(
                "La courbe de traÃ®nÃ©e tabulÃ©e doit contenir au moins deux points.");
            return;
        }

        double PreviousMachNumber = double.NegativeInfinity;

        for (int Index = 0; Index < Input.Points.Count; Index++)
        {
            DragCoefficientPointInput Point = Input.Points[Index];

            if (Point is null)
            {
                Errors.Add("Un point de traÃ®nÃ©e ne peut pas Ãªtre nul.");
                continue;
            }

            RequireFinite(
                Point.MachNumber,
                $"Le nombre de Mach du point {Index + 1}",
                Errors);
            RequireFinite(
                Point.DragCoefficient,
                $"Le coefficient de traÃ®nÃ©e du point {Index + 1}",
                Errors);

            if (Point.MachNumber < 0.0)
            {
                Errors.Add("Les nombres de Mach doivent Ãªtre positifs ou nuls.");
            }

            if (Point.DragCoefficient < 0.0)
            {
                Errors.Add(
                    "Les coefficients de traÃ®nÃ©e doivent Ãªtre positifs ou nuls.");
            }

            if (Index > 0 && Point.MachNumber <= PreviousMachNumber)
            {
                Errors.Add(
                    "Les nombres de Mach doivent Ãªtre strictement croissants.");
            }

            PreviousMachNumber = Point.MachNumber;
        }
    }

    private static void ValidateRotation(
        RotationInput Input,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        if (!Input.IsCoriolisEnabled)
        {
            return;
        }

        if (!Enum.IsDefined(
                typeof(CoriolisDefinitionKind),
                Input.DefinitionKind))
        {
            Errors.Add(
                "La dÃ©finition de la rotation de l'astre est inconnue.");
            return;
        }

        if (Input.DefinitionKind == CoriolisDefinitionKind.Latitude)
        {
            RequireFinite(Input.LatitudeDegrees, "La latitude", Errors);
            RequireFinite(
                Input.AngularVelocity,
                "La vitesse angulaire de l'astre",
                Errors);

            if (Input.LatitudeDegrees < -90.0 ||
                Input.LatitudeDegrees > 90.0)
            {
                Errors.Add(
                    "La latitude doit Ãªtre comprise entre -90Â° et 90Â°.");
            }

            if (Input.AngularVelocity < 0.0)
            {
                Errors.Add(
                    "La vitesse angulaire de l'astre doit Ãªtre positive ou nulle.");
            }

            return;
        }

        ValidateVector(
            Input.AngularVelocityX,
            Input.AngularVelocityY,
            Input.AngularVelocityZ,
            "Le vecteur de vitesse angulaire de l'astre",
            Errors);
    }

    private static void ValidateMagnus(
        MagnusInput Input,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        ValidateVector(
            Input.AngularVelocityX,
            Input.AngularVelocityY,
            Input.AngularVelocityZ,
            "La vitesse angulaire du projectile",
            Errors);
        RequireFinite(
            Input.MagnusCoefficient,
            "Le coefficient de Magnus",
            Errors);

        if (Input.MagnusCoefficient < 0.0)
        {
            Errors.Add(
                "Le coefficient de Magnus doit Ãªtre positif ou nul.");
        }
    }

    private static void ValidateSolver(
        SolverInput Input,
        SimulationLimitsInput Limits,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        if (!Enum.IsDefined(typeof(SolverKind), Input.ModelKind))
        {
            Errors.Add("Le solveur sÃ©lectionnÃ© est inconnu.");
        }

        RequireFinite(Input.TimeStep, "Le pas de temps", Errors);

        if (Input.TimeStep <= 0.0)
        {
            Errors.Add("Le pas de temps doit Ãªtre strictement positif.");
        }

        if (Input.TimeStep > Limits.MaximumSimulationTime)
        {
            Errors.Add(
                "Le pas de temps ne peut pas dÃ©passer la durÃ©e maximale.");
        }

        if (Input.ModelKind != SolverKind.RungeKutta45)
        {
            return;
        }

        RequireFinite(
            Input.AbsoluteTolerance,
            "La tolÃ©rance absolue",
            Errors);
        RequireFinite(
            Input.RelativeTolerance,
            "La tolÃ©rance relative",
            Errors);
        RequireFinite(
            Input.MinimumTimeStep,
            "Le pas minimal",
            Errors);
        RequireFinite(
            Input.MaximumTimeStep,
            "Le pas maximal",
            Errors);

        if (Input.AbsoluteTolerance <= 0.0)
        {
            Errors.Add("La tolÃ©rance absolue doit Ãªtre strictement positive.");
        }

        if (Input.RelativeTolerance <= 0.0)
        {
            Errors.Add("La tolÃ©rance relative doit Ãªtre strictement positive.");
        }

        if (Input.MinimumTimeStep <= 0.0)
        {
            Errors.Add("Le pas minimal doit Ãªtre strictement positif.");
        }

        if (Input.MaximumTimeStep <= 0.0)
        {
            Errors.Add("Le pas maximal doit Ãªtre strictement positif.");
        }

        if (Input.MinimumTimeStep > Input.MaximumTimeStep)
        {
            Errors.Add("Le pas minimal ne peut pas dÃ©passer le pas maximal.");
        }
    }

    private static void ValidateLimits(
        InitialConditionsInput InitialConditions,
        CelestialBodyInput CelestialBody,
        SimulationLimitsInput Input,
        ICollection<string> Errors)
    {
        ArgumentNullException.ThrowIfNull(Input);

        RequireFinite(
            Input.MaximumSimulationTime,
            "La durÃ©e maximale",
            Errors);
        RequireFinite(Input.GroundAltitude, "L'altitude du sol", Errors);

        if (Input.MaximumSimulationTime <= 0.0)
        {
            Errors.Add("La durÃ©e maximale doit Ãªtre strictement positive.");
        }

        if (InitialConditions.InitialZ < Input.GroundAltitude)
        {
            Errors.Add(
                "La position initiale ne peut pas Ãªtre situÃ©e sous le sol.");
        }

        if (CelestialBody.ModelKind == GravityModelKind.UniformSphere &&
            Input.GroundAltitude < -CelestialBody.BodyRadius)
        {
            Errors.Add(
                "L'altitude du sol ne peut pas Ãªtre situÃ©e au-delÃ  du centre de l'astre.");
        }
    }

    private static void ValidateVector(
        double X,
        double Y,
        double Z,
        string Name,
        ICollection<string> Errors)
    {
        RequireFinite(X, $"{Name} sur X", Errors);
        RequireFinite(Y, $"{Name} sur Y", Errors);
        RequireFinite(Z, $"{Name} sur Z", Errors);
    }

    private static void RequireFinite(
        double Value,
        string Name,
        ICollection<string> Errors)
    {
        if (!double.IsFinite(Value))
        {
            Errors.Add($"{Name} doit Ãªtre finie.");
        }
    }
}