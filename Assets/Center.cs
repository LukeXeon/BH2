using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Center : MonoBehaviour
{
    public void Test()
    {
        Task.Run((() => Thread.Sleep(10000)))
            .ContinueWith((t) => Debug.Log("1111"), TaskScheduler.FromCurrentSynchronizationContext());
    }
    
}