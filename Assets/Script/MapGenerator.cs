using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Vector2 mapSize;

    [Range(0, 1)]
    public float outlinePercent;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
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
                Vector3 tilePosition = new Vector3(-mapSize.x/2 + 0.5f + x, 0, -mapSize.y/2 + 0.5f + y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }
    }
}
