using UnityEngine;

[ExecuteInEditMode]
public class RoomEdge : MonoBehaviour
{
    public Edge edge;

    public MeshRenderer target;

    public enum Edge
    {
        TOP,
        LEFT,
        RIGHT,
        BOTTOM
    }

    private void Update()
    {
        if (target)
        {
            var bounds = target.bounds;
            var transform1 = transform;
            transform1.position =
                new Vector3(bounds.center.x + bounds.extents.x + GetComponent<MeshRenderer>().bounds.extents.x / 2f,
                    bounds.center.y, transform1.position.z);
        }
    }
}