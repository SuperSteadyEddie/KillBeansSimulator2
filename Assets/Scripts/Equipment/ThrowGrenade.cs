using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class ThrowGrenade : MonoBehaviourPun
{
    public Grenade[] grenade;
    Grenade grenadeSelected;
    public GameObject player;
    GameObject currentGrenade;
    float currentGrenades;
    int index=0;
    [HideInInspector]public bool canThrow;
    bool grenadeInHand;
    private void Start()
    {
        if(photonView.IsMine)
        {
            if (grenade.Length == 0) { enabled = false; }
            canThrow = true;
            currentGrenades = grenade[0].count;
            grenadeSelected = grenade[0];
        }
    }

    void ChangeGrenade(int i)
    {
        grenadeSelected = grenade[i];
        currentGrenades = grenade[i].count;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            index++;
            if (index > grenade.Length - 1)
            {
                index = 0;
            }
            print(index);
            ChangeGrenade(index);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            index--;
            if (index < 0)
            {
                index = grenade.Length-1;
            }
            print(index);
            ChangeGrenade(index);
        }

        if (Input.GetKeyDown(KeyCode.G) && currentGrenades > 0 && photonView.IsMine && canThrow)
        {
            currentGrenades -= 1;
            grenadeInHand = true;
            spawnGrenade(transform.position);
            photonView.RPC("parentGrenade", RpcTarget.All, currentGrenade.GetComponent<PhotonView>().ViewID, photonView.ViewID, true);
        }
        if(Input.GetKeyUp(KeyCode.G) && photonView.IsMine && grenadeInHand)
        {
            throwGrenade();
        }
    }

    public void spawnGrenade(Vector3 pos)
    {
        currentGrenade = PhotonNetwork.Instantiate(grenadeSelected.model.name, pos, Quaternion.identity);
    }

    public void throwGrenade()
    {
        photonView.RPC("parentGrenade", RpcTarget.All, currentGrenade.GetComponent<PhotonView>().ViewID, photonView.ViewID, false);
        currentGrenade.AddComponent<Rigidbody>().mass = grenadeSelected.grenadeMass;
        currentGrenade.GetComponent<IAmAGrenade>().player = player;
        currentGrenade.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        currentGrenade.GetComponent<Rigidbody>().AddForce(transform.forward * grenadeSelected.throwForce, ForceMode.Impulse);
        grenadeInHand = false;
    }

    [PunRPC]
    void parentGrenade(int grenadeid, int parentId, bool parent)
    {
        if (parent == true)
        {
            PhotonView.Find(grenadeid).transform.parent = PhotonView.Find(parentId).transform;
        }
        else
        {
            PhotonView.Find(grenadeid).transform.parent = null;
        }

    }
}
