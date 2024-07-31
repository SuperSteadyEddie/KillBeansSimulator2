using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OnPlayerJoin : MonoBehaviourPunCallbacks
{
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        photonView.RPC("setNickName", newPlayer, gameObject.GetComponent<PhotonView>().ViewID, gameObject.GetComponent<playerSetup>().nickname);
    }
}
