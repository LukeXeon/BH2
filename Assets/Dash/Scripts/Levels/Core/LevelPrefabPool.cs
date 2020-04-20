using System;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dash.Scripts.Levels.Core
{
    public class LevelPrefabPool : IPunPrefabPool
    {
        public string GetPlayerKey()
        {
            manager.TryGetTarget(out var m);
            return (m == null ? null : m)?.playerKey;
        }

        public readonly WeakReference<LevelPrefabManager> manager = new WeakReference<LevelPrefabManager>(null);

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            manager.TryGetTarget(out var m);
            var item = (m == null ? null : m)?.prefabs?.Find(i => i.Item1 == prefabId)
                       ?? throw new UnityException(
                           "DefaultPool failed to load \"" + prefabId +
                           "\" . Make sure it's in a \"Resources\" folder.");
            var res = item.Item2;
            bool wasActive = res.activeSelf;
            if (wasActive) res.SetActive(false);

            GameObject instance = Object.Instantiate(res, position, rotation);

            if (wasActive) res.SetActive(true);
            return instance;
        }

        public void Destroy(GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }
    }
}