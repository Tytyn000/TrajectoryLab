namespace TrajectoryLab.Core.Mathematics;

/// Vecteur tridimensionnel utilisant des nombres à double précision.
public readonly record struct Vector3D(
    double X,
    double Y,
    double Z)
{
    public static Vector3D Zero => new(0.0, 0.0, 0.0);

    public double Length()
    {
        return Math.Sqrt(LengthSquared());
    }

    public double LengthSquared()
    {
        return X * X + Y * Y + Z * Z;
    }

    public Vector3D Normalized()
    {
        double LengthValue = Length();

        if (LengthValue == 0.0)
        {
            return Zero;
        }

        return this / LengthValue;
    }

    public static double Dot(Vector3D Left, Vector3D Right)
    {
        return
            Left.X * Right.X +
            Left.Y * Right.Y +
            Left.Z * Right.Z;
    }

    public static Vector3D Cross(Vector3D Left, Vector3D Right)
    {
        return new Vector3D(
            Left.Y * Right.Z - Left.Z * Right.Y,
            Left.Z * Right.X - Left.X * Right.Z,
            Left.X * Right.Y - Left.Y * Right.X
        );
    }

    public static Vector3D operator +(
        Vector3D Left,
        Vector3D Right)
    {
        return new Vector3D(
            Left.X + Right.X,
            Left.Y + Right.Y,
            Left.Z + Right.Z
        );
    }

    public static Vector3D operator -(
        Vector3D Left,
        Vector3D Right)
    {
        return new Vector3D(
            Left.X - Right.X,
            Left.Y - Right.Y,
            Left.Z - Right.Z
        );
    }

    public static Vector3D operator -(Vector3D Value)
    {
        return new Vector3D(
            -Value.X,
            -Value.Y,
            -Value.Z
        );
    }

    public static Vector3D operator *(
        Vector3D Vector,
        double Scalar)
    {
        return new Vector3D(
            Vector.X * Scalar,
            Vector.Y * Scalar,
            Vector.Z * Scalar
        );
    }

    public static Vector3D operator *(
        double Scalar,
        Vector3D Vector)
    {
        return Vector * Scalar;
    }

    public static Vector3D operator /(
        Vector3D Vector,
        double Scalar)
    {
        if (Scalar == 0.0)
        {
            throw new DivideByZeroException(
                "Un vecteur ne peut pas être divisé par zéro."
            );
        }

        return new Vector3D(
            Vector.X / Scalar,
            Vector.Y / Scalar,
            Vector.Z / Scalar
        );
    }
}