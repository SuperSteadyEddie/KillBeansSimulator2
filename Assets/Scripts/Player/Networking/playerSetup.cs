using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;

public class playerSetup : MonoBehaviourPun
{
    public MovePlayer movePlayer;
    public GunScript gunScript;
    public PlayerStamina playerStamina;
    public MoveCamera moveCamera;
    public HUD hud;
    public Health health;
    public ThrowingStuff throwingStuff;
    public GameObject canvas;
    public GameObject cam;
    public GameObject vfx;
    public GameObject[] bodyparts;
    public GameObject GunMenu;
    public ThrowGrenade tg;
    public TMP_Text NameTag;

    public string nickname;
    bool inBox = true;

    private void Start()
    {
        photonView.Owner.TagObject = this;
    }

    public void spawnInMap()
    {
        if (photonView.IsMine)
        {
            inBox = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            nickname = PhotonNetwork.NickName;
            hud.enabled = true;
            gunScript.enabled = true;
            throwingStuff.enabled = true;
            moveCamera.enabled = true;
            movePlayer.enabled = true;
            tg.enabled = true;
            playerStamina.enabled = true;
            health.enabled = true;
            canvas.SetActive(true);
            cam.GetComponent<Camera>().enabled = true;
            cam.GetComponent<AudioListener>().enabled = true;
            photonView.RPC("setNickName", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID, nickname);
            StartCoroutine(Spawn(.1f));
            print("Spawned!");
        }
    }

    IEnumerator Spawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 spawnPosition = new Vector3(Random.RandomRange(50f, -50f), 30f, Random.RandomRange(50f, -50f));
        gameObject.GetComponent<Rigidbody>().position = spawnPosition;
    }

    private void Update()
    {
        if(inBox && photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                spawnInMap();
            }
        }
    }

    public void spawnInBox()
    {
        if (photonView.IsMine)
        {
            inBox = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            nickname = PhotonNetwork.NickName;
            foreach (GameObject g in bodyparts)
            {
                g.layer = 6;
            }
            vfx.SetActive(true);
            cam.GetComponent<Camera>().enabled = true;
            cam.GetComponent<AudioListener>().enabled = true;
            photonView.RPC("setNickName", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID, nickname);
        }
    }

    [PunRPC]
    public void setNickName(int viewID,string name)
    {
        PhotonView.Find(viewID).GetComponent<playerSetup>().NameTag.text = name;
    }
}
