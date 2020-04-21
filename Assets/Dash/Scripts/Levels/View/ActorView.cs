using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.Levels.View
{
    public abstract class ActorView : MonoBehaviour
    {


        [PunRPC]
        public virtual void OnDamage(int value)
        {

        }
    }
}