using TrajectoryLab.Core.Models;

namespace TrajectoryLab.Core.Solvers;

/// Représente le résultat d'une tentative de pas adaptatif.
public readonly record struct AdaptiveStepResult(
    SimulationState State,
    bool IsAccepted,
    double UsedTimeStep,
    double SuggestedTimeStep,
    double ErrorNorm
);
