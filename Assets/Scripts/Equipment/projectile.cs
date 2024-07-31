using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Pun.UtilityScripts;

public class projectile : MonoBehaviourPun
{
    [HideInInspector] public float damage;
    [HideInInspector] public float headDamageMultipler;
    [HideInInspector] public Vector3 shotPosition;
    [HideInInspector] public GameObject shooter;
    [HideInInspector] public bool canHit = true;
    [HideInInspector] public bool shrapnel = false;
    [HideInInspector] public AnimationCurve damageCurve;
    float currentDistance;

    void OnCollisionEnter(Collision c)
    {
        if(!shrapnel && c.gameObject != shooter)
        {
            HitTarget(c);
            canHit = false;
        }
        else if (shrapnel)
        {
            HitTarget(c);
        }
    }
    void HitTarget(Collision collision)
    {
        if (collision.gameObject.GetComponent<Health>() && canHit)
        {
            currentDistance = Vector3.Distance(collision.transform.position, shotPosition);

            damage = damageCurve.Evaluate(currentDistance);
            if (collision.GetContact(0).otherCollider.gameObject.GetComponent<onHit>())
            {
                if (collision.GetContact(0).otherCollider.gameObject.GetComponent<onHit>().bodyT == onHit.bodyType.head)
                {
                    damage *= headDamageMultipler;
                }
                print("distance: " + currentDistance + " base damage: " + damageCurve.Evaluate(currentDistance) + " damage taken: " + damage);
                collision.GetContact(0).otherCollider.gameObject.GetComponent<onHit>().healthScript.TD(damage);
                if (shooter != null)
                {
                    shooter.GetComponent<HUD>().hit();
                }
            }
        }
    }

}

