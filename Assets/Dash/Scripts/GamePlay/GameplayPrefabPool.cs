using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Dash.Scripts.GamePlay
{
    public class GameplayPrefabPool : IPunPrefabPool
    {
        /// <summary>Returns an inactive instance of a networked GameObject, to be used by PUN.</summary>
        /// <param name="prefabId">String identifier for the networked object.</param>
        /// <param name="position">Location of the new object.</param>
        /// <param name="rotation">Rotation of the new object.</param>
        /// <returns></returns>
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            var refs = Object.FindObjectOfType<GameplayManager>().prefabRefs;
            if (refs == null)
            {
                throw new NullReferenceException("GameplayManager Not Found");
            }
            var res = Array.Find(refs, i => i.name == prefabId);
            if (!res)
            {
                Debug.LogError("DefaultPool failed to load \"" + prefabId +
                               "\" . Make sure it's in a \"Resources\" folder.");
            }
            bool wasActive = res.activeSelf;
            if (wasActive) res.SetActive(false);

            GameObject instance = GameObject.Instantiate(res, position, rotation) as GameObject;

            if (wasActive) res.SetActive(true);
            return instance;
        }

        /// <summary>Simply destroys a GameObject.</summary>
        /// <param name="gameObject">The GameObject to get rid of.</param>
        public void Destroy(GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }
    }
}