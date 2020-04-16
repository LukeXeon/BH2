using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TestNetwork : MonoBehaviour
{
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = PhotonNetwork.NetworkingClient.State.ToString();
    }
}
