using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    [Range(100, 1000)] public float maxStamina;
    [Range(0, 50)] public float staminaDrainRate;
    [Range(0, 50)] public float staminaGainRateWhenCrouching;
    [Range(0, 50)] public float staminaGainRate;
    [Range(1, 50)] public float staminaCoolDown;

    [HideInInspector] public bool staminaGainAllowed = true;
    [HideInInspector] public float currentGainRate;
    [HideInInspector] public float currentDrainRate;
    [HideInInspector] public float stamina;
    bool s = true;
    [HideInInspector] public float i = 0;

    public UnityEngine.UI.Image staminaBar;
    public UnityEngine.UI.Image staminaRegenBar;
    public GameObject staminaBarBG;

    MovePlayer movePlayer;
    Adrenaline adrenaline;

    // Start is called before the first frame update
    void Start()
    {
        stamina = maxStamina;
        currentGainRate = staminaGainRate;
        currentDrainRate = staminaDrainRate;
        adrenaline = GetComponent<Adrenaline>();
        movePlayer = GetComponent<MovePlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStaminaBar();
        if (movePlayer.state != MovePlayer.MovementState.sprinting)
        {
            StaminaGain();
        }
        else
        {
            i = 0;
        }
    }

    public void StaminaGain()
    {
       if (s && !staminaGainAllowed)
       {
            s = false;
            StartCoroutine(RechargeStamina());
       }
       else if (staminaGainAllowed && stamina < maxStamina)
       {
            if (adrenaline.adrenalineActive)
            {
                stamina += currentGainRate * adrenaline.adrenalineStaminaGainRateMultiplier * Time.deltaTime;
            }
            else
            {
                stamina += currentGainRate * Time.deltaTime;
            }
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
                i = 0;
            }
        }
    }
    public void StaminaDrain()
    {
        i = 0;
        if (stamina > 0)
        {
            if (adrenaline.adrenalineActive)
            {
                stamina -= staminaDrainRate * adrenaline.adrenalineStaminaDrainRateMultiplier * Time.deltaTime;
            }
            else
            {
                stamina -= staminaDrainRate * Time.deltaTime;
            }
            if (stamina < 0)
            {
                stamina = 0;
            }
        }
    }

    private IEnumerator RechargeStamina()
    {
        i = 0;
        staminaRegenBar.enabled = true;
        while (i < staminaCoolDown)
        {
            if (movePlayer.state == MovePlayer.MovementState.sprinting)
            {
                i = 0;
                staminaRegenBar.enabled = false;
                s = true;
                break;
            }
            yield return new WaitForSeconds(staminaCoolDown / 100f);
            i += staminaCoolDown / 100f;
            staminaRegenBar.fillAmount = i / staminaCoolDown;
        }
        staminaGainAllowed = true;
        s = true;
        yield return null;
    }

    void UpdateStaminaBar()
    {
        staminaBar.fillAmount = stamina / maxStamina;
    }
}
