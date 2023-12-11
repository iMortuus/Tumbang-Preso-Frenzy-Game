
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;
    public EnemyStatesHandler enemyStatesHandler;

    public Transform player;
    public Transform TinCanTransform;
    public GameObject graphics;
    public GameObject tinCanHold;
    public Transform roamingArea;
    public Transform InitialTinCanPosition;
    public TinCan TinCan;
    public LayerMask whatIsGround, whatIsPlayer, whatIsTinCan, whatIsTinCanOgPos;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange, pickUpRange, dropRange;
    public bool playerInSightRange, playerInAttackRange, canInPickUpRange, canInDropRange, isPatroling;
    public bool playerIsSafe, outOfThrows, outOfThrowsChase;

    public bool showSFX;
    [ShowIf("showSFX")] public AudioSource movingSFX;
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        canInPickUpRange = Physics.CheckSphere(transform.position, pickUpRange, whatIsTinCan);
        canInDropRange = Physics.CheckSphere(transform.position, dropRange, whatIsTinCanOgPos);

        handleSFX();

        if(enemyStatesHandler.enemyState == EnemyStates.Normal){
            if(!TinCan.displaced && !TinCan.gotDisplaced && !outOfThrows) Patroling();
            if(TinCan.displaced) GetTinCAn(); 
            if(TinCan.displaced && canInPickUpRange) PickUpCan();
            if(TinCan.displaced && TinCan.gotPickedUp) PlaceTinCan();
            if(TinCan.displaced && canInDropRange && TinCan.gotPickedUp) dropTinCan();
            if(TinCan.gotDisplaced || outOfThrows && !TinCan.gotDisplaced && !TinCan.displaced) ChasePlayer();
            if(playerIsSafe && TinCan.gotDisplaced) PlayerIsSafe();
            if(playerInAttackRange && TinCan.gotDisplaced || playerInAttackRange && outOfThrows && !TinCan.gotDisplaced && !TinCan.displaced) AttackPlayer();
        }
        HandleOutOfThrows();
    }

    private void Patroling()
    {
        isPatroling = true;
        if (!walkPointSet) SearchWalkPoint();
        transform.LookAt(player);
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range( -walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(roamingArea.position.x + randomX, roamingArea.position.y, roamingArea.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        isPatroling = false;
        agent.SetDestination(player.position);
    }
    private void GetTinCAn()
    {
        agent.SetDestination(TinCanTransform.position);
    }
    private void PickUpCan(){
        TinCan.transform.GetComponent<MeshRenderer>().enabled = false;
        graphics.SetActive(false);
        tinCanHold.SetActive(true);
        TinCan.gotPickedUp = true;
    }
    private void PlaceTinCan(){
        agent.SetDestination(InitialTinCanPosition.position);
    }
    private void dropTinCan(){
        agent.SetDestination(transform.position);
        TinCan.transform.GetComponent<MeshRenderer>().enabled = true;
        graphics.SetActive(true);
        tinCanHold.SetActive(false);
        TinCan.gotPickedUp = false;
        TinCan.displaced = false;
        TinCan.gotDisplaced = true;
        ResetTinCanPos();
    }
    private void HandleOutOfThrows(){
        if(player.GetComponent<Throwing>().totalThrows <= 0 ){
            if(!TinCan.displaced && !TinCan.gotDisplaced){
                outOfThrows = true;
            }else if(TinCan.displaced && !TinCan.gotDisplaced || !TinCan.displaced && TinCan.gotDisplaced ){
                outOfThrows = false;
            }
        }
    }

    private void PlayerIsSafe(){
        TinCan.gotDisplaced = false;
        TinCan.displaced = false;
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            TinCan.gotDisplaced = false;
            TinCan.displaced = false;
            ResetTinCanPos();
            player.transform.GetComponent<PlayerStats>().currentHealth = 0f;
            player.transform.GetComponent<PlayerStats>().TakeDamage(player.transform.GetComponent<PlayerStats>().maxHealth);
            /**
            ///Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);**/
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    private void ResetTinCanPos(){
        TinCanTransform.position = TinCanTransform.parent.position;
        TinCanTransform.rotation = Quaternion.Euler(0,0,0);
    }
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    
    void handleSFX(){
        if(TinCan.displaced){
            movingSFX.Play();
        }else if(TinCan.displaced && !TinCan.gotDisplaced){
            movingSFX.Stop();
        }
        if(player.GetComponent<PlayerStateHandler>().states == PlayerStates.Dead){
            movingSFX.Stop();
        }
    }
}
