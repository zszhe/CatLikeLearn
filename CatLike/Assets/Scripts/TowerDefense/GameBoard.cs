using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    bool showPaths, showGrids;

    [SerializeField]
    Transform ground = default;

    [SerializeField]
    GameTile tilePrefab = default;

    [SerializeField]
    Texture2D gridTexture = default;

    Vector2Int size;

    GameTile[] tiles;

    GameTileContentFactory contentFactory;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    List<GameTile> spawnPoints = new List<GameTile>();

    public bool ShowPaths
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (var tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach(var tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }

    public bool ShowGrids
    {
        get => showGrids;
        set
        {
            showGrids = value;
            Material mat = ground.GetComponent<MeshRenderer>().material;
            if (showGrids)
            {
                mat.mainTexture = gridTexture;
                mat.SetTextureScale("_MainTex", size);
            }
            else
            {
                mat.mainTexture = null;
            }
        }
    }

    public int SpawnPointsCount => spawnPoints.Count;

    public void Initialize(Vector2Int size, GameTileContentFactory contentFactory)
    {
        this.size = size;
        this.contentFactory = contentFactory;
        ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        tiles = new GameTile[size.x * size.y];
        for (int y = 0, i = 0; y < size.y; y++)
        {
            for(int x = 0;x < size.x; x++, i++)
            {
                GameTile tile = tiles[i] = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0, y - offset.y);
                if(x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tiles[i], tiles[i - 1]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tiles[i], tiles[i - size.x]);
                }

                tile.IsAlternative = (x & 1) != 0;
                if ((y & 1) != 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }

                tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }

        ToggleDestination(tiles[tiles.Length / 2]);
        ToggleSpawnPoint(tiles[0]);
    }

    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return tiles[x + y * size.x];
            }
        }

        return null;
    }

    public GameTile GetSpawnPoint(int index)
    {
        if (index < spawnPoints.Count)
        {
            return spawnPoints[index];
        }

        return null;
    }

    public void ToggleDestination(GameTile tile)
    {
        if(tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Wall);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }

    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.SpawnPoint)
        {
            if(spawnPoints.Count > 1)
            {
                spawnPoints.Remove(tile);
                tile.Content = contentFactory.Get(GameTileContentType.Empty);

            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
            spawnPoints.Add(tile);
        }
    }

    bool FindPaths()
    {
        foreach (GameTile target in tiles)
        {
            if (target.Content.Type == GameTileContentType.Destination)
            {
                target.BecomeDestination();
                searchFrontier.Enqueue(target);
            }
            else
            {
                target.ClearPath();
            }
        }

        if(searchFrontier.Count == 0)
        {
            return false;
        }

        while (searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    searchFrontier.Enqueue(tile.GrowPathToNorth());
                    searchFrontier.Enqueue(tile.GrowPathToSorth());
                    searchFrontier.Enqueue(tile.GrowPathToEast());
                    searchFrontier.Enqueue(tile.GrowPathToWest());
                }
                else
                {
                    searchFrontier.Enqueue(tile.GrowPathToWest());
                    searchFrontier.Enqueue(tile.GrowPathToEast());
                    searchFrontier.Enqueue(tile.GrowPathToSorth());
                    searchFrontier.Enqueue(tile.GrowPathToNorth());
                }
            }
        }

        foreach (var target in tiles)
        {
            if (!target.HasPath)
            {
                return false;
            }
        }

        if (showPaths)
        {
            foreach (GameTile target in tiles)
            {
                target.ShowPath();
            }
        }

        return true;
    }
}
