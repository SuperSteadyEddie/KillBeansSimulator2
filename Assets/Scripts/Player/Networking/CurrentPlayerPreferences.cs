using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentPlayerPreferences : MonoBehaviour
{
       public Slider sensitivity;
       public TMP_Text sensText;
       public MoveCamera moveCamera;
       public void onsliderChange()
       {
          sensText.text = "Sensitivity: " + sensitivity.value.ToString();
       }

       public void ChangeSettings()
       {
          GetComponent<GunScript>().baseSens = sensitivity.value;
          moveCamera.sensX = sensitivity.value;
          moveCamera.sensY = sensitivity.value;
       }
}
