namespace TrajectoryLab.Core.Atmosphere;

/// Modélise l'atmosphère standard entre 86 km et 1 000 km.
public sealed class UpperAtmosphere1976Model : IAtmosphereModel
{
    public const double MinimumSupportedAltitude =
        86_000.0;

    public const double MaximumSupportedAltitude =
        1_000_000.0;

    public const double SeaLevelPressure =
        101_325.0;

    public const double SeaLevelDensity =
        1.225;

    private static readonly double[] AltitudeTableKilometers =
    [
        86.0,
        93.0,
        100.0,
        107.0,
        114.0,
        121.0,
        128.0,
        135.0,
        142.0,
        150.0,
        160.0,
        170.0,
        180.0,
        190.0,
        200.0,
        220.0,
        260.0,
        300.0,
        400.0,
        500.0,
        600.0,
        700.0,
        800.0,
        900.0,
        1_000.0
    ];

    private static readonly double[] PressureRatios =
    [
        3.6850e-6,
        1.0660e-6,
        3.1593e-7,
        1.0611e-7,
        4.3892e-8,
        2.3095e-8,
        1.3997e-8,
        9.2345e-9,
        6.4440e-9,
        4.4828e-9,
        2.9997e-9,
        2.0933e-9,
        1.5072e-9,
        1.1118e-9,
        8.3628e-10,
        4.9494e-10,
        1.9634e-10,
        8.6557e-11,
        1.4328e-11,
        2.9840e-12,
        8.1056e-13,
        3.1491e-13,
        1.6813e-13,
        1.0731e-13,
        7.4155e-14
    ];

    private static readonly double[] DensityRatios =
    [
        5.680e-6,
        1.632e-6,
        4.575e-7,
        1.341e-7,
        4.061e-8,
        1.614e-8,
        7.932e-9,
        4.461e-9,
        2.741e-9,
        1.694e-9,
        1.007e-9,
        6.380e-10,
        4.240e-10,
        2.923e-10,
        2.074e-10,
        1.116e-10,
        3.871e-11,
        1.564e-11,
        2.288e-12,
        4.257e-13,
        9.279e-14,
        2.506e-14,
        9.272e-15,
        4.701e-15,
        2.907e-15
    ];

    private static readonly double[] LogPressureDerivatives =
    [
        -0.174061,
        -0.177924,
        -0.167029,
        -0.142755,
        -0.107859,
        -0.079322,
        -0.064664,
        -0.054879,
        -0.048260,
        -0.042767,
        -0.037854,
        -0.034270,
        -0.031543,
        -0.029384,
        -0.027632,
        -0.024980,
        -0.021559,
        -0.019557,
        -0.016735,
        -0.014530,
        -0.011314,
        -0.007677,
        -0.005169,
        -0.003944,
        -0.003612
    ];

    private static readonly double[] LogDensityDerivatives =
    [
        -0.172421,
        -0.182258,
        -0.178090,
        -0.176372,
        -0.154322,
        -0.113750,
        -0.090582,
        -0.075033,
        -0.064679,
        -0.056067,
        -0.048461,
        -0.043042,
        -0.038869,
        -0.035648,
        -0.033063,
        -0.029164,
        -0.024220,
        -0.021336,
        -0.017686,
        -0.016035,
        -0.014327,
        -0.011631,
        -0.008248,
        -0.005580,
        -0.004227
    ];

    private static readonly double[] LogPressureRatios =
        CreateLogarithms(PressureRatios);

    private static readonly double[] LogDensityRatios =
        CreateLogarithms(DensityRatios);

    private static readonly
        UpperAtmosphere1976TemperatureModel TemperatureModel =
            new();

    public double GetTemperature(double Altitude)
    {
        return TemperatureModel.GetTemperature(
            Altitude
        );
    }

    public double GetPressure(double Altitude)
    {
        double PressureRatio =
            InterpolateRatio(
                Altitude,
                LogPressureRatios,
                LogPressureDerivatives
            );

        return SeaLevelPressure *
            PressureRatio;
    }

