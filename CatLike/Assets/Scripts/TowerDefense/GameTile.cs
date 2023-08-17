using UnityEngine;

public class GameTile : MonoBehaviour
{
    static Quaternion
        northQuaternion = Quaternion.Euler(90f, 0f, 0f),
        eastQuaternion = Quaternion.Euler(90f, 90f, 0f),
        southQuaternion = Quaternion.Euler(90f, 180f, 0f),
        westQuaternion = Quaternion.Euler(90f, 270f, 0f);

    [SerializeField]
    Transform arrow = default;

    GameTile north, east, south, west, nextOnPath;

    int distance;

    GameTileContent content;

    public bool HasPath => distance != int.MaxValue;

    public bool IsAlternative { get; set; }

    public GameTile NextOnPath => nextOnPath;

    public Vector3 ExitPoint { get; private set; }

    public Direction PathDirection { get; private set; }

    public GameTileContent Content
    {
        get => content;
        set
        {
            Debug.Assert(value != null, "Null assigned to content");
            if (content != null)
            {
                content.Recycle();
            }

            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        Debug.Assert(west.east == null && east.west == null, "Redifined neighbors");

        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        Debug.Assert(south.north == null && north.south == null, "Redifined neighbors");

        south.north = north;
        north.south = south;
    }

    public void ShowPath()
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northQuaternion :
            nextOnPath == east ? eastQuaternion :
            nextOnPath == south ? southQuaternion :
            westQuaternion;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }

    public GameTile GrowPathToNorth() => GrowPathTo(north, Direction.South);

    public GameTile GrowPathToSorth() => GrowPathTo(south, Direction.North);

    public GameTile GrowPathToWest() => GrowPathTo(west, Direction.East);

    public GameTile GrowPathToEast() => GrowPathTo(east, Direction.West);


    GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        Debug.Assert(HasPath, "no path");
        if(neighbor == null || neighbor.HasPath)
        {
            return null;
        }

        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = (neighbor.transform.localPosition + transform.localPosition) * 0.5f;
        neighbor.PathDirection = direction;
        return neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null;
    }
}
