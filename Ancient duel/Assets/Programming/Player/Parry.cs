using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof (HealthManager))]
public class Parry : MonoBehaviour {

    /*TODO:
     * Capture the difference in seconds between a defense button being pressed and the impact time of an attack
     * Attack is detected if the player's health is reduced
     * Try to detect an attack by collision and not reduce the health
     */

    #region Variables
    public float timeDifference;
    [SerializeField]
    float auxHealth;
    float health;
    [SerializeField]
    float timeDown;
    [SerializeField]
    bool startTime;
    #endregion

    private void Start() {
        health = GetComponent<HealthManager>().Health;
        auxHealth = health;
    }


    private void FixedUpdate() {
        if (Input.GetButtonDown("B")) {
            //timeDifference = 0;
            timeDown = 0;
            startTime = true;
        }
        CountDown();
    }

    private void CountDown () {
        if (startTime) {
            timeDown += Time.fixedDeltaTime;
        }
        if (health < auxHealth) {
            startTime = false;
            timeDifference = timeDown;
        }
    }

}
