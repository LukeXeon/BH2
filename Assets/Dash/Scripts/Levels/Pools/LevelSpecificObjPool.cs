using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Dash.Scripts.Levels.Pools
{
    public class LevelSpecificObjPool : MonoBehaviour
    {
        public GameObject[] prefabs;
        private Dictionary<string, GameObject> table;
        private Dictionary<string, Stack<GameObject>> pooled;
        private Transform poolRoot;

        private void Awake()
        {
            var go = new GameObject("Holder");
            poolRoot = go.transform;
            poolRoot.SetParent(transform);
            go.SetActive(false);
            pooled = new Dictionary<string, Stack<GameObject>>();
            table = new Dictionary<string, GameObject>();
            foreach (var o in prefabs)
            {
                table.Add(o.name, o);
                var pooledInterface = go.GetComponent<IPooled>();
                if (pooledInterface != null && pooledInterface.PreAlloc > 0)
                {
                    var cache = new Stack<GameObject>();
                    for (int i = 0; i < pooledInterface.PreAlloc; i++)
                    {
                        var newInstance = Instantiate(o, poolRoot);
                        cache.Push(newInstance);
                    }

                    pooled[o.name] = cache;
                }
            }
        }

        private GameObject Alloc(string prefabId, Vector3 position, Quaternion rotation)
        {
            var res = table[prefabId];
            if (IsPooledRes(res))
            {
                pooled.TryGetValue(prefabId, out var pooledGo);
                if (pooledGo != null && pooledGo.Count > 0)
                {
                    var newInstance = pooledGo.Pop();
                    newInstance.transform.SetParent(null);
                    newInstance.transform.position = position;
                    newInstance.transform.rotation = rotation;
                    var i = newInstance.GetComponent<IPooled>();
                    i.Reusing();
                    return newInstance;
                }
            }

            bool wasActive = res.activeSelf;
            if (wasActive) res.SetActive(false);
            GameObject instance = Instantiate(res, position, rotation);
            if (wasActive) res.SetActive(true);
            return instance;
        }
        
        private static bool IsPooledRes(GameObject go)
        {
            return go.GetComponent<IPooled>() != null;
        }

        private void Release(GameObject go)
        {
            var pooledInterface = go.GetComponent<IPooled>();
            if (pooledInterface != null)
            {
                pooled.TryGetValue(go.name, out var cache);
                if (cache == null)
                {
                    cache = new Stack<GameObject>();
                    pooled[go.name] = cache;
                }

                if (cache.Count < pooledInterface.MaxPool)
                {
                    pooledInterface.Recycle();
                    go.transform.SetParent(poolRoot);
                    cache.Push(go);
                    return;
                }
            }
            Destroy(go);
        }
        
        public class PunPool : IPunPrefabPool
        {
            private LevelSpecificObjPool level;

            public PunPool()
            {
                SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            }

            private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
            {
                level = FindObjectOfType<LevelSpecificObjPool>();
            }

            public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
            {
                if (prefabId == "player")
                {
                    var res = Resources.Load<GameObject>("Prefab/Level/Player");
                    bool wasActive = res.activeSelf;
                    GameObject instance = Object.Instantiate(res, position, rotation);
                    if (wasActive) res.SetActive(false);
                    return instance;
                }

                return level ? level.Alloc(prefabId, position, rotation) : null;
            }

            public void Destroy(GameObject gameObject)
            {
                if (level)
                {
                    level.Release(gameObject);
                }
                else
                {
                    Object.Destroy(gameObject);
                }
            }
        }
    }
}