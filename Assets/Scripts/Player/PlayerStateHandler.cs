using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerStateHandler : MonoBehaviour
{
    public PlayerStates states;
    public WeaponUsing weaponUsing;

    [Header ("Buffs")]
    public bool Shield;
    public bool Invincible;
    public bool SpeedBoost;
    public bool JumpBoost;
    public bool UnlimitedStamina;

    [Header ("DeBuffs")]
    public bool Slowed;

    [Header ("Player Status")]
    public bool outOfStamina;
    public bool playerIsSafe = true;


    #region Timers
    public bool showTimers;
    [ShowIf("showTimers")] public float currentInvincibleTime;
    [ShowIf("showTimers")] public float currentSpeedBoostTimer;
    [ShowIf("showTimers")] public float currentJumpBoostTimer;
    [ShowIf("showTimers")] public float currentUnlimitedStaminaTimer;
    [ShowIf("showTimers")] public float dangerZoneTime;
    [ShowIf("showTimers")] public float currentTimeMultiplier;
    [ShowIf("showTimers")] public float timeMultiplier;

    [ShowIf("showTimers")] public float currentSlowedTimer;
    #endregion

    #region Call outs
    private void Update() {
        HandleDangerZoneTime();
    }

    void HandleDangerZoneTime(){
        if(!playerIsSafe){
            dangerZoneTime += 1 * Time.deltaTime;
            currentTimeMultiplier += 1 * Time.deltaTime;
            if(currentTimeMultiplier >= 1f){
                timeMultiplier += 0.05f;
                Debug.Log(timeMultiplier);
                currentTimeMultiplier = 0f;
            }
            Mathf.RoundToInt(dangerZoneTime);
        }
    }

    #endregion
}
