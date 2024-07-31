using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements.Experimental;

public class SpawnMags : MonoBehaviourPun
{
    public Transform position;
    public GameObject mag;
    public GameObject player;
    public float ejectUpForce;
    public float ejectRightForce;
    public float ejectBackForce;
    public float torqueUpForce;
    public float torqueRightForce;
    public float torqueBackForce;

    GameObject currentMag;
    Rigidbody magRb;

    void spawnMag()
    {
        currentMag = PhotonNetwork.Instantiate(mag.name, position.position, position.rotation, (byte)photonView.ViewID);
        ParentMag(currentMag.GetComponent<PhotonView>().ViewID, photonView.ViewID, true);
        print("spawned");
    }
    void removeMag()
    {
        magRb = currentMag.AddComponent<Rigidbody>();
        ParentMag(currentMag.GetComponent<PhotonView>().ViewID, photonView.ViewID, false);
        magRb.AddForce(player.GetComponent<Rigidbody>().velocity);
        magRb.AddForce(transform.up * ejectUpForce);
        magRb.AddForce(transform.right * ejectRightForce);
        magRb.AddForce(transform.forward * -ejectBackForce);
        magRb.AddTorque(transform.up * torqueUpForce);
        magRb.AddTorque(transform.right * torqueRightForce);
        magRb.AddTorque(transform.forward * -torqueBackForce);
    }
    public void ParentMag(int projectileid, int parentid, bool parent)
    {
        if (parent)
        {
            PhotonView.Find(projectileid).transform.parent = PhotonView.Find(parentid).transform.Find("MagPosition");
            print("Done");
        }
        else
        {
            PhotonView.Find(projectileid).transform.parent = null;
        }
    }
}
