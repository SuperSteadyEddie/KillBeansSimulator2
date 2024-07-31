using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Photon.Pun;
using TMPro;

public class Adrenaline : MonoBehaviourPun
{
    [Range(1, 10)] public float adrenalineSpeedMultiplier;
    [Range(1, 2.5f)] public float adrenalineJumpMultiplier;
    [Range(1, 2.5f)] public float adrenalineReloadSpeedMultiplier;
    [Range(0.1f, 1f)] public float adrenalineJumpCostMultiplier;
    [Range(1, 25)] public float adrenalineStaminaGainRateMultiplier;
    [Range(0, 1)] public float adrenalineStaminaDrainRateMultiplier;
    [Range(1, 120)] public float maxAdrenalineTime;
    [Range(1, 120)] public float adrenalineCoolDown;
    [Range(0, 5)] public int stims;
    public bool adrenalineCanKill;

    [Header("UI")]
    public GameObject stimModel;
    public GameObject attackPoint;
    public TMP_Text STIMTEXT;
    GameObject projectile;
    Rigidbody projectileRb;
    public float stimThrowForce;
    public float stimUpwardsThrowForce;

    public Transform cam;
    public GunScript gunScript;
    MovePlayer movePlayer;
    Health health;
    PhotonView pv;
    HUD hud;

    [HideInInspector] public bool adrenalineActive = false;
    bool canStim = true;
    float timeleftuntilstim;
    float adrenalineTime;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            pv = gameObject.GetComponent<PhotonView>(); 
            hud = GetComponent<HUD>();
            movePlayer = GetComponent<MovePlayer>();
            health = GetComponent<Health>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && photonView.IsMine)
        {
            if (adrenalineActive)
            {
                RemoveAdrenaline();
            }
            else if (canStim) 
            {
                AddAdrenaline();
            }
        }
    }


    public void AddAdrenaline()
    {
        if (stims > 0)
        {
            hud.StimEffect(true);
            projectile = PhotonNetwork.Instantiate(stimModel.name, attackPoint.transform.position, cam.rotation);
            photonView.RPC("ParentAdrenaline", RpcTarget.All, projectile.GetComponent<PhotonView>().ViewID, photonView.ViewID, true);
            movePlayer.currentJumpForceMultiplier *= adrenalineJumpMultiplier;
            movePlayer.currentJumpCostMultiplier *= adrenalineJumpCostMultiplier;
            movePlayer.currentSpeedMultiplier *= adrenalineSpeedMultiplier;
            gunScript.reloadSpeedMultiplier *= adrenalineReloadSpeedMultiplier;

            adrenalineActive = true;
            stims -= 1;
            StartCoroutine(AdrenalineUse());
        }
    }

    public void RemoveAdrenaline()
    {
        if (adrenalineTime < 0.1f * maxAdrenalineTime)
        {
            timeleftuntilstim = adrenalineCoolDown * 0.75f;
        }
        else
        {
            timeleftuntilstim = adrenalineCoolDown;
        }
        canStim = false;
        hud.StimEffect(false);
        photonView.RPC("ParentAdrenaline", RpcTarget.All,projectile.GetComponent<PhotonView>().ViewID,photonView.ViewID, false);
        Vector3 forceToAdd = cam.transform.forward * stimThrowForce + transform.up * stimUpwardsThrowForce;
        if (projectile != null)
        {
            projectileRb = projectile.gameObject.AddComponent<Rigidbody>();
            projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
            projectile.layer = 7;
        }
        movePlayer.currentJumpForceMultiplier /= adrenalineJumpMultiplier;
        movePlayer.currentJumpCostMultiplier /= adrenalineJumpCostMultiplier;
        movePlayer.currentSpeedMultiplier /= adrenalineSpeedMultiplier;
        gunScript.reloadSpeedMultiplier /= adrenalineReloadSpeedMultiplier;
        adrenalineActive = false;
        StartCoroutine(AdrenalineCooldown());
    }

    [PunRPC]
    public void ParentAdrenaline(int projectileid, int parentid,bool parent)
    {
        if (parent) 
        { 
            PhotonView.Find(projectileid).transform.parent = PhotonView.Find(parentid).transform.Find("HOLDS EVERYTHING");
        }
        else
        {
            PhotonView.Find(projectileid).transform.parent = null;
        }
    }


    private IEnumerator AdrenalineCooldown()
    {
        STIMTEXT.text = "";
        STIMTEXT.gameObject.SetActive(true);
        while (timeleftuntilstim > 0)
        {
            yield return new WaitForSeconds(adrenalineCoolDown / 100f);
            timeleftuntilstim -= adrenalineCoolDown / 100f;
            STIMTEXT.text = ("" + Math.Round(timeleftuntilstim, 2));
        }
        canStim = true;
        STIMTEXT.text = "STIM AVAILABLE";
        yield return new WaitForSeconds(2);
        STIMTEXT.gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator AdrenalineUse()
    {
        Debug.Log("Started");
        adrenalineTime = maxAdrenalineTime;
        while (adrenalineActive)
        {
            if (adrenalineTime < 0)
            {
                if (adrenalineCanKill)
                {
                    RemoveAdrenaline();
                    health.currentHealth = 0;
                }
                RemoveAdrenaline();
            }
            adrenalineTime -= maxAdrenalineTime / 100f;
            hud.UpdateAdrenalineBar(adrenalineTime, maxAdrenalineTime);
            yield return new WaitForSeconds(maxAdrenalineTime / 100f);
        }
        adrenalineTime = 0;
        yield return null;
    }
}
