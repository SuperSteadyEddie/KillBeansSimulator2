
using System.Net.Cache;
using UnityEngine;

public class Recoil : MonoBehaviour
{

    public GameObject recoilObject;

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 gunCurrentRotation;
    private Vector3 gunTargetRotation;

    public float snappiness;
    public float returnSpeed;

    private float recoilX;
    private float recoilY;
    private float recoilZ;
    private float gunRecoilX;
    private float gunRecoilY;
    private float gunRecoilZ;

    public GunScript gunScript;
    public GameObject gunHolder;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero,returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        gunTargetRotation = Vector3.Lerp(gunTargetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        gunCurrentRotation = Vector3.Slerp(gunCurrentRotation, gunTargetRotation, snappiness * Time.fixedDeltaTime);
        recoilObject.transform.localRotation = Quaternion.Euler(currentRotation);
        gunHolder.transform.localRotation = Quaternion.Euler(gunCurrentRotation);
    }

    public void AddRecoil()
    {
        if (gunScript.fullyScoped)
        {
            recoilX = gunScript.currentGun.aimrecoilX;
            recoilY = gunScript.currentGun.aimrecoilY;
            recoilZ = gunScript.currentGun.aimrecoilZ;
            gunRecoilX = gunScript.currentGun.gunAimrecoilX;
            gunRecoilY = gunScript.currentGun.gunAimrecoilY;
            gunRecoilZ = gunScript.currentGun.gunAimrecoilZ;
        }
        else
        {
            recoilX = gunScript.currentGun.recoilX;
            recoilY = gunScript.currentGun.recoilY;
            recoilZ = gunScript.currentGun.recoilZ;
            gunRecoilX = gunScript.currentGun.gunRecoilX;
            gunRecoilY = gunScript.currentGun.gunRecoilY;
            gunRecoilZ = gunScript.currentGun.gunRecoilZ;
        }
        targetRotation += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        gunTargetRotation += new Vector3(-gunRecoilX, Random.Range(-gunRecoilY, gunRecoilY), Random.Range(-gunRecoilZ, gunRecoilZ));
    }
}
