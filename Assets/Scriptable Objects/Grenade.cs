using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Grenade", menuName = "Grenades")]
public class Grenade : ScriptableObject
{
    public GameObject model;
    public string name;
    public int shrapnel;
    public float throwForce;
    public float damage;
    public float count;
    public float grenadeMass;
    public float fuseTime;
    public bool ImpactGrenade;

    public AudioClip explosion;

    [Header("Range")]
    public AnimationCurve damageFalloff;
    public float[] distances;
    public float[] damagesAtRange;

    private void OnValidate()
    {
        damageFalloff.MoveKey(0, new Keyframe(0, damage));
        int i = 1;
        foreach (float distance in distances)
        {
            try
            {
                damageFalloff.MoveKey(i, new Keyframe(distance, damagesAtRange[i - 1]));
            }
            catch
            {
                damageFalloff.AddKey(distance, damagesAtRange[i - 1]);
            }
            i++;
        }
    }
}
