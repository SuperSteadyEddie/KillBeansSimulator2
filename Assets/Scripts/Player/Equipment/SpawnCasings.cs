using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class SpawnCasings : MonoBehaviourPun
{
    public Transform[] positions;
    public GameObject casing;
    GameObject shell;
    Rigidbody shellRb;
    List<GameObject> shells = new List<GameObject>();

    void spawnCasings()
    {
        int i = 0;
        foreach (Transform p in positions)
        {
            shell = PhotonNetwork.Instantiate(casing.name, p.position, p.rotation, (byte)photonView.ViewID);
            i++;
            ParentCasing(shell.GetComponent<PhotonView>().ViewID, photonView.ViewID, true,i);
            shells.Add(shell);
        }
    }
    void spawnOneCasing()
    {
        int i = 0;
        shell = PhotonNetwork.Instantiate(casing.name, positions[0].position, positions[0].rotation, (byte)photonView.ViewID);
        i++;
        ParentCasing(shell.GetComponent<PhotonView>().ViewID, photonView.ViewID, true, i);
        shells.Add(shell);
    }
    void removeCasings()
    {
        foreach (GameObject s in shells) 
        {
            ParentCasing(s.GetComponent<PhotonView>().ViewID, photonView.ViewID, false, 0);
            shellRb = s.AddComponent<Rigidbody>();
            shellRb.AddForce(s.transform.up * -1f);
            shellRb.AddForce(s.transform.forward * -10f);
        }
        shells = new List<GameObject>();
    }
    public void ParentCasing(int projectileid, int parentid, bool parent, int i)
    {
        if (parent)
        {
            PhotonView.Find(projectileid).transform.parent = PhotonView.Find(parentid).transform.Find("rotate bit/casingSpawnPosition " + i);
        }
        else
        {
            PhotonView.Find(projectileid).transform.parent = null;
        }
    }
}
