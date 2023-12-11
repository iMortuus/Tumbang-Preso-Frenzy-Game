using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinCan : MonoBehaviour
{
    //private float resetTimer;
    //[SerializeField] private float resetTime = 5;
    //private float rotationResetSpeed = 10;
    
    #region Hidden 
    [HideInInspector] public Quaternion  originalRotationValue;
    [HideInInspector] public bool displaced;
    [HideInInspector] public bool gotDisplaced;
    [HideInInspector] public bool gotPickedUp;

    #endregion

    #region  Objecect References
    [Header("GameObject References")]
    public Transform parent;
    public Transform enemy;
    public Transform Barrier;
    public GameObject indicator;
    
    #endregion
    
    #region Script References
    [Header("Script References")]
    public Throwing throwingScript;
    public PointsHandler pointsHandler;
    public PlayerStateHandler playerStateHandler;
    #endregion
    
    #region GUI
    [Header("Game UI")]
    public GameObject strikeGUI;
    #endregion

    #region  VFX
    [Header("Visual Effects")]
    public GameObject strikeVFX;
    #endregion

    #region  SFX
    [Header("Sound Effects")]
    public AudioSource bgmDefault;
    public AudioSource bgmChase;
    public AudioSource strikeSFX;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.transform;
        originalRotationValue = transform.rotation;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        Collider collider = transform.GetComponent<Collider>();
        Rigidbody rb = transform.GetComponent<Rigidbody>();
        if(!playerStateHandler.playerIsSafe && !displaced){
            collider.enabled = false;
            rb.isKinematic = true;
        }else{
            collider.enabled = true;
            rb.isKinematic = false;
        }
        if(transform.localRotation != originalRotationValue){
            if(!gotDisplaced){
                if(!displaced){
                    StartCoroutine(HandleStrikeGUI());
                    strikeVFX.active = true;
                    indicator.active = false;
                    strikeSFX.Play();
                    pointsHandler.carriedPoints += 5f;
                    //throwingScript.totalThrows = 0;
                }
                displaced = true;
            }
            transform.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
        }else
        {
            displaced = false;
            strikeVFX.active = false;
            indicator.active = true;
            transform.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = true;
        }
        DeactivateBarrier();
    }

    void DeactivateBarrier(){
        if(displaced && !gotDisplaced){
            Barrier.GetComponent<Collider>().enabled = false;
        }
    }

    [System.Obsolete]
    IEnumerator HandleStrikeGUI(){
        strikeGUI.active = true;
        yield return new WaitForSeconds(2f);
        strikeGUI.active = false;
    }
    
}
