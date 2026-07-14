using TrajectoryLab.Application.Models.Inputs;

namespace TrajectoryLab.Desktop.Models;

public static class SimulationOptionCatalog
{
    public static IReadOnlyList<SelectionOption<GravityModelKind>>
        GravityModels { get; } =
        [
            new(
                GravityModelKind.UniformSphere,
                "Sphère uniforme continue"),
            new(
                GravityModelKind.Constant,
                "Gravité locale constante")
        ];

    public static IReadOnlyList<SelectionOption<UniformSphereDefinitionKind>>
        UniformSphereDefinitions { get; } =
        [
            new(
                UniformSphereDefinitionKind.SurfaceGravity,
                "Rayon + gravité de surface"),
            new(
                UniformSphereDefinitionKind.Density,
                "Rayon + masse volumique"),
            new(
                UniformSphereDefinitionKind.Mass,
                "Rayon + masse totale")
        ];

    public static IReadOnlyList<SelectionOption<AtmosphereModelKind>>
        AtmosphereModels { get; } =
        [
            new(AtmosphereModelKind.Vacuum, "Vide"),
            new(AtmosphereModelKind.Constant, "Constante"),
            new(
                AtmosphereModelKind.Standard1976,
                "Atmosphère standard 1976")
        ];

    public static IReadOnlyList<SelectionOption<ConstantAtmosphereDefinitionKind>>
        ConstantAtmosphereDefinitions { get; } =
        [
            new(
                ConstantAtmosphereDefinitionKind
                    .DensityAndTemperature,
                "Densité + température"),
            new(
                ConstantAtmosphereDefinitionKind
                    .PressureAndTemperature,
                "Pression + température")
        ];

    public static IReadOnlyList<SelectionOption<WindModelKind>>
        WindModels { get; } =
        [
            new(WindModelKind.None, "Aucun"),
            new(WindModelKind.Constant, "Constant"),
            new(WindModelKind.Linear, "Linéaire"),
            new(WindModelKind.Layered, "Par couches")
        ];

    public static IReadOnlyList<SelectionOption<DragCoefficientModelKind>>
        DragCoefficientModels { get; } =
        [
            new(
                DragCoefficientModelKind.Constant,
                "Coefficient constant"),
            new(
                DragCoefficientModelKind.Tabulated,
                "Courbe tabulée selon Mach")
        ];

    public static IReadOnlyList<SelectionOption<CoriolisDefinitionKind>>
        CoriolisDefinitions { get; } =
        [
            new(
                CoriolisDefinitionKind.Latitude,
                "Latitude + vitesse angulaire"),
            new(
                CoriolisDefinitionKind.AngularVelocityVector,
                "Vecteur local Ω")
        ];

    public static IReadOnlyList<SelectionOption<SolverKind>>
        Solvers { get; } =
        [
            new(SolverKind.Euler, "Euler explicite"),
            new(SolverKind.RungeKutta4, "Runge-Kutta 4"),
            new(
                SolverKind.RungeKutta45,
                "Runge-Kutta 4/5 adaptatif")
        ];
}