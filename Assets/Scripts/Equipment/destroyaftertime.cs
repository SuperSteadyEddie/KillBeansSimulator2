using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class destroyaftertime : MonoBehaviourPun
{
    public float secondsUntilDestroy;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(kill());
    }

    public IEnumerator kill()
    {
        yield return new WaitForSeconds(secondsUntilDestroy);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }
}
