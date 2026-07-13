namespace TrajectoryLab.Core;

public sealed class ProjectileParameters
{
    public double Mass { get; }

    public double DragCoefficient { get; }

    public double CrossSectionalArea { get; }

    public ProjectileParameters(
        double Mass,
        double DragCoefficient,
        double CrossSectionalArea)
    {
        // La masse intervient au dénominateur dans le calcul de l'accélération.
        if (!double.IsFinite(Mass) || Mass <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(Mass), "La masse doit être finie et strictement positive.");
        }

        // Un coefficient nul permet de désactiver la traînée aérodynamique.
        if (!double.IsFinite(DragCoefficient) || DragCoefficient < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(DragCoefficient), "Le coefficient de traînée doit être fini et positif ou nul.");
        }

        // Une surface nulle permet également de désactiver la traînée.
        if (!double.IsFinite(CrossSectionalArea) || CrossSectionalArea < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(CrossSectionalArea), "La surface frontale doit être finie et positive ou nulle.");
        }

        this.Mass = Mass;
        this.DragCoefficient = DragCoefficient;
        this.CrossSectionalArea = CrossSectionalArea;
    }
}