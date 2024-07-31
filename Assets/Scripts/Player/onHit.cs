using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onHit : MonoBehaviour
{
    public Health healthScript;
    public bodyType bodyT;
    public enum bodyType
    {
        generic,
        limb,
        body,
        head
    }
}
