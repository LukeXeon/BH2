using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.Levels.Core
{
    public class PunPool : IPunPrefabPool
    {
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            return ObjectPool.GlobalObtain(prefabId, position, rotation, true);
        }

        public void Destroy(GameObject gameObject)
        {
            if (!ObjectPool.TryGlobalRelease(gameObject))
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}