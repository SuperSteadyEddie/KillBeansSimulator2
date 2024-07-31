using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GunMenu : MonoBehaviour
{
    public GameObject loadoutButton;
    public GameObject spawnButton;
    public GameObject primaryGunMenu;
    public GameObject secondaryGunMenu;

    public TMP_Dropdown primaryDropDown;

    public List<int> Guns = new List<int>();

    public void BackToStart()
    {
        Guns.Add(0);
        Guns.Add(1);
        loadoutButton.SetActive(true);
        primaryGunMenu.SetActive(false);
        secondaryGunMenu.SetActive(false);
    }

    public void OpenLoadout()
    {
        loadoutButton.SetActive(false);
        primaryGunMenu.SetActive(true);
    }

    public void OpenPrimary()
    {
       primaryGunMenu.SetActive(true);
       secondaryGunMenu.SetActive(false);
    }
    public void OpenSecondary()
    {
        primaryGunMenu.SetActive(false);
        secondaryGunMenu.SetActive(true);
    }

    public void ChangePrimary(int GunID)
    {
        Guns[0] = GunID;
    }
    public void ChangeSecondary(int GunID)
    {
        Guns[1] = GunID;
    }
}
