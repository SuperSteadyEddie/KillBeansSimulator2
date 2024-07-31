using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;


public class LegacyGunScript : MonoBehaviour
{
    public Gun[] guns;
    public Recoil recoil;
    public GameObject[] weapons;
    public List<GameObject> weaponModel = new List<GameObject>();
    public GameObject cam;
    public GameObject player;
    public GameObject crosshare;
    public TMP_Text gunInfo;
    public MoveCamera moveCamera;
    public PhotonView pv;
    Gun weaponPlaceHolder;
    Animation anim;
    Camera camera;
    GameObject bullet;
    GameObject casing;
    Rigidbody bulletRb;
    Rigidbody casingRb;


    [HideInInspector] public Transform shootPosition;
    [HideInInspector] public Transform casingSpawnPosition;
    [HideInInspector] public int gunSelected = 0;
    [HideInInspector] public Gun currentGun;
    [HideInInspector] public bool changedGun = true;
    [HideInInspector] public bool isReloading = false;
    [HideInInspector] public float reloadSpeedMultiplier = 1;
    [HideInInspector] public bool fullyScoped = false;
    bool shootCoolDownOver;
    bool canReload = true;
    bool shootAllowed = true;
    bool bursting = false;
    bool canSwitch = true;
    bool canAim = true;
    bool reloadCancel = false;
    int currentFireMode = 0;
    float waitTime;
    float elapsedTime;
    float startTime;
    float baseFOV;
    float baseSens;
    float aimSens;

    private void Start()
    {
        if (pv.IsMine)
        {
            baseSens = moveCamera.sensX;
            aimSens = baseSens / 3;
            camera = cam.GetComponent<Camera>();
            baseFOV = camera.fieldOfView;
            pv = GetComponent<PhotonView>();
            int i = 0;
            while (i <= guns.Length - 1)
            {
                guns[i].currentAmmo = guns[i].ammoInMags;
                guns[i].currentMags = guns[i].mags;
                //instantiateGun(guns[i].model.name);
                //print(i+" , " + weaponModel[i].name);
                i++;
            }
            currentGun = guns[0];
            anim = weapons[0].GetComponent<Animation>();
            selectGun(0);
            shootCoolDownOver = true;
        }

    }

    void instantiateGun(string gun)
    {
        GameObject newgun = PhotonNetwork.Instantiate(gun, transform.position, Quaternion.identity, (byte)pv.ViewID);
        weaponModel.Add(newgun);
        pv.RPC("parentGun", RpcTarget.All, newgun.GetComponent<PhotonView>().ViewID, pv.ViewID);
        newgun.SetActive(false);
    }
    [PunRPC]
    void parentGun(int gunId, int parentId)
    {
        PhotonView.Find(gunId).transform.parent = PhotonView.Find(parentId).transform;
    }

    void Update()
    {
        if (pv.IsMine)
        {
            checkInput();
        }
    }

    void checkInput()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            ScollInput(true);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            ScollInput(false);
        }
        if (currentGun.firemodes[currentFireMode] == Gun.FireType.auto)
        {
            if (Input.GetMouseButton(0) && canShoot())
            {
                shoot();
            }
        }
        else if (currentGun.firemodes[currentFireMode] == Gun.FireType.burst)
        {
            if (Input.GetMouseButtonDown(0) && canShoot() && !bursting)
            {
                StartCoroutine(BurstShot());
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && canShoot())
            {
                shoot();
            }
        }
        if (Input.GetKeyDown(KeyCode.R) & canReload)
        {
            canReload = false;
            weaponPlaceHolder = currentGun;
            StartCoroutine(Reload());
        }
        if (Input.GetKeyDown(KeyCode.V) & !isReloading && currentGun.firemodes.Length > 1)
        {
            StartCoroutine(FireSelect());
        }
        if (Input.GetMouseButtonUp(1))
        {
            elapsedTime = 0;
            startTime = Time.time;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            elapsedTime = 0;
            startTime = Time.time;
        }
        Aim(Input.GetMouseButton(1));
    }

    IEnumerator BurstShot()
    {
        canSwitch = false;
        bursting = true;
        int fired = 0;
        while (fired < currentGun.bulletsPerShot)
        {
            if (currentGun.currentAmmo <= 0)
            {
                bursting = false;
                canSwitch = true;
                break;
            }
            shoot();
            yield return new WaitForSeconds(currentGun.burstFireRate);
            fired++;
        }
        yield return null;
        bursting = false;
        canSwitch = true;
    }

    public IEnumerator FireSelect()
    {
        canSwitch = false;
        canReload = false;
        shootAllowed = false;
        currentFireMode += 1;
        if (currentFireMode > currentGun.firemodes.Length - 1)
        {
            currentFireMode = 0;
        }
        if (anim.GetClip("change fire mode (semi)"))
        {
            if (currentGun.firemodes[currentFireMode] == Gun.FireType.semiauto)
            {
                waitTime = anim.GetClip("change fire mode (semi)").length;
                yield return new WaitForSeconds(waitTime);
                anim.Play("change fire mode (semi)");
            }
            else if (currentGun.firemodes[currentFireMode] == Gun.FireType.auto)
            {
                waitTime = anim.GetClip("change fire mode (auto)").length;
                yield return new WaitForSeconds(waitTime);
                anim.Play("change fire mode (auto)");
            }
        }
        canReload = true;
        shootAllowed = true;
        canSwitch = true;
        UpdateUI();
        yield return null;
    }

    public IEnumerator Reload()
    {
        reloadCancel = false;
        startTime = Time.time;
        canAim = false;
        isReloading = true;
        if (currentGun.currentAmmo <= 0)
        {
            waitTime = currentGun.reloadTimeNoAmmo / reloadSpeedMultiplier;
            foreach (AnimationState animState in anim)
            {
                animState.speed *= reloadSpeedMultiplier;
            }
            anim.Play("reload no ammo");
        }
        else
        {
            waitTime = currentGun.reloadTime / reloadSpeedMultiplier;
            foreach (AnimationState animState in anim)
            {
                animState.speed *= reloadSpeedMultiplier;
            }
            anim.Play("reload");
        }
        yield return new WaitForSeconds(waitTime);
        if (reloadCancel == false)
        {
            startTime = Time.time;
            canAim = true;
            canReload = true;
            isReloading = false;
            currentGun.currentAmmo = currentGun.ammoInMags;
            currentGun.currentMags -= 1;
            UpdateUI();
            foreach (AnimationState animState in anim)
            {
                animState.speed = 1;
            }
            yield return null;
        }

    }

    public void shoot()
    {
        shootCoolDownOver = false;
        SpawnBullet();
        recoil.AddRecoil();
        pv.RPC("PlaySound", RpcTarget.All, 0, player.GetComponent<PhotonView>().ViewID, gunSelected);
        if (currentGun.currentAmmo <= 0)
        {
            anim.Play("shot no ammo");
        }
        else if (anim.GetClip("shot"))
        {
            anim.Play("shot");
        }
        UpdateUI();
        StartCoroutine(ShootCooldown());
    }

    public void Aim(bool scopedIn)
    {
        if (scopedIn && canAim)
        {
            if (currentGun.sight.type == Sight.SightType.Telescopic)
            {
                moveCamera.sensX = aimSens / 2;
                moveCamera.sensY = aimSens / 2;
            }
            else
            {
                moveCamera.sensX = aimSens;
                moveCamera.sensY = aimSens;
            }
            crosshare.SetActive(false);
            elapsedTime = (Time.time - startTime) / currentGun.sight.aimTime;
            weapons[gunSelected].transform.localPosition = Vector3.Lerp(currentGun.HipPosition, currentGun.AimInPosition, elapsedTime);
            camera.fieldOfView = Mathf.Lerp(baseFOV, baseFOV / currentGun.sight.cameraZoom, elapsedTime);
            if (elapsedTime >= 1)
            {
                fullyScoped = true;
            }
        }
        else
        {
            moveCamera.sensX = baseSens;
            moveCamera.sensY = baseSens;
            if (currentGun.gunType != Gun.GunType.sniper)
            {
                crosshare.SetActive(true);
            }
            elapsedTime = (Time.time - startTime) / currentGun.sight.aimTime;
            weapons[gunSelected].transform.localPosition = Vector3.Lerp(weapons[gunSelected].transform.localPosition, currentGun.HipPosition, elapsedTime);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, baseFOV, elapsedTime);
            fullyScoped = false;
        }
    }

    [PunRPC]
    void PlaySound(int soundId, int viewId, int gunID)
    {
        PhotonView.Find(viewId).GetComponent<AudioSource>().PlayOneShot(GetComponent<GunScript>().guns[gunID].audioClips[0]);
    }

    bool canShoot()
    {
        if (shootCoolDownOver && currentGun.currentAmmo > 0 && shootAllowed && !isReloading)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SpawnBullet()
    {
        if (currentGun.gunType == Gun.GunType.generic)
        {
            shootPosition = transform.Find(weapons[gunSelected].name + "/ShootPosition");
            currentGun.currentAmmo -= 1;
            bullet = PhotonNetwork.Instantiate(currentGun.bulletPrefab.name, shootPosition.position, shootPosition.rotation);
            bullet.GetComponent<projectile>().damage = currentGun.damage;
            bullet.GetComponent<projectile>().headDamageMultipler = currentGun.headShotMultiplier;
            bullet.GetComponent<projectile>().shooter = player;
            bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.AddForce(bullet.transform.forward * currentGun.muzzleForce, ForceMode.Impulse);
            SpawnCasing();

        }
        else if (currentGun.gunType == Gun.GunType.sniper)
        {
            shootPosition = transform.Find(weapons[gunSelected].name + "/ShootPosition");
            currentGun.currentAmmo -= 1;
            bullet = PhotonNetwork.Instantiate(currentGun.bulletPrefab.name, shootPosition.position, shootPosition.rotation);
            bullet.GetComponent<projectile>().damage = currentGun.damage;
            bullet.GetComponent<projectile>().headDamageMultipler = currentGun.headShotMultiplier;
            bullet.GetComponent<projectile>().shooter = player;
            bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.AddForce(bullet.transform.forward * currentGun.muzzleForce, ForceMode.Impulse);
        }
        else if (currentGun.gunType == Gun.GunType.shotgun)
        {
            currentGun.currentAmmo -= 1;
            int pelletCount = 0;
            while (pelletCount < currentGun.pelletCount)
            {
                Quaternion randomRotation = Random.rotation;
                if (currentGun.shotgunType == Gun.ShotgunType.breakAction)
                {
                    shootPosition = transform.Find(weapons[gunSelected].name + "/ShootPosition" + (currentGun.currentAmmo + 1));
                    bullet = PhotonNetwork.Instantiate(currentGun.bulletPrefab.name, shootPosition.position, shootPosition.rotation);
                }
                else
                {
                    shootPosition = transform.Find(weapons[gunSelected].name + "/ShootPosition");
                    bullet = PhotonNetwork.Instantiate(currentGun.bulletPrefab.name, shootPosition.position, shootPosition.rotation);
                }
                bullet.GetComponent<projectile>().damage = currentGun.damage;
                bullet.GetComponent<projectile>().headDamageMultipler = currentGun.headShotMultiplier;
                bullet.GetComponent<projectile>().shooter = player;
                if (fullyScoped)
                {
                    bullet.transform.rotation = Quaternion.RotateTowards(bullet.transform.rotation, randomRotation, currentGun.spreadWhileAiming);
                }
                else
                {
                    bullet.transform.rotation = Quaternion.RotateTowards(bullet.transform.rotation, randomRotation, currentGun.spread);
                }
                bulletRb = bullet.GetComponent<Rigidbody>();
                bulletRb.AddForce(bullet.transform.forward * currentGun.muzzleForce, ForceMode.Impulse);
                pelletCount++;
            }
            if (currentGun.shotgunType != Gun.ShotgunType.breakAction)
            {
                SpawnCasing();
            }
        }
    }

    void SpawnCasing()
    {
        casing = PhotonNetwork.Instantiate(currentGun.bulletCasingPrefab.name, casingSpawnPosition.position, casingSpawnPosition.rotation);
        casingRb = casing.GetComponent<Rigidbody>();
        float casingRightForce = Random.Range(1f, 2f);
        float casingUpForce = Random.Range(2.1f, 2.5f);
        float casingBackForce = Random.Range(-2.5f, -2.75f);
        float casingUpTorque = Random.Range(10f, -60f); ;
        float casingRightTorque = Random.Range(10f, -60f); ;
        casingRb.AddForce(cam.transform.right * casingRightForce, ForceMode.Impulse);
        casingRb.AddForce(cam.transform.up * casingUpForce, ForceMode.Impulse);
        casingRb.AddForce(cam.transform.forward * casingBackForce, ForceMode.Impulse);
        casingRb.AddForce(player.GetComponent<Rigidbody>().velocity, ForceMode.Impulse);
        casingRb.AddTorque(cam.transform.up * casingUpTorque, ForceMode.Impulse);
        casingRb.AddTorque(cam.transform.right * casingRightTorque, ForceMode.Impulse);
    }

    public IEnumerator ShootCooldown()
    {
        if (currentGun.gunType == Gun.GunType.sniper)
        {
            elapsedTime = 0;
            startTime = Time.time;
            canAim = false;
        }
        yield return new WaitForSeconds(currentGun.fireRate);
        shootCoolDownOver = true;
        if (currentGun.gunType == Gun.GunType.sniper)
        {
            elapsedTime = 0;
            startTime = Time.time;
            canAim = true;
        }
        yield return null;
    }


    void ScollInput(bool isPositive)
    {
        if (canSwitch)
        {
            if (isPositive)
            {
                pv.RPC("setModelView", RpcTarget.All, gunSelected, false);
                gunSelected += 1;
                if (gunSelected > guns.Length - 1)
                {
                    gunSelected = 0;
                }
                selectGun(gunSelected);
            }
            else
            {
                pv.RPC("setModelView", RpcTarget.All, gunSelected, false);
                gunSelected -= 1;
                if (gunSelected < 0)
                {
                    gunSelected = guns.Length - 1;
                }
                selectGun(gunSelected);
            }
        }
    }

    public void selectGun(int i)
    {
        anim.Stop("reload");
        anim.Stop("reload no ammo");
        anim.Stop("shoot");
        anim.Play("idle");
        canReload = true;
        reloadCancel = true;
        changedGun = true;
        isReloading = false;
        canAim = true;
        currentGun = guns[i];
        currentFireMode = 0;
        //weaponModel[i].transform.localPosition = currentGun.HipPosition;
        startTime = Time.time;
        pv.RPC("setModelView", RpcTarget.All, i, true);
        anim = weapons[i].GetComponent<Animation>();
        casingSpawnPosition = transform.Find(weapons[i].name + "/casingSpawnPosition");
        if (currentGun.gunType == Gun.GunType.sniper)
        {
            crosshare.SetActive(false);
        }
        UpdateUI();
        changedGun = false;
    }

    [PunRPC]
    public void setModelView(int i, bool isActive)
    {
        weapons[i].SetActive(isActive);
    }

    void UpdateUI()
    {
        gunInfo.text = currentGun.name + "\nMags: " + currentGun.currentMags + "\n" + currentGun.currentAmmo + "/" + currentGun.ammoInMags + "\n" + currentGun.firemodes[currentFireMode];
    }
}
