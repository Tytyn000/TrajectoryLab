namespace TrajectoryLab.Core.Simulation;

public sealed record PolynomialCurveSamples(
    double[] HorizontalValues,
    double[] VerticalValues);

public sealed record PolynomialFitResult(
    int Degree,
    double Center,
    double[] Coefficients,
    double MinimumHorizontalValue,
    double MaximumHorizontalValue,
    double StartHorizontalValue,
    double EndHorizontalValue,
    int PointCount,
    double RootMeanSquareError,
    double CoefficientOfDetermination)
{
    public double Evaluate(
        double HorizontalValue)
    {
        if (!double.IsFinite(HorizontalValue))
        {
            throw new ArgumentOutOfRangeException(
                nameof(HorizontalValue),
                "La valeur horizontale doit être finie.");
        }

        double ShiftedValue =
            HorizontalValue -
            Center;

        double Result =
            0.0;

        for (
            int Index = Degree;
            Index >= 0;
            Index--)
        {
            Result =
                Result *
                ShiftedValue +
                Coefficients[Index];
        }

        return Result;
    }

    public PolynomialCurveSamples Sample(
        int SampleCount)
    {
        if (SampleCount < 2)
        {
            throw new ArgumentOutOfRangeException(
                nameof(SampleCount),
                "Le nombre d'échantillons doit être au moins égal à deux.");
        }

        double[] HorizontalValues =
            new double[SampleCount];

        double[] VerticalValues =
            new double[SampleCount];

        for (
            int Index = 0;
            Index < SampleCount;
            Index++)
        {
            double Ratio =
                (double)Index /
                (SampleCount - 1);

            double HorizontalValue =
                StartHorizontalValue +
                Ratio *
                (
                    EndHorizontalValue -
                    StartHorizontalValue
                );

            HorizontalValues[Index] =
                HorizontalValue;

            VerticalValues[Index] =
                Evaluate(
                    HorizontalValue);
        }

        return new PolynomialCurveSamples(
            HorizontalValues,
            VerticalValues);
    }
}

public sealed record PolynomialCurveBranchFit(
    int Index,
    int StartPointIndex,
    int EndPointIndex,
    bool IsIncreasing,
    PolynomialFitResult Fit);

public sealed record PolynomialCurveFitAnalysis(
    bool IsAvailable,
    string Message,
    TrajectoryQuantityInfo HorizontalQuantity,
    TrajectoryQuantityInfo VerticalQuantity,
    IReadOnlyList<PolynomialCurveBranchFit> Branches,
    int RequestedDegree)
{
    public bool IsPiecewise =>
        Branches.Count > 1;
}

/// Ajuste l'ordonnée en fonction de l'abscisse avec un polynôme
/// calculé par moindres carrés.
public static class PolynomialCurveFitter
{
    private const double NumericalTolerance =
        1.0e-12;

    public static PolynomialCurveFitAnalysis Analyze(
        IReadOnlyList<double> HorizontalValues,
        IReadOnlyList<double> VerticalValues,
        TrajectoryQuantityInfo HorizontalQuantity,
        TrajectoryQuantityInfo VerticalQuantity,
        int RequestedDegree = 0,
        int MaximumAutomaticDegree = 6)
    {
        ArgumentNullException.ThrowIfNull(
            HorizontalValues);

        ArgumentNullException.ThrowIfNull(
            VerticalValues);

        ArgumentNullException.ThrowIfNull(
            HorizontalQuantity);

        ArgumentNullException.ThrowIfNull(
            VerticalQuantity);

        ValidateValues(
            HorizontalValues,
            VerticalValues);

        if (
            RequestedDegree < 0 ||
            RequestedDegree > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(RequestedDegree),
                "Le degré demandé doit être compris entre zéro et douze.");
        }

