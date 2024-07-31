using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Pun.Demo.Cockpit;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class Health : MonoBehaviourPunCallbacks
{
    GameObject spawnBoxPos;
    public Image healthBar;
    public TMP_Text textHealth;
    public bool onPlayer;
    public int healthRegenCooldown;
    public float healthRegenAmount;
    public string PrefabName;
    float timeWaited = 0;
    bool takenDamageRecently = false;
    bool cooldownActivated = false;
    bool damageTaken = false;
    [HideInInspector]public bool dead = false;
    public GameObject[] bodyParts;

    [Header("Health")]
    [Range(1,100000)] public float playerMaxHealth = 100;
    [HideInInspector] public float currentHealth;
    

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = playerMaxHealth;
        healthBar.fillAmount = currentHealth / playerMaxHealth;
        spawnBoxPos = GameObject.Find("Map/SpawnBox/SpawnPos");
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            if (currentHealth <= 0 || Input.GetKeyDown(KeyCode.F5) && onPlayer)
            {
                Die();
                Respawn();
            }
            if (takenDamageRecently == false && currentHealth < playerMaxHealth)
            {
                Heal(healthRegenAmount * Time.deltaTime);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if(!onPlayer)
        {
            photonView.RPC("UpdateHealthBar", newPlayer, photonView.ViewID, currentHealth, playerMaxHealth);
        }
    }

    [PunRPC]
    private void UpdateHealthBar(int viewID,float currentHealth,float maxHealth)
    {
        PhotonView.Find(viewID).GetComponent<Health>().healthBar.fillAmount = (float)Math.Round(currentHealth, 0) / maxHealth;
        PhotonView.Find(viewID).GetComponent<Health>().textHealth.text = Math.Round(currentHealth, 0).ToString();
    }


    void Die()
    {
        dead = true;
        if (gameObject != null) 
        {
            PhotonNetwork.Destroy(gameObject);
            currentHealth = playerMaxHealth;
        }
        if(onPlayer)
        {
            PhotonNetwork.LocalPlayer.AddScore(-1);
        }
    }
    public void Respawn()
    {
        dead = false;
        if (onPlayer)
        {           
            GameObject newPlayer = PhotonNetwork.Instantiate(PrefabName, spawnBoxPos.transform.position, Quaternion.identity);
            newPlayer.GetComponent<playerSetup>().nickname = GetComponent<playerSetup>().nickname;
            newPlayer.GetComponent<playerSetup>().spawnInBox();
        }
        else
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.RandomRange(50f, -50f), 30f, UnityEngine.Random.RandomRange(50f, -50f));
            GameObject obj = PhotonNetwork.Instantiate(PrefabName, spawnPosition, Quaternion.identity);
        }
    }

    public void TD(float damage)
    {
        damageTaken = true;
        takenDamageRecently = true;
        photonView.RPC("TakeDamage", RpcTarget.All, damage);
        if (cooldownActivated == false)
        {
            StartCoroutine(RechargeHealthCoolDown());
        }
        if (onPlayer) 
        {
            UpdateHealthBar(photonView.ViewID, currentHealth, playerMaxHealth);
        }
        else if (healthBar != null)
        {
            photonView.RPC("UpdateHealthBar",RpcTarget.All, photonView.ViewID, currentHealth, playerMaxHealth);
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    [PunRPC]
    public void Heal(float hp)
    {
        currentHealth += hp;
        if (currentHealth > playerMaxHealth)
        {
            currentHealth = playerMaxHealth;
        }
        if (onPlayer)
        {
            UpdateHealthBar(photonView.ViewID, currentHealth, playerMaxHealth);
        }
        else if (healthBar != null)
        {
            photonView.RPC("UpdateHealthBar", RpcTarget.All, photonView.ViewID, currentHealth, playerMaxHealth);
        }
    }

    IEnumerator RechargeHealthCoolDown()
    {
        damageTaken = false;
        cooldownActivated = true;
        while (timeWaited < healthRegenCooldown)
        {
            if (damageTaken)
            { 
                timeWaited = 0;
            }
            yield return new WaitForSeconds(healthRegenCooldown / 100f);
            timeWaited += healthRegenCooldown/100f;
        }
        takenDamageRecently = false;
        cooldownActivated = false;
    }
}
