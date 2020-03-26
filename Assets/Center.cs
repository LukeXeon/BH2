using UnityEngine;

[ExecuteInEditMode]
public class Center : MonoBehaviour
{
    public MeshRenderer target;


    private void Update()
    {
        if (target)
        {
            var bounds = target.bounds;
            var transform1 = target.transform;
            transform1.localPosition = transform.rotation * bounds.extents;
            transform1.localRotation = Quaternion.identity;
        }
    }
}