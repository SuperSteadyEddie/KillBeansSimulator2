using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Gun", menuName = "Guns")]
public class Gun : ScriptableObject
{
    #region variables 
    public GameObject model;

    [Header("Stats")]
    public float damage;
    public float fireRate;
    [HideInInspector] public float burstFireRate;
    public float headShotMultiplier;
    public float bodyShotMultiplier;
    public int muzzleForce;
    public FireType[] firemodes;
    public GunType gunType;
    [HideInInspector] public int bulletsPerShot;

    [Header("Range")]
    public AnimationCurve damageFalloff;
    public float[] distances;
    public float[] damagesAtRange;

    [Header("Reload")]
    public int mags;
    public int ammoInMags;
    public float reloadTime;
    public float reloadTimeNoAmmo;

    [Header("Bullet")]
    public bulletType bullet;
    public GameObject bulletPrefab;
    public GameObject bulletCasingPrefab;

    [Header("Hipfire Recoil")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float gunRecoilX;
    public float gunRecoilY;
    public float gunRecoilZ;
    [Header("Aim Recoil")]
    public float aimrecoilX;
    public float aimrecoilY;
    public float aimrecoilZ;
    public float gunAimrecoilX;
    public float gunAimrecoilY;
    public float gunAimrecoilZ;

    [Header("Sight")]
    public Sight sight;
    public Vector3 AimInPosition;
    public Vector3 HipPosition;

    [Header("Audio")]
    public AudioClip[] audioClips;

    [HideInInspector] public int currentAmmo;
    [HideInInspector] public int currentMags;

    //shotguns only
    [HideInInspector] public ShotgunType shotgunType;
    [HideInInspector] public float spread;
    [HideInInspector] public float spreadWhileAiming;
    [HideInInspector] public int pelletCount;


    public enum bulletType
    {
        Rocket,
        Twentyxonehundredandten,
        Fiftycal,
        Buckshot,
        ThreeThreeEightNormaMagnum,
        SevenSixTwo,
        FiveFiveSix,
        TwoTwoThree,
        NineMilimeter,
        TwentyTwoLR,
    }


    public enum FireType
    {
        semi,
        semiauto,
        burst,
        auto
    }

    public enum GunType
    {
        generic,
        shotgun,
        sniper
    }

    public enum ShotgunType
    {
        breakAction,
        pump,
        automatic
    }
    #endregion

    private void OnValidate()
    {
        damageFalloff.MoveKey(0, new Keyframe(0, damage));
        int i = 1;
        foreach(float distance in distances)
        {
            try
            {
                damageFalloff.MoveKey(i, new Keyframe(distance, damagesAtRange[i-1]));
            }
            catch
            {
                damageFalloff.AddKey(distance, damagesAtRange[i-1]);
            }
            i++;
        }
    }

    #region Editor 
#if UNITY_EDITOR
    [CustomEditor(typeof(Gun))]
    public class GunsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Gun gun = (Gun)target;

            EditorGUILayout.Space();
            if (gun.gunType == GunType.shotgun)
            {
                EditorGUILayout.LabelField("Shotgun");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("PelletCount");
                gun.pelletCount = EditorGUILayout.IntField(gun.pelletCount);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Shotgun Type: ");
                gun.shotgunType = (ShotgunType)EditorGUILayout.EnumPopup(gun.shotgunType);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Spread");
                gun.spread = EditorGUILayout.FloatField(gun.spread);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Spread whle aiming");
                gun.spreadWhileAiming = EditorGUILayout.FloatField(gun.spreadWhileAiming);
                EditorGUILayout.EndHorizontal();
            }
            if (gun.firemodes.Contains(FireType.burst))
            {
                EditorGUILayout.LabelField("Burst");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Bullets per burst");
                gun.bulletsPerShot = EditorGUILayout.IntField(gun.bulletsPerShot);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Burst fire rate");
                gun.burstFireRate = EditorGUILayout.FloatField(gun.burstFireRate);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
    #endregion
}
