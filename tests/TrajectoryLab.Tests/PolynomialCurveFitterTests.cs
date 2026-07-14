using TrajectoryLab.Core.Simulation;
using Xunit;

namespace TrajectoryLab.Tests;

public sealed class PolynomialCurveFitterTests
{
    [Fact]
    public void FitReproducesLinearFunction()
    {
        PolynomialFitResult Fit =
            PolynomialCurveFitter.Fit(
                new[]
                {
                    0.0,
                    1.0,
                    2.0,
                    3.0
                },
                new[]
                {
                    1.0,
                    3.0,
                    5.0,
                    7.0
                },
                Degree: 1);

        Assert.Equal(
            6.0,
            Fit.Evaluate(2.5),
            10);
    }

    [Fact]
    public void FitReproducesQuadraticFunction()
    {
        double[] HorizontalValues =
        [
            -2.0,
            -1.0,
            0.0,
            1.0,
            2.0
        ];

        double[] VerticalValues =
            HorizontalValues
                .Select(
                    HorizontalValue =>
                        2.0 +
                        3.0 *
                        HorizontalValue -
                        4.0 *
                        HorizontalValue *
                        HorizontalValue)
                .ToArray();

        PolynomialFitResult Fit =
            PolynomialCurveFitter.Fit(
                HorizontalValues,
                VerticalValues,
                Degree: 2);

        double HorizontalValue =
            0.75;

        double ExpectedValue =
            2.0 +
            3.0 *
            HorizontalValue -
            4.0 *
            HorizontalValue *
            HorizontalValue;

        Assert.Equal(
            ExpectedValue,
            Fit.Evaluate(
                HorizontalValue),
            10);
    }

    [Fact]
    public void AutomaticDegreeFindsQuadraticFunction()
    {
        double[] HorizontalValues =
        [
            -2.0,
            -1.0,
            0.0,
            1.0,
            2.0
        ];

        double[] VerticalValues =
            HorizontalValues
                .Select(
                    HorizontalValue =>
                        1.0 +
                        2.0 *
                        HorizontalValue +
                        3.0 *
                        HorizontalValue *
                        HorizontalValue)
                .ToArray();

        PolynomialCurveFitAnalysis Analysis =
            PolynomialCurveFitter.Analyze(
                HorizontalValues,
                VerticalValues,
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.Time),
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.Altitude));

        Assert.True(
            Analysis.IsAvailable);

        Assert.Single(
            Analysis.Branches);

        Assert.Equal(
            2,
            Analysis.Branches[0].Fit.Degree);
    }

    [Fact]
    public void AnalyzeUsesOneBranchForMonotonicAxis()
    {
        PolynomialCurveFitAnalysis Analysis =
            PolynomialCurveFitter.Analyze(
                new[]
                {
                    0.0,
                    1.0,
                    2.0,
                    3.0
                },
                new[]
                {
                    0.0,
                    1.0,
                    4.0,
                    9.0
                },
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.Time),
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.Altitude));

        Assert.True(
            Analysis.IsAvailable);

        Assert.Single(
            Analysis.Branches);

        Assert.False(
            Analysis.IsPiecewise);
    }

    [Fact]
    public void AnalyzeSplitsNonMonotonicHorizontalAxis()
    {
        PolynomialCurveFitAnalysis Analysis =
            PolynomialCurveFitter.Analyze(
                new[]
                {
                    0.0,
                    1.0,
                    2.0,
                    1.0,
                    0.0
                },
                new[]
                {
                    0.0,
                    1.0,
                    2.0,
                    3.0,
                    4.0
                },
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.Altitude),
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.Time));

        Assert.True(
            Analysis.IsAvailable);

        Assert.True(
            Analysis.IsPiecewise);

        Assert.Equal(
            2,
            Analysis.Branches.Count);
    }

    [Fact]
    public void AnalyzeRejectsConstantHorizontalAxis()
    {
        PolynomialCurveFitAnalysis Analysis =
            PolynomialCurveFitter.Analyze(
                new[]
                {
                    1.0,
                    1.0,
                    1.0
                },
                new[]
                {
                    0.0,
                    1.0,
                    2.0
                },
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.PositionX),
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.Time));

        Assert.False(
            Analysis.IsAvailable);
    }

    [Fact]
    public void RequestedDegreeIsLimitedByPointCount()
    {
        PolynomialCurveFitAnalysis Analysis =
            PolynomialCurveFitter.Analyze(
                new[]
                {
                    0.0,
                    1.0,
                    2.0
                },
                new[]
                {
                    0.0,
                    1.0,
                    4.0
                },
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.Time),
                TrajectoryQuantityCatalog.Get(
                    TrajectoryQuantity.Altitude),
                RequestedDegree: 6);

        Assert.True(
            Analysis.IsAvailable);

        Assert.Equal(
            2,
            Analysis.Branches[0].Fit.Degree);
    }

    [Fact]
    public void SampleIncludesBranchEndpoints()
    {
        PolynomialFitResult Fit =
            PolynomialCurveFitter.Fit(
                new[]
                {
                    0.0,
                    1.0,
                    2.0
                },
                new[]
                {
                    1.0,
                    3.0,
                    5.0
                },
                Degree: 1);

        PolynomialCurveSamples Samples =
            Fit.Sample(
                SampleCount: 5);

        Assert.Equal(
            Fit.StartHorizontalValue,
            Samples.HorizontalValues[0],
            12);

        Assert.Equal(
            Fit.EndHorizontalValue,
            Samples.HorizontalValues[^1],
            12);
    }
}
