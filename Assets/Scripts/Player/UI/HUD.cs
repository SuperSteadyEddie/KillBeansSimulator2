using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class HUD : MonoBehaviourPun
{

    //adrenaline
    public Image adrenalinebar;
    public GameObject adrenalinebarBG;
    public GameObject vignette;
    public AudioSource audioSource;
    public AudioClip hitMarkerSound;
    public GameObject hitMarker;
    public MoveCamera moveCamera;
    public GameObject settingsScreen;
    public GameObject scoreBoardScreen;

    public TMP_Text scoreBoardPlayerText;
    public TMP_Text scoreBoardScoreText;

    string scoreBoardPlayerString;
    string scoreBoardScoreString;

    float x = 0;
    bool settingsOpen = false;
    bool scoreboardOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        adrenalinebarBG.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (!settingsOpen) 
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                moveCamera.enabled = false;
                settingsScreen.SetActive(true);
                settingsOpen = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                moveCamera.enabled = true;
                settingsScreen.SetActive(false);
                settingsOpen = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboardOpen = !scoreboardOpen;
            scoreBoardScreen.SetActive(scoreboardOpen);
        }
        UpdateScoreBoard();
    }

    [PunRPC]
    public void UpdateScoreBoard()
    {
        scoreBoardPlayerString = "Players: ";
        scoreBoardScoreString = "Scores: ";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            scoreBoardPlayerString += "\n" + player.NickName;
            scoreBoardScoreString += "\n" + player.GetScore();
        }
        scoreBoardScoreText.text = scoreBoardScoreString;
        scoreBoardPlayerText.text = scoreBoardPlayerString;
    }

    public void hit()
    {
        x = 0;
        audioSource.PlayOneShot(hitMarkerSound);
        hitMarker.SetActive(true);
        StartCoroutine(hitMarkerDespawn());
    }

    IEnumerator hitMarkerDespawn()
    {
        while (x < .25f)
        {
            yield return new WaitForSeconds(.25f / 100f);
            x += .25f / 100f;
        }
        hitMarker.SetActive(false);
    }

    public void StimEffect(bool activate)
    {
        if (activate)
        {
            adrenalinebarBG.SetActive(true);
            vignette.SetActive(true);
        }
        else
        {
            adrenalinebarBG.SetActive(false);
            vignette.SetActive(false);
        }
    }



    public void UpdateAdrenalineBar(float currentTime,float maxTime)
    {
        adrenalinebar.fillAmount = currentTime / maxTime;
    }

}
