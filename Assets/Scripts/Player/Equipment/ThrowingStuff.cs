using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowingStuff : MonoBehaviour
{

    [Header("References")]
    public GameObject attackPoint;
    public Transform cam;
    public GameObject throwObject;


    // [Header("Settings")]
    // public float throwCooldown;
    //  public float totalThrows;

    [Header("Throwing")]
    public float throwForce;
    public float upwardThrowForce;
    public float throwCooldown;
    bool readyToThrow;


    // Start is called before the first frame update
    void Start()
    {
        readyToThrow = true;
    }


    public void Throw()
    {
        readyToThrow = false;
        GameObject projectile = Instantiate(throwObject, attackPoint.transform.position, cam.rotation);


        Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * upwardThrowForce;
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        Invoke(nameof(resetThrow), throwCooldown);
    }

    public void resetThrow()
    {
        readyToThrow = true;
    }

}   
