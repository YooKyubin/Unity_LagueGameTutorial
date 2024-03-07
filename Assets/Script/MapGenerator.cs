using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0, 1)]
    public float outlinePercent;
    [Range (0, 1)]
    public float obstaclePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    public int seed = 10;
    Coord mapCenter;

    Coord[] dirs = { new Coord(0, 1), new Coord(1, 0), new Coord(-1, 0), new Coord(0,-1) };

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCenter = new Coord ( (int)mapSize.x / 2, (int)mapSize.y / 2 );

        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
            // strongly recommanded Destroy 
            // Destroy는 해당 프레임이 끝난 뒤 파괴,
            // DestroyImmediate은 즉시 파괴 -> 해당 오브젝트를 같은 프레임에 참조하면 null ref 오류가 발생할 수 있음
            // 하지만 Editor 코드에서 동작할 것이기 때문에 DestroyImmediate 사용
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        for (int i=0; i<obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            ++currentObstacleCount;

            if (randomCoord != mapCenter && MapIsFullyAccessbile(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * .5f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                --currentObstacleCount;
            }
        }
    }

    bool MapIsFullyAccessbile(bool[,] obstacleMap, int currentObstacleCount)
    {
        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        bool[,] visited = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(mapCenter);
        visited[mapCenter.x, mapCenter.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord curTile = queue.Dequeue();

            foreach (Coord d in dirs)
            {
                int nx = curTile.x + d.x;
                int ny = curTile.y + d.y;

                // OOB
                if (nx < 0 || ny < 0 || nx >= obstacleMap.GetLength(0) || ny >= obstacleMap.GetLength(1))
                    continue;

                if (visited[nx, ny])
                    continue;

                if (obstacleMap[nx, ny])
                    continue;

                visited[nx, ny] = true;
                queue.Enqueue(new Coord(nx, ny));
                accessibleTileCount++;
            }
        }

        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x; 
            y = _y;
        }

        public static bool operator ==(Coord a, Coord b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Coord a, Coord b)
        {
            return !(a == b);
        }
    }
}
