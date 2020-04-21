using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class particleColorChanger : MonoBehaviour
{

    [System.Serializable]
    public struct colorChange
    {
        public string Name;
        public ParticleSystem[] colored_ParticleSystem;
        public Gradient Gradient_custom;
    }

    public colorChange[] colorChangeList;

    public bool applyChanges = false;
    public bool Keep_applyChanges = false;

    void Update()
    {
        if (applyChanges || Keep_applyChanges)
        {
            foreach (var t in colorChangeList)
            {
                foreach (var t1 in t.colored_ParticleSystem)
                {
                    var col = t1.colorOverLifetime;
                    col.color = t.Gradient_custom;
                }
            }

            applyChanges = false;
        }
    }
}
