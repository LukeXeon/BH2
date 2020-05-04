using System.Collections.Generic;
using UnityEngine;

namespace Dash.Scripts.GamePlay.View
{
    public class AudioView : MonoBehaviour
    {
        private LinkedList<AudioSource> inUseSources;
        private Stack<AudioSource> cacheSource;

        private void Awake()
        {
            inUseSources = new LinkedList<AudioSource>();
            cacheSource = new Stack<AudioSource>();
            var s = gameObject.AddComponent<AudioSource>();
            cacheSource.Push(s);
        }

        private void Update()
        {
            for (var it = inUseSources.First; it != null; it = it?.Next)
            {
                var source = it.Value;
                if (!source.isPlaying || source.time >= ((source.clip == null ? null : source.clip)?.length ?? 0))
                {
                    source.clip = null;
                    source.Stop();
                    cacheSource.Push(source);
                    inUseSources.Remove(it);
                    it = it.Next;
                }
            }
        }

        public static AudioView Create(Transform root)
        {
            var go = new GameObject("AudioSources");
            go.transform.SetParent(root);
            go.transform.localPosition = Vector3.zero;
            return go.AddComponent<AudioView>();
        }

        public AudioSource GetOrCreateSource()
        {
            AudioSource s;
            if (cacheSource.Count == 0)
            {
                s = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                s = cacheSource.Pop();
            }

            inUseSources.AddLast(s);
            return s;
        }
    }
}