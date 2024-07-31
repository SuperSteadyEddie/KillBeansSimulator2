using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SightScript : MonoBehaviourPun
{
    public Camera scopeCam;
    public Sight sight;

    // Update is called once per frame

    private void Start()
    {
        if (!photonView.IsMine)
        {
            scopeCam.enabled = false;
        }
        else
        {
            sight.currentScopeZoom = 90 / scopeCam.fieldOfView;
        }
    }

    void Update()
    {
        if (photonView.IsMine) 
        {
            if (Input.GetKey(KeyCode.B) && sight.type == Sight.SightType.Telescopic)
            {
                ChangeScopeZoom(sight.scopeZoomChangeSpeed);
            }
            else if (Input.GetKey(KeyCode.N) && sight.type == Sight.SightType.Telescopic)
            {
                ChangeScopeZoom(-sight.scopeZoomChangeSpeed);
            }
        }
    }
    public void ChangeScopeZoom(float Amount)
    {
        scopeCam.GetComponent<Camera>().fieldOfView -= Amount * Time.deltaTime;
        if (scopeCam.fieldOfView < 90 / sight.maxScopeZoom)
        {
            scopeCam.fieldOfView = 90 / sight.maxScopeZoom;
        }
        else if (scopeCam.GetComponent<Camera>().fieldOfView > 90 / sight.minScopeZoom)
        {
            scopeCam.GetComponent<Camera>().fieldOfView = 90 / sight.minScopeZoom;
        }
        sight.currentScopeZoom = 90/scopeCam.fieldOfView;
    }
}
