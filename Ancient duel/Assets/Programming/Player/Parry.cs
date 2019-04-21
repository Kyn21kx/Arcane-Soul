using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof (HealthManager))]
public class Parry : MonoBehaviour {

    /*TODO:
     * Capture the difference in seconds between a defense button being pressed and the impact time of an attack
     * Attack is detected if the player's health is reduced
     */

    #region Variables
    public float timeDifference;
    float health;
    #endregion

    private void Start() {
        health = GetComponent<HealthManager>().Health;
    }


    private void FixedUpdate() {
        
    }

}
