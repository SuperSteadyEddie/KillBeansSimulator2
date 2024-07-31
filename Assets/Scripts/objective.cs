using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System;
using Unity.VisualScripting;
using TMPro;

public class objective : MonoBehaviourPunCallbacks
{
    public int ScoreToWin;
    public int scoreAddAmount;
    bool alreadyWaiting = false;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<playerSetup>())
        {
            if (alreadyWaiting == false)
            {
                StartCoroutine(waitBeforeAddingScore(1, collision.gameObject.GetComponent<PhotonView>().Owner));
            }
        }
    }

    IEnumerator waitBeforeAddingScore(int time, Player p)
    {
        alreadyWaiting = true;
        yield return new WaitForSeconds(time);
        AddScore(p, scoreAddAmount);
        alreadyWaiting = false;
    }

    void AddScore(Player p,int amount)
    {
        p.AddScore(amount);
        print("Added " + amount);
        print(p.NickName + " Score: " + p.GetScore());
    }

    private void Update()
    {
        checkScore();
    }
    void checkScore()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetScore() >= ScoreToWin) 
            {
                Win(player.NickName);
                ResetScores();
            }
        }
    }

    void Win(string playerName)
    {
        Debug.Log(playerName + " wins!");
    }

    private void ResetScores()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);
        }
    }

}
