using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsHandler : MonoBehaviour
{
    public float carriedPoints;
    public float totalPoints;
    private PlayerStateHandler playerState;

    private void Start() {
        playerState = transform.GetComponent<PlayerStateHandler>();
    }

    private void addCarriedPoints(float amount){
        carriedPoints += amount;
    }
    private void addTotalPoints(float amount){
        totalPoints += amount;
    }
}