        if (
            MaximumAutomaticDegree < 1 ||
            MaximumAutomaticDegree > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaximumAutomaticDegree),
                "Le degré automatique maximal doit être compris entre un et douze.");
        }

        if (HorizontalValues.Count < 2)
        {
            return CreateUnavailableAnalysis(
                "Deux points au minimum sont nécessaires.",
                HorizontalQuantity,
                VerticalQuantity,
                RequestedDegree);
        }

        double MinimumHorizontalValue =
            HorizontalValues.Min();

        double MaximumHorizontalValue =
            HorizontalValues.Max();

        double HorizontalTolerance =
            NumericalTolerance *
            Math.Max(
                1.0,
                Math.Max(
                    Math.Abs(MinimumHorizontalValue),
                    Math.Abs(MaximumHorizontalValue)));

        if (
            MaximumHorizontalValue -
            MinimumHorizontalValue <=
            HorizontalTolerance)
        {
            return CreateUnavailableAnalysis(
                "L'axe horizontal est constant. Il est impossible d'ajuster une fonction de cet axe.",
                HorizontalQuantity,
                VerticalQuantity,
                RequestedDegree);
        }

        IReadOnlyList<(
            int StartIndex,
            int EndIndex
        )> BranchRanges =
            FindMonotonicBranchRanges(
                HorizontalValues);

        List<PolynomialCurveBranchFit> Branches =
            [];

        foreach (
            (
                int StartIndex,
                int EndIndex
            ) in BranchRanges)
        {
            int PointCount =
                EndIndex -
                StartIndex +
                1;

            if (PointCount < 2)
            {
                continue;
            }

            double[] BranchHorizontalValues =
                new double[PointCount];

            double[] BranchVerticalValues =
                new double[PointCount];

            for (
                int LocalIndex = 0;
                LocalIndex < PointCount;
                LocalIndex++)
            {
                int SourceIndex =
                    StartIndex +
                    LocalIndex;

                BranchHorizontalValues[LocalIndex] =
                    HorizontalValues[SourceIndex];

                BranchVerticalValues[LocalIndex] =
                    VerticalValues[SourceIndex];
            }

            PolynomialFitResult? Fit =
                CreateBranchFit(
                    BranchHorizontalValues,
                    BranchVerticalValues,
                    RequestedDegree,
                    MaximumAutomaticDegree);

            if (Fit is null)
            {
                continue;
            }

            bool IsIncreasing =
                BranchHorizontalValues[^1] >=
                BranchHorizontalValues[0];

            Branches.Add(
                new PolynomialCurveBranchFit(
                    Branches.Count,
                    StartIndex,
                    EndIndex,
                    IsIncreasing,
                    Fit));
        }

        if (Branches.Count == 0)
        {
            return CreateUnavailableAnalysis(
                "Les points ne permettent pas de construire un ajustement polynomial stable.",
                HorizontalQuantity,
                VerticalQuantity,
                RequestedDegree);
        }

        string Message =
            Branches.Count == 1
                ? "Une équation polynomiale a été ajustée aux points par moindres carrés."
                : "L'abscisse n'est pas monotone. Une équation distincte a été calculée pour chaque branche.";

        return new PolynomialCurveFitAnalysis(
            true,
            Message,
            HorizontalQuantity,
            VerticalQuantity,
            Array.AsReadOnly(
                Branches.ToArray()),
            RequestedDegree);
    }

    public static PolynomialFitResult Fit(
        IReadOnlyList<double> HorizontalValues,
        IReadOnlyList<double> VerticalValues,
        int Degree)
    {
        ArgumentNullException.ThrowIfNull(
            HorizontalValues);

        ArgumentNullException.ThrowIfNull(
            VerticalValues);

        ValidateValues(
            HorizontalValues,
            VerticalValues);

        if (Degree < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Degree),
                "Le degré doit être au moins égal à un.");
        }

        if (HorizontalValues.Count <= Degree)
        {
            throw new ArgumentException(
                "Le nombre de points doit être supérieur au degré du polynôme.",
                nameof(HorizontalValues));
        }

        int PointCount =
            HorizontalValues.Count;

        int CoefficientCount =
            Degree + 1;

        double MinimumHorizontalValue =
            HorizontalValues.Min();

        double MaximumHorizontalValue =
            HorizontalValues.Max();

        double Center =
            0.5 *
            (
                MinimumHorizontalValue +
                MaximumHorizontalValue
            );

        double Scale =
            0.5 *
            (
                MaximumHorizontalValue -
                MinimumHorizontalValue
            );

        double ScaleTolerance =
            NumericalTolerance *
            Math.Max(
                1.0,
                Math.Abs(Center));

        if (Scale <= ScaleTolerance)
        {
            throw new InvalidOperationException(
                "Les abscisses ne sont pas suffisamment distinctes.");
        }

        double[,] Matrix =
            new double[
                PointCount,
                CoefficientCount];

        for (
            int Row = 0;
            Row < PointCount;
            Row++)
        {
            double NormalizedHorizontalValue =
                (
                    HorizontalValues[Row] -
                    Center
                ) /
                Scale;

            double Power =
                1.0;

            for (
                int Column = 0;
                Column < CoefficientCount;
                Column++)
            {
                Matrix[Row, Column] =
                    Power;

                Power *=
                    NormalizedHorizontalValue;
            }
        }

        double[] NormalizedCoefficients =
            SolveLeastSquaresByQr(
                Matrix,
                VerticalValues);

        double[] ShiftedCoefficients =
            new double[CoefficientCount];

        double ScalePower =
            1.0;

        for (
            int Index = 0;
            Index < CoefficientCount;
            Index++)
        {
            ShiftedCoefficients[Index] =
                NormalizedCoefficients[Index] /
                ScalePower;

            ScalePower *=
                Scale;
        }

        PolynomialFitResult PreliminaryResult =
            new(
                Degree,
                Center,
                ShiftedCoefficients,
                MinimumHorizontalValue,
                MaximumHorizontalValue,
                HorizontalValues[0],
                HorizontalValues[^1],
                PointCount,
                0.0,
                0.0);

        double VerticalMean =
            VerticalValues.Average();

        double SquaredResidualSum =
            0.0;

        double TotalSquaredDeviation =
            0.0;

        for (
            int Index = 0;
            Index < PointCount;
            Index++)
        {
            double ExpectedValue =
                PreliminaryResult.Evaluate(
                    HorizontalValues[Index]);

            double Residual =
                VerticalValues[Index] -
                ExpectedValue;

            SquaredResidualSum +=
                Residual *
                Residual;

            double MeanDeviation =
                VerticalValues[Index] -
                VerticalMean;

            TotalSquaredDeviation +=
                MeanDeviation *
                MeanDeviation;
        }

        double RootMeanSquareError =
            Math.Sqrt(
                SquaredResidualSum /
                PointCount);

        double CoefficientOfDetermination;

        if (
            TotalSquaredDeviation <=
            double.Epsilon)
        {
            CoefficientOfDetermination =
                SquaredResidualSum <=
                double.Epsilon
                    ? 1.0
                    : 0.0;
        }
        else
        {
            CoefficientOfDetermination =
                1.0 -
                SquaredResidualSum /
                TotalSquaredDeviation;
        }

        return PreliminaryResult with
        {
            RootMeanSquareError =
                RootMeanSquareError,

            CoefficientOfDetermination =
                CoefficientOfDetermination
        };
    }

    private static PolynomialFitResult?
        CreateBranchFit(
            IReadOnlyList<double> HorizontalValues,
            IReadOnlyList<double> VerticalValues,
            int RequestedDegree,
            int MaximumAutomaticDegree)
    {
        int MaximumPossibleDegree =
            Math.Min(
                HorizontalValues.Count - 1,
                RequestedDegree == 0
                    ? MaximumAutomaticDegree
                    : RequestedDegree);

        if (MaximumPossibleDegree < 1)
        {
            return null;
        }

        if (RequestedDegree > 0)
        {
            for (
                int Degree = MaximumPossibleDegree;
                Degree >= 1;
                Degree--)
            {
                try
                {
                    return Fit(
                        HorizontalValues,
                        VerticalValues,
                        Degree);
                }
                catch (InvalidOperationException)
                {
                }
            }

            return null;
        }

        PolynomialFitResult? BestFit =
            null;

        double BestScore =
            double.PositiveInfinity;

        for (
            int Degree = 1;
            Degree <= MaximumPossibleDegree;
            Degree++)
        {
            PolynomialFitResult CurrentFit;

            try
            {
                CurrentFit =
                    Fit(
                        HorizontalValues,
                        VerticalValues,
                        Degree);
            }
            catch (InvalidOperationException)
            {
                continue;
            }

            double MeanSquaredError =
                CurrentFit.RootMeanSquareError *
                CurrentFit.RootMeanSquareError;

            double SafeMeanSquaredError =
                Math.Max(
                    MeanSquaredError,
                    1.0e-30);

            int ParameterCount =
                Degree + 1;

            double BayesianInformationCriterion =
                HorizontalValues.Count *
                Math.Log(
                    SafeMeanSquaredError) +
                ParameterCount *
                Math.Log(
                    HorizontalValues.Count);

            if (
                BayesianInformationCriterion <
                BestScore)
            {
                BestScore =
                    BayesianInformationCriterion;

                BestFit =
                    CurrentFit;
            }
        }

        return BestFit;
    }

    private static IReadOnlyList<(
        int StartIndex,
        int EndIndex
    )> FindMonotonicBranchRanges(
        IReadOnlyList<double> HorizontalValues)
    {
        List<(
            int StartIndex,
            int EndIndex
        )> Ranges =
            [];

        double MinimumValue =
            HorizontalValues.Min();

        double MaximumValue =
            HorizontalValues.Max();

        double Tolerance =
            NumericalTolerance *
            Math.Max(
                1.0,
                Math.Max(
                    Math.Abs(MinimumValue),
                    Math.Abs(MaximumValue)));

        int BranchStartIndex =
            0;

        int CurrentDirection =
            0;

        for (
            int Index = 1;
            Index < HorizontalValues.Count;
            Index++)
        {
            double Difference =
                HorizontalValues[Index] -
                HorizontalValues[Index - 1];

            int Direction =
                Difference > Tolerance
                    ? 1
                    : Difference < -Tolerance
                        ? -1
                        : 0;

            if (Direction == 0)
            {
                continue;
            }

            if (CurrentDirection == 0)
            {
                CurrentDirection =
                    Direction;

                continue;
            }

            if (Direction == CurrentDirection)
            {
                continue;
            }

            int TurningPointIndex =
                Index - 1;

            if (
                TurningPointIndex -
                BranchStartIndex >= 1)
            {
                Ranges.Add(
                    (
                        BranchStartIndex,
                        TurningPointIndex
                    ));
            }

            BranchStartIndex =
                TurningPointIndex;

            CurrentDirection =
                Direction;
        }

        if (
            HorizontalValues.Count - 1 -
            BranchStartIndex >= 1)
        {
            Ranges.Add(
                (
                    BranchStartIndex,
                    HorizontalValues.Count - 1
                ));
        }

        if (Ranges.Count == 0)
        {
            Ranges.Add(
                (
                    0,
                    HorizontalValues.Count - 1
                ));
        }

        return Ranges;
    }

    private static double[]
        SolveLeastSquaresByQr(
            double[,] Matrix,
            IReadOnlyList<double> Values)
    {
        int RowCount =
            Matrix.GetLength(0);

        int ColumnCount =
            Matrix.GetLength(1);

        double[,] OrthogonalMatrix =
            new double[
                RowCount,
                ColumnCount];

        double[,] UpperTriangularMatrix =
            new double[
                ColumnCount,
                ColumnCount];

        for (
            int Column = 0;
            Column < ColumnCount;
            Column++)
        {
            double[] WorkingVector =
                new double[RowCount];

            for (
                int Row = 0;
                Row < RowCount;
                Row++)
            {
                WorkingVector[Row] =
                    Matrix[Row, Column];
            }

            for (
                int PreviousColumn = 0;
                PreviousColumn < Column;
                PreviousColumn++)
            {
                double Projection =
                    0.0;

                for (
                    int Row = 0;
                    Row < RowCount;
                    Row++)
                {
                    Projection +=
                        OrthogonalMatrix[
                            Row,
                            PreviousColumn
                        ] *
                        WorkingVector[Row];
                }

                UpperTriangularMatrix[
                    PreviousColumn,
                    Column
                ] =
                    Projection;

                for (
                    int Row = 0;
                    Row < RowCount;
                    Row++)
                {
                    WorkingVector[Row] -=
                        Projection *
                        OrthogonalMatrix[
                            Row,
                            PreviousColumn
                        ];
                }
            }

            double NormSquared =
                0.0;

            for (
                int Row = 0;
                Row < RowCount;
                Row++)
            {
                NormSquared +=
                    WorkingVector[Row] *
                    WorkingVector[Row];
            }

            double Norm =
                Math.Sqrt(
                    NormSquared);

            if (
                !double.IsFinite(Norm) ||
                Norm <= NumericalTolerance)
            {
                throw new InvalidOperationException(
                    "Le système d'ajustement polynomial est singulier.");
            }

            UpperTriangularMatrix[
                Column,
                Column
            ] =
                Norm;

            for (
                int Row = 0;
                Row < RowCount;
                Row++)
            {
                OrthogonalMatrix[
                    Row,
                    Column
                ] =
                    WorkingVector[Row] /
                    Norm;
            }
        }

        double[] ProjectedValues =
            new double[ColumnCount];

        for (
            int Column = 0;
            Column < ColumnCount;
            Column++)
        {
            double Projection =
                0.0;

            for (
                int Row = 0;
                Row < RowCount;
                Row++)
            {
                Projection +=
                    OrthogonalMatrix[
                        Row,
                        Column
                    ] *
                    Values[Row];
            }

            ProjectedValues[Column] =
                Projection;
        }

        double[] Coefficients =
            new double[ColumnCount];

        for (
            int Row = ColumnCount - 1;
            Row >= 0;
            Row--)
        {
            double RemainingValue =
                ProjectedValues[Row];

            for (
                int Column = Row + 1;
                Column < ColumnCount;
                Column++)
            {
                RemainingValue -=
                    UpperTriangularMatrix[
                        Row,
                        Column
                    ] *
                    Coefficients[Column];
            }

            Coefficients[Row] =
                RemainingValue /
                UpperTriangularMatrix[
                    Row,
                    Row
                ];
        }

        return Coefficients;
    }

    private static void ValidateValues(
        IReadOnlyList<double> HorizontalValues,
        IReadOnlyList<double> VerticalValues)
    {
        if (
            HorizontalValues.Count !=
            VerticalValues.Count)
        {
            throw new ArgumentException(
                "Les tableaux horizontal et vertical doivent avoir la même longueur.");
        }

        for (
            int Index = 0;
            Index < HorizontalValues.Count;
            Index++)
        {
            if (
                !double.IsFinite(
                    HorizontalValues[Index]) ||
                !double.IsFinite(
                    VerticalValues[Index]))
            {
                throw new ArgumentException(
                    "Toutes les coordonnées doivent être finies.");
            }
        }
    }

    private static PolynomialCurveFitAnalysis
        CreateUnavailableAnalysis(
            string Message,
            TrajectoryQuantityInfo HorizontalQuantity,
            TrajectoryQuantityInfo VerticalQuantity,
            int RequestedDegree)
    {
        return new PolynomialCurveFitAnalysis(
            false,
            Message,
            HorizontalQuantity,
            VerticalQuantity,
            Array.Empty<PolynomialCurveBranchFit>(),
            RequestedDegree);
    }
}