    public double GetAirDensity(double Altitude)
    {
        double DensityRatio =
            InterpolateRatio(
                Altitude,
                LogDensityRatios,
                LogDensityDerivatives
            );

        return SeaLevelDensity *
            DensityRatio;
    }

    private static double InterpolateRatio(
        double Altitude,
        double[] LogValues,
        double[] LogDerivatives)
    {
        ValidateAltitude(Altitude);

        double AltitudeKilometers =
            Altitude / 1_000.0;

        int IntervalIndex =
            FindIntervalIndex(
                AltitudeKilometers
            );

        double LogRatio =
            EvaluateCubic(
                LeftAltitude:
                    AltitudeTableKilometers[
                        IntervalIndex
                    ],
                LeftValue:
                    LogValues[
                        IntervalIndex
                    ],
                LeftDerivative:
                    LogDerivatives[
                        IntervalIndex
                    ],
                RightAltitude:
                    AltitudeTableKilometers[
                        IntervalIndex + 1
                    ],
                RightValue:
                    LogValues[
                        IntervalIndex + 1
                    ],
                RightDerivative:
                    LogDerivatives[
                        IntervalIndex + 1
                    ],
                Altitude:
                    AltitudeKilometers
            );

        return Math.Exp(LogRatio);
    }

    private static double EvaluateCubic(
        double LeftAltitude,
        double LeftValue,
        double LeftDerivative,
        double RightAltitude,
        double RightValue,
        double RightDerivative,
        double Altitude)
    {
        double IntervalLength =
            RightAltitude - LeftAltitude;

        double SecantSlope =
            (RightValue - LeftValue) /
            IntervalLength;

        double RightWeight =
            (Altitude - LeftAltitude) /
            IntervalLength;

        double LeftWeight =
            1.0 - RightWeight;

        // Polynôme cubique d'Hermite défini par les valeurs
        // et les dérivées aux deux extrémités de l'intervalle.
        return
            LeftWeight * LeftValue +
            RightWeight * RightValue -
            LeftWeight *
            RightWeight *
            IntervalLength *
            (
                LeftWeight *
                (
                    SecantSlope -
                    LeftDerivative
                )
                -
                RightWeight *
                (
                    SecantSlope -
                    RightDerivative
                )
            );
    }

    private static int FindIntervalIndex(
        double AltitudeKilometers)
    {
        if (AltitudeKilometers >=
            AltitudeTableKilometers[^1])
        {
            return
                AltitudeTableKilometers.Length -
                2;
        }

        int LowerIndex = 0;

        int UpperIndex =
            AltitudeTableKilometers.Length -
            1;

        while (UpperIndex > LowerIndex + 1)
        {
            int MiddleIndex =
                (LowerIndex + UpperIndex) /
                2;

            if (AltitudeKilometers <
                AltitudeTableKilometers[
                    MiddleIndex
                ])
            {
                UpperIndex =
                    MiddleIndex;
            }
            else
            {
                LowerIndex =
                    MiddleIndex;
            }
        }

        return LowerIndex;
    }

    private static double[] CreateLogarithms(
        double[] Values)
    {
        double[] Logarithms =
            new double[Values.Length];

        for (
            int Index = 0;
            Index < Values.Length;
            Index++)
        {
            Logarithms[Index] =
                Math.Log(
                    Values[Index]
                );
        }

        return Logarithms;
    }

    private static void ValidateAltitude(
        double Altitude)
    {
        if (!double.IsFinite(Altitude))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Altitude),
                "L'altitude doit être finie."
            );
        }

        if (Altitude < MinimumSupportedAltitude ||
            Altitude > MaximumSupportedAltitude)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Altitude),
                $"L'altitude doit être comprise entre " +
                $"{MinimumSupportedAltitude} m et " +
                $"{MaximumSupportedAltitude} m."
            );
        }
    }
}