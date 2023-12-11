using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatesHandler : MonoBehaviour
{
    public EnemyStates enemyState;
    EnemyAi enemyAi;
    public float stunTimer;
    public float maxStunTimer;
    
    #region VFX
    public GameObject stunVFX;
    public GameObject stunVFX1;
    #endregion

    #region  SFX
    public AudioSource stunSFX;
    #endregion
    private void Start() {
        enemyAi = transform.GetComponent<EnemyAi>();
    }

    [System.Obsolete]
    private void Update() {
        handleStun();
    }

    #region  Private Callbacks
    [System.Obsolete]
    void handleStun(){
        if(stunTimer != 0){
            stunVFX.active = true;
            stunVFX1.active = true;
            stunTimer -= 1 * Time.deltaTime;
            enemyState = EnemyStates.Stunned;
            if(stunTimer <= 0)stunTimer = 0f;
            if(stunTimer >= maxStunTimer) stunTimer = maxStunTimer;
        }else{
            enemyState = EnemyStates.Normal;
            stunVFX.active = false;
            stunVFX1.active = false;
        }
    }

    #endregion 

    #region Public Callbacks
    public void addStun(float time){
        if(!enemyAi.isPatroling){
            stunSFX.Play();
            stunTimer += time;

        }
    }

    #endregion
}
