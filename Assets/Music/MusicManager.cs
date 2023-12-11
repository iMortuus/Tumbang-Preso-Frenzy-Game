using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject enemy;
    public TinCan tinCanScript;
    private PlayerStateHandler playerState;
    private PointsHandler pointsHandler;
    private Throwing throwScript;
    private EnemyAi enemyAI;


    [Header("Background Music")]
    public float bgmTransitionSpeed;
    public bool transitionBGM;
    public AudioSource bgmDefault;
    public AudioSource bgmChase;
    private bool bgmChaseplayed; 

    [Header("Sound Effects")]
    public AudioSource strikeSFX;
    public AudioSource safeSFX;

    public 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        enemy = GameObject.Find("Enemy");
        tinCanScript = GameObject.Find("TinCan").GetComponent<TinCan>();

        playerState = player.GetComponent<PlayerStateHandler>();
        throwScript = player.GetComponent<Throwing>();
        pointsHandler = player.GetComponent<PointsHandler>();

        enemyAI = enemy.GetComponent<EnemyAi>();
    }

    // Update is called once per frame
    void Update()
    {
        handleChaseBGM();
        handleSafeSound();
        handleDeathSound();
    }

    void handleSafeSound(){
        if(enemyAI.playerIsSafe && !transitionBGM){
            safeSFX.Play();
            bgmChaseplayed = false;
            transitionBGM = true;
            enemyAI.playerIsSafe = false;
        }

        if(transitionBGM){
            bgmChase.volume = Mathf.Lerp(bgmChase.volume, -0.1f, bgmTransitionSpeed * Time.deltaTime);
            bgmChase.volume = Mathf.Clamp(bgmChase.volume, 0, 1);
            if(bgmChase.volume == 0){
                bgmDefault.UnPause();
                bgmChase.Stop();
                bgmChase.volume = 1f;
                transitionBGM = false;
            }
        }
    }

    void handleChaseBGM(){
        if(throwScript.totalThrows == 0 && !bgmChaseplayed ){
            bgmChaseplayed = true;
            if(bgmChaseplayed && tinCanScript.displaced){
                strikeSFX.Play();
            }

            if(bgmChaseplayed && !tinCanScript.displaced){
                bgmChase.Play();
                bgmDefault.Pause();
            }
        }

        if(tinCanScript.displaced && !tinCanScript.gotDisplaced && !bgmChaseplayed){
            bgmChaseplayed = true;
            if(bgmChaseplayed){
                strikeSFX.Play();
                bgmDefault.Pause();
                bgmChase.Play();
            }
        }
    }

    void handleDeathSound(){
        if(playerState.states == PlayerStates.Dead){
            bgmChase.pitch = Mathf.Lerp(bgmChase.pitch, 0.3f, bgmTransitionSpeed * Time.deltaTime);
        }
    }
}
