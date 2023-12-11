using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public Transform Barrier;
    public GameObject safeGUI;
    public AudioSource bgmDefault;
    public AudioSource bgmChase;
    public TinCan TinCan;
    public EnemyAi enemyAi;
    public Transform player;
    private PlayerStateHandler playerState;
    public PointsHandler pointsHandler;
    // Start is called before the first frame update
    void Start()
    {
        playerState = player.GetComponent<PlayerStateHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetComponent<Throwing>().totalThrows <= 0){
            Barrier.GetComponent<Collider>().enabled = false;
        }
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other){
        if(TinCan.gotDisplaced && other.tag == "Player" && pointsHandler.carriedPoints != 0 || enemyAi.outOfThrows && other.tag == "Player" && pointsHandler.carriedPoints != 0 && !playerState.playerIsSafe){
            Barrier.GetComponent<Collider>().enabled = true;
            pointsHandler.totalPoints += (pointsHandler.carriedPoints + (pointsHandler.carriedPoints * playerState.timeMultiplier));
            StartCoroutine(HandleSafeGUI());
            //player.transform.GetComponent<Throwing>().totalThrows = 
            pointsHandler.carriedPoints = 0;
            playerState.playerIsSafe = true;
            enemyAi.playerIsSafe = true;
            enemyAi.outOfThrows = false;
            playerState.dangerZoneTime = 0f;

        }
    }

    [System.Obsolete]
    IEnumerator HandleSafeGUI(){
        safeGUI.active = true;
        yield return new WaitForSeconds(2f);
        safeGUI.active = false;
    }
}
