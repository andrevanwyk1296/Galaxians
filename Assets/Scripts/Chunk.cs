using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    public const int ChunkSize = 16;
    public Material terrainMaterial;

    private BlockType[,,] blocks = new BlockType[ChunkSize, ChunkSize, ChunkSize];
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // Toggle: true = use MeshCollider, false = use BoxCollider
    public bool useMeshCollider = true;

    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        GenerateTerrain();
        BuildMesh();

        meshRenderer.sharedMaterials = new Material[] { terrainMaterial };
    }

    void GenerateTerrain()
    {
        Vector3 worldPos = transform.position;

        for (int x = 0; x < ChunkSize; x++)
            for (int z = 0; z < ChunkSize; z++)
            {
                float noiseX = (worldPos.x + x) * 0.05f;
                float noiseZ = (worldPos.z + z) * 0.05f;
                int surfaceHeight = Mathf.RoundToInt(Mathf.PerlinNoise(noiseX, noiseZ) * 8) + 6;

                for (int y = 0; y < ChunkSize; y++)
                {
                    if (y < surfaceHeight - 2)
                        blocks[x, y, z] = BlockType.Stone;
                    else if (y < surfaceHeight)
                        blocks[x, y, z] = BlockType.Dirt;
                    else if (y == surfaceHeight)
                        blocks[x, y, z] = BlockType.Grass;
                    else
                        blocks[x, y, z] = BlockType.Air;
                }
            }
    }

    void BuildMesh()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        for (int x = 0; x < ChunkSize; x++)
            for (int y = 0; y < ChunkSize; y++)
                for (int z = 0; z < ChunkSize; z++)
                {
                    if (blocks[x, y, z] == BlockType.Air) continue;
                    AddBlockFaces(x, y, z, vertices, triangles);
                }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.subMeshCount = 1;
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;

        // Collider setup
        if (useMeshCollider)
        {
            MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
                meshCollider = gameObject.AddComponent<MeshCollider>();

            meshCollider.sharedMesh = null; // force refresh
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = false;    // keep non-convex for terrain
            Debug.Log("MeshCollider bounds: " + meshCollider.bounds);
        }
        else
        {
            // Replace MeshCollider with BoxCollider for testing
            MeshCollider existing = GetComponent<MeshCollider>();
            if (existing != null) DestroyImmediate(existing);

            BoxCollider box = gameObject.GetComponent<BoxCollider>();
            if (box == null) box = gameObject.AddComponent<BoxCollider>();

            box.center = new Vector3(ChunkSize / 2f, ChunkSize / 2f, ChunkSize / 2f);
            box.size = new Vector3(ChunkSize, ChunkSize, ChunkSize);
            Debug.Log("BoxCollider bounds: " + box.bounds);
        }
    }

    void AddBlockFaces(int x, int y, int z,
        List<Vector3> verts,
        List<int> tris)
    {
        if (y + 1 >= ChunkSize || blocks[x, y + 1, z] == BlockType.Air)
            AddFace(verts, tris,
                new Vector3(x, y + 1, z),
                new Vector3(x, y + 1, z + 1),
                new Vector3(x + 1, y + 1, z + 1),
                new Vector3(x + 1, y + 1, z));

        if (y - 1 < 0 || blocks[x, y - 1, z] == BlockType.Air)
            AddFace(verts, tris,
                new Vector3(x, y, z + 1),
                new Vector3(x, y, z),
                new Vector3(x + 1, y, z),
                new Vector3(x + 1, y, z + 1));

        if (z + 1 >= ChunkSize || blocks[x, y, z + 1] == BlockType.Air)
            AddFace(verts, tris,
                new Vector3(x, y, z + 1),
                new Vector3(x, y + 1, z + 1),
                new Vector3(x + 1, y + 1, z + 1),
                new Vector3(x + 1, y, z + 1));

        if (z - 1 < 0 || blocks[x, y, z - 1] == BlockType.Air)
            AddFace(verts, tris,
                new Vector3(x + 1, y, z),
                new Vector3(x + 1, y + 1, z),
                new Vector3(x, y + 1, z),
                new Vector3(x, y, z));

        if (x + 1 >= ChunkSize || blocks[x + 1, y, z] == BlockType.Air)
            AddFace(verts, tris,
                new Vector3(x + 1, y, z),
                new Vector3(x + 1, y + 1, z),
                new Vector3(x + 1, y + 1, z + 1),
                new Vector3(x + 1, y, z + 1));

        if (x - 1 < 0 || blocks[x - 1, y, z] == BlockType.Air)
            AddFace(verts, tris,
                new Vector3(x, y, z + 1),
                new Vector3(x, y + 1, z + 1),
                new Vector3(x, y + 1, z),
                new Vector3(x, y, z));
    }

    void AddFace(
        List<Vector3> verts,
        List<int> tris,
        Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        int i = verts.Count;
        verts.Add(a); verts.Add(b); verts.Add(c); verts.Add(d);
        tris.Add(i); tris.Add(i + 1); tris.Add(i + 2);
        tris.Add(i); tris.Add(i + 2); tris.Add(i + 3);
    }
}
