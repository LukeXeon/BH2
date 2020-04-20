using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Dash.Scripts.Levels.Core
{
    public class LevelPrefabManager : MonoBehaviour
    {
        public GameObject[] npcPrefabs;
        public List<Tuple<string, GameObject>> prefabs;

        private void Awake()
        {
            var pool = (LevelPrefabPool) PhotonNetwork.PrefabPool;
            prefabs = new List<Tuple<string, GameObject>>();
            foreach (var o in npcPrefabs)
            {
                prefabs.Add(Tuple.Create(o.name, o));
            }
            pool.manager.SetTarget(this);
        }
    }

    public static class LevelPrefabs
    {
        public static string GetKey(this GameObject gameObject)
        {
            var pool = (LevelPrefabPool) PhotonNetwork.PrefabPool;
            pool.manager.TryGetTarget(out var m);
            return (m == null ? null : m)?.prefabs.Find(i => i.Item2 == gameObject).Item1;
        }
    }
}