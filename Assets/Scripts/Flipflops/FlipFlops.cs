using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipFlops : MonoBehaviour
{
    EnemyAi enemyAi;
    GameObject player;
    public GameObject indicator;
    GameObject currentIndicator;
    public float pickUpRange;
    public AudioSource pickUpSFX;
    public LayerMask whatIsPlayer;
    public new Collider collider;
    public bool canRotate = false;
    private bool collided = false;
    bool stunAdded = false;
    bool indicatorOn;
    private bool playerInPickUpRange, added;
    public bool canGivePoints;
    [SerializeField] private float rotationSpeed = 20;
    public float stunTime = 2f;
    float canStunTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pickUpSFX = GameObject.Find("PickUpFlipflopsSFX").GetComponent<AudioSource>();
        enemyAi = GameObject.Find("Enemy").GetComponent<EnemyAi>();
        player = GameObject.Find("Player");
        if(rotationSpeed <= 0){
            canRotate = false;
        }
        if(collided){
            rotationSpeed = rotationSpeed - (Time.deltaTime * 10);
            canStunTime -= 1 * Time.deltaTime;
            CheckForPlayer();
            if(player.GetComponent<Throwing>().totalThrows == 0){
                enemyAi.outOfThrows = true;
            }
        }
        if(canRotate){
            transform.Rotate(0,rotationSpeed,0);
        }
        if(playerInPickUpRange){
            transform.position = GameObject.Find("Player").transform.position;
        }

        if(currentIndicator != null)currentIndicator.transform.position = transform.position + (transform.up * .5f);
    }
    
    void OnCollisionEnter(Collision collision){
        collided = true;
        GiveFlipFlops();
        if(!indicatorOn){
            StartCoroutine(InstantiateIndicator());
            indicatorOn = true;
        }

        if(collision.transform.CompareTag("Player")){
            
            Destroy(gameObject);
            Destroy(currentIndicator);
        }
    }

    void CheckForPlayer(){
        playerInPickUpRange = Physics.CheckSphere(transform.position, pickUpRange, whatIsPlayer);
    }

    private void GiveFlipFlops(){
        Collider[] _colliders = Physics.OverlapSphere(transform.position, pickUpRange);
        foreach (Collider _collider in _colliders){
            if(_collider.CompareTag("Player")){
                if(!added){
                    pickUpSFX.Play();
                    PlayerStateHandler playerstate = _collider.GetComponent<PlayerStateHandler>();
                    if(!playerstate.playerIsSafe && canGivePoints){
                        PointsHandler pointsHandler = _collider.GetComponent<PointsHandler>();
                        Throwing throwScript = _collider.GetComponent<Throwing>();
                        pointsHandler.carriedPoints++;
                        throwScript.totalThrows++;
                    }else if(playerstate.playerIsSafe || !canGivePoints){
                        Throwing throwScript = _collider.GetComponent<Throwing>();
                        throwScript.totalThrows++;
                    }
                }
                added = true;
            }

            if(_collider.CompareTag("Enemy")){
                if(!stunAdded && canStunTime >= 0){
                    EnemyStatesHandler enemyStatesHandler = _collider.GetComponent<EnemyStatesHandler>();
                    enemyStatesHandler.addStun(stunTime);
                    stunAdded = true;
                }
            }
            
        }
    }

    IEnumerator InstantiateIndicator(){
        yield return new WaitForSeconds(2f);
        currentIndicator = Instantiate(indicator, transform.position + (transform.up * .5f), indicator.transform.localRotation);
    }
}
