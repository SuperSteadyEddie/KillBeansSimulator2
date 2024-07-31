using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravity : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 newGravity;
    void Start()
    {
        Physics.gravity = newGravity;
    }

}
