using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public GameObject chunkPrefab;
    public Material terrainMaterial;
    public int viewDistance = 4;

    private Transform player;
    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int lastPlayerChunk;

    void Start()
    {
        player = Camera.main.transform;
        lastPlayerChunk = new Vector2Int(int.MaxValue, int.MaxValue);
    }

    void Update()
    {
        Vector2Int currentChunk = GetChunkCoord(player.position);

        if (currentChunk != lastPlayerChunk)
        {
            lastPlayerChunk = currentChunk;
            UpdateChunks(currentChunk);
        }
    }

    Vector2Int GetChunkCoord(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / Chunk.ChunkSize),
            Mathf.FloorToInt(pos.z / Chunk.ChunkSize)
        );
    }

    void UpdateChunks(Vector2Int center)
    {
        HashSet<Vector2Int> neededChunks = new HashSet<Vector2Int>();

        // Determine which chunks should exist
        for (int x = -viewDistance; x <= viewDistance; x++)
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2Int coord = new Vector2Int(center.x + x, center.y + z);
                neededChunks.Add(coord);

                if (!activeChunks.ContainsKey(coord))
                    SpawnChunk(coord);
            }

        // Remove chunks that are too far
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunks)
        {
            if (!neededChunks.Contains(kvp.Key))
                toRemove.Add(kvp.Key);
        }

        foreach (var coord in toRemove)
        {
            Destroy(activeChunks[coord]);
            activeChunks.Remove(coord);
        }
    }

    void SpawnChunk(Vector2Int coord)
    {
        Vector3 position = new Vector3(
            coord.x * Chunk.ChunkSize,
            0,
            coord.y * Chunk.ChunkSize
        );

        GameObject chunkObj = Instantiate(chunkPrefab, position, Quaternion.identity);
        Chunk chunk = chunkObj.GetComponent<Chunk>();
        chunk.terrainMaterial = terrainMaterial;
        activeChunks[coord] = chunkObj;
    }
}