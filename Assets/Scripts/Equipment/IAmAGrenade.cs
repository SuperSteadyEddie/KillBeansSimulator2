using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IAmAGrenade : MonoBehaviourPun
{

    public Grenade grenade;
    [HideInInspector]public GameObject player;
    GameObject currentShrapnel;
    private void Start()
    {
        if (!grenade.ImpactGrenade)
        {
            StartCoroutine(beginCountDown());
        }
    }

    IEnumerator beginCountDown()
    {
        yield return new WaitForSeconds(grenade.fuseTime);
        photonView.RPC("DetonateEffect", RpcTarget.All, photonView.ViewID);
        Detonate();
    }

    public void Detonate()
    {
        int shrapnelCount = 0;
        Vector3 grenadePos = transform.position;
        while (shrapnelCount < grenade.shrapnel)
        {
            currentShrapnel = PhotonNetwork.Instantiate("shrapnel", grenadePos, Random.rotation);
            projectile p = currentShrapnel.GetComponent<projectile>();
            p.shrapnel = true;
            p.damage = grenade.damage;
            p.damageCurve = grenade.damageFalloff;
            p.shooter = player;
            p.shotPosition = grenadePos;
            currentShrapnel.GetComponent<Rigidbody>().AddForce(currentShrapnel.transform.forward * 20f,ForceMode.Impulse);
            shrapnelCount++;
        }
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        gameObject.SetActive(false);

    }

    [PunRPC]
    public void DetonateEffect(int viewID)
    {
        AudioSource.PlayClipAtPoint(PhotonView.Find(viewID).GetComponent<IAmAGrenade>().grenade.explosion, PhotonView.Find(viewID).transform.position);
    }

    private void OnCollisionEnter(Collision c)
    {
        if(grenade.ImpactGrenade && c.gameObject != player)
        {
            photonView.RPC("DetonateEffect", RpcTarget.All, photonView.ViewID);
            Detonate();
        }
    }
}
