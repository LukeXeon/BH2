using System;
using System.Collections.Generic;
using Dash.Scripts.Core;
using UnityEngine;

namespace Dash.Scripts.GamePlay.Core
{
    public sealed class ObjectPool : MonoBehaviour
    {
        private static readonly LinkedList<ObjectPool> pools = new LinkedList<ObjectPool>();

        private Dictionary<string, CacheItem> caches;


        private Transform inactivePoolRoot;


        public PrefabItem[] prefabs;

        public static GameObject GlobalObtain(string guid, Vector3 position, Quaternion rotation, bool inactive)
        {
            foreach (var objectPool in pools)
            {
                var go = objectPool.Obtain(guid, position, rotation, inactive);
                if (go != null) return go;
            }

            return null;
        }

        public static bool TryGlobalRelease(GameObject go)
        {
            foreach (var objectPool in pools)
                if (objectPool.TryRelease(go))
                    return true;

            return false;
        }

        private void Awake()
        {
            pools.AddFirst(this);
            caches = new Dictionary<string, CacheItem>();
            var go = new GameObject("PoolRoot") {hideFlags = HideFlags.NotEditable};
            go.SetActive(false);
            inactivePoolRoot = go.transform;
            inactivePoolRoot.SetParent(transform);
            if (prefabs != null && prefabs.Length > 0)
                foreach (var item in prefabs)
                {

                    var guid = item.prefab.guid;
                    var count = item.preloadAmount;
                    if (count <= 0)
                    {
                        caches.Add(guid, new CacheItem {prefabItem = item});
                        continue;
                    }

                    var stack = new Stack<GameObject>();
                    var a = item.prefab.gameObject.activeSelf;
                    item.prefab.gameObject.SetActive(false);
                    for (var i = 0; i < count; i++)
                    {
                        go = Instantiate(item.prefab.gameObject, inactivePoolRoot);
                        go.name = item.prefab.gameObject.name + $"@({i})";
                        stack.Push(go);
                    }

                    item.prefab.gameObject.SetActive(a);
                    caches.Add(guid, new CacheItem {prefabItem = item, allocCount = count, cache = stack});
                }
        }

        private void OnDestroy()
        {
            pools.Remove(this);
        }

        public GameObject Obtain(string guid, Vector3 position, Quaternion rotation, bool inactive)
        {
            caches.TryGetValue(guid, out var cache);
            if (cache == null) return null;

            if (cache.cache != null && cache.cache.Count > 0)
            {
                var go = cache.cache.Pop();
                go.SetActive(!inactive);
                go.transform.SetParent(null);
                go.transform.position = position;
                go.transform.rotation = rotation;
                ++cache.allocCount;
                go.GetComponent<IPoolLifecycle>()?.Reusing();
                return go;
            }

            var pgo = cache.prefabItem.prefab.gameObject;
            var a = pgo.activeSelf;
            pgo.SetActive(!inactive);
            var go2 = Instantiate(pgo, position, rotation);
            pgo.SetActive(a);
            go2.name = cache.prefabItem.prefab.gameObject.name + $"@({cache.allocCount++})";
            return go2;
        }

        public void Release(GameObject go)
        {
            if (!TryRelease(go)) Destroy(go);
        }

        public bool TryRelease(GameObject go)
        {
            var com = go.GetComponent<GuidIndexer>();
            if (com == null) return false;

            var guid = com.guid;
            caches.TryGetValue(guid, out var cache);
            if (cache == null) return false;

            if (cache.cache == null)
            {
                cache.cache = new Stack<GameObject>();
                caches[guid] = cache;
            }

            if (cache.cache.Count < cache.prefabItem.maxAmount)
            {
                go.GetComponent<IPoolLifecycle>()?.Recycle();
                go.transform.SetParent(inactivePoolRoot);
                cache.cache.Push(go);
                return true;
            }

            return false;
        }

        [Serializable]
        public struct PrefabItem
        {
            public GuidIndexer prefab;
            public int preloadAmount;
            public int maxAmount;
        }

        private class CacheItem
        {
            public int allocCount;
            public Stack<GameObject> cache;
            public PrefabItem prefabItem;
        }
    }
}