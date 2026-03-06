using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class ChunkColliderGizmo : MonoBehaviour
{
    void OnDrawGizmos()
    {
        MeshCollider mc = GetComponent<MeshCollider>();
        if (mc != null && mc.sharedMesh != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(mc.bounds.center, mc.bounds.size);
        }
    }
}
