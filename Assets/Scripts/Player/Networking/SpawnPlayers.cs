using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Realtime;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject spawnBoxPos;
    // Start is called before the first frame update
    public void spawnPlayer()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnBoxPos.transform.position, Quaternion.identity);
        player.GetComponent<playerSetup>().spawnInBox();
        print("done");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        spawnPlayer();
    }
}
