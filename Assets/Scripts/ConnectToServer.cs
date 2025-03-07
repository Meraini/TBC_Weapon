using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("StartSS");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
        Debug.Log("JoinSS");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined");
        SceneManager.LoadScene("Lobby");
    }

}
