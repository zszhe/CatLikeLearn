using UnityEngine;

public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 4,
}

public enum DirectionChange
{
    None = 0,
    TurnRight = 1,
    TurnLeft = 2,
    TurnAround = 3,
}

public static class DirectionExtensions
{
    static Quaternion[] rotations = {
    Quaternion.identity,
    Quaternion.Euler(0f, 90f, 0f),
    Quaternion.Euler(0f, 180f, 0f),
    Quaternion.Euler(0f, 270f, 0f)
    };

    public static Quaternion GetRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }

    public static DirectionChange GetDirectioChangeTo(this Direction current, Direction next)
    {
        if (current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }

        return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction)
    {
        return (float)direction * 90f;
    }
}
