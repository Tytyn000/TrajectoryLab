namespace TrajectoryLab.Application.Models.Inputs;

public enum AtmosphereModelKind
{
    Vacuum,
    Constant,
    Standard1976
}

public enum ConstantAtmosphereDefinitionKind
{
    DensityAndTemperature,
    PressureAndTemperature
}

public enum WindModelKind
{
    None,
    Constant,
    Linear,
    Layered
}

public enum DragCoefficientModelKind
{
    Constant,
    Tabulated
}

public enum GravityModelKind
{
    UniformSphere,
    Constant
}

public enum UniformSphereDefinitionKind
{
    SurfaceGravity,
    Density,
    Mass
}

public enum CoriolisDefinitionKind
{
    Latitude,
    AngularVelocityVector
}

public enum SolverKind
{
    Euler,
    RungeKutta4,
    RungeKutta45
}
