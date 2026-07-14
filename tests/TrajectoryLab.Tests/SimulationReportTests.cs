using System.Runtime.CompilerServices;
using TrajectoryLab.Core;
using TrajectoryLab.Core.Models;
using TrajectoryLab.Core.Simulation;

namespace TrajectoryLab.Tests.Simulation;

public sealed class SimulationReportTests
{
    [Fact]
    public void Constructor_StoresValuesAndCopiesFlightConditions()
    {
        SimulationResult Result =
            CreateUninitialized<SimulationResult>();

        AerodynamicSummary Summary =
            CreateUninitialized<AerodynamicSummary>();

        List<FlightCondition> FlightConditions =
        [
            CreateUninitialized<FlightCondition>()
        ];

        SimulationReport Report = new(
            Result,
            FlightConditions,
            Summary);

        FlightConditions.Add(
            CreateUninitialized<FlightCondition>());

        Assert.Same(
            Result,
            Report.Result);

        Assert.Same(
            Summary,
            Report.AerodynamicSummary);

        Assert.Single(
            Report.FlightConditions);
    }

    [Fact]
    public void Constructor_WithNullResult_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SimulationReport(
                null!,
                Array.Empty<FlightCondition>(),
                CreateUninitialized<AerodynamicSummary>()));
    }

    [Fact]
    public void Constructor_WithNullFlightConditions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SimulationReport(
                CreateUninitialized<SimulationResult>(),
                null!,
                CreateUninitialized<AerodynamicSummary>()));
    }

    [Fact]
    public void Constructor_WithNullAerodynamicSummary_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SimulationReport(
                CreateUninitialized<SimulationResult>(),
                Array.Empty<FlightCondition>(),
                null!));
    }

    private static T CreateUninitialized<T>()
        where T : class
    {
        return (T)RuntimeHelpers.GetUninitializedObject(
            typeof(T));
    }
}