using System;
using System.Collections.Generic;
using Dash.Scripts.Core;
using UnityEngine;

namespace Dash.Scripts.Levels.Core
{
    public sealed class ObjectPool : MonoBehaviour
    {
        private static readonly LinkedList<ObjectPool> pools = new LinkedList<ObjectPool>();

        public static GameObject GlobalObtain(string guid, Vector3 position, Quaternion rotation)
        {
            foreach (var objectPool in pools)
            {
                
                var go = objectPool.Obtain(guid, position, rotation);
                if (go != null)
                {
                    return go;
                }
            }

            return null;
        }

        public static bool TryGlobalRelease(GameObject go)
        {
            foreach (var objectPool in pools)
            {
                if (objectPool.TryRelease(go))
                {
                    return true;
                }
            }

            return false;
        }


        public PrefabItem[] prefabs;

        [Serializable]
        public struct PrefabItem
        {
            public GuidIndexer prefab;
            public int preloadAmount;
            public int maxAmount;
        }

        private class CacheItem
        {
            public PrefabItem prefabItem;
            public Stack<GameObject> cache;
            public int allocCount;
        }


        private Transform inactive;

        private Dictionary<string, CacheItem> caches;

        private void Awake()
        {
            pools.AddFirst(this);
            caches = new Dictionary<string, CacheItem>();
            var go = new GameObject("PoolRoot") {hideFlags = HideFlags.NotEditable};
            go.SetActive(false);
            inactive = go.transform;
            inactive.SetParent(transform);
            if (prefabs != null && prefabs.Length > 0)
            {
                foreach (var item in prefabs)
                {
                    item.prefab.gameObject.SetActive(false);
                    var guid = item.prefab.guid;
                    var count = item.preloadAmount;
                    if (count <= 0)
                    {
                        caches.Add(guid, new CacheItem {prefabItem = item});
                        continue;
                    }

                    var stack = new Stack<GameObject>();
                    for (int i = 0; i < count; i++)
                    {
                        go = Instantiate(item.prefab.gameObject, inactive);
                        go.name = item.prefab.gameObject.name + $"@({i})";
                        stack.Push(go);
                    }

                    caches.Add(guid, new CacheItem {prefabItem = item, allocCount = count, cache = stack});
                }
            }
        }

        private void OnDestroy()
        {
            pools.Remove(this);
        }

        public GameObject Obtain(string guid, Vector3 position, Quaternion rotation)
        {
            caches.TryGetValue(guid, out var cache);
            if (cache == null)
            {
                return null;
            }

            if (cache.cache != null && cache.cache.Count > 0)
            {
                var go = cache.cache.Pop();
                go.transform.SetParent(null);
                go.transform.position = position;
                go.transform.rotation = rotation;
                ++cache.allocCount;
                go.GetComponent<IPoolLifecycle>()?.Reusing();
                return go;
            }

            var go2 = Instantiate(cache.prefabItem.prefab.gameObject, position, rotation);
            go2.name = cache.prefabItem.prefab.gameObject.name + $"@({cache.allocCount++})";
            return go2;
        }

        public void Release(GameObject go)
        {
            if (!TryRelease(go))
            {
                Destroy(go);
            }
        }

        public bool TryRelease(GameObject go)
        {
            var com = go.GetComponent<GuidIndexer>();
            if (com == null)
            {
                return false;
            }

            var guid = com.guid;
            caches.TryGetValue(guid, out var cache);
            if (cache == null)
            {
                return false;
            }

            if (cache.cache == null)
            {
                cache.cache = new Stack<GameObject>();
                caches[guid] = cache;
            }

            if (cache.cache.Count < cache.prefabItem.maxAmount)
            {
                go.GetComponent<IPoolLifecycle>()?.Recycle();
                go.transform.SetParent(inactive);
                cache.cache.Push(go);
                return true;
            }

            return false;
        }
    }
}