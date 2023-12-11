using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    private PlayerStateHandler playerState;
    public TinCan TinCan;
    public EnemyAi enemyAi;
    public GameObject dangerTimerGUI;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerState = player.GetComponent<PlayerStateHandler>();

    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if(playerState.playerIsSafe){
            dangerTimerGUI.active = false;
        }else{
            dangerTimerGUI.active = true;
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            playerState.playerIsSafe = false;
        }
    }
}
