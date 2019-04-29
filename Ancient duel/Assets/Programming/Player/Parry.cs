﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof (HealthManager))]
public class Parry : MonoBehaviour {

    /*TODO:
     * Capture the difference in seconds between a defense button being pressed and the impact time of an attack
     * Attack is detected if the player's health is reduced
     * Try to detect an attack by collision and not reduce the health
     * Average for a successful parry: 0.286999948s
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
    public bool perfect;
    public bool blocking;
    public bool collided;
    #endregion
    //Testing vars
    [SerializeField]
    int cntr = 0;
    
    private void Start() {
        health = GetComponent<HealthManager>().Health;
        auxHealth = health;
    }


    private void FixedUpdate() {
        health = GetComponent<HealthManager>().Health;
        perfect = PerfectParry();
        if (Input.GetButtonDown("B")) {
            blocking = true;
            timeDown = 0;
            startTime = true;
        }
        else if (Input.GetButtonUp("B")) {
            blocking = false;
            timeDown = 0;
            startTime = false;
        }
        CountDown();
    }
    //After getting a successful parry, we call Late update to set the perfect parry bool to false;
    private void Update() {
        if (perfect) {
            cntr++;
            perfect = false;
            timeDifference = 0f;
        }
    }
    private void CountDown () {
        if (startTime) {
            timeDown += Time.fixedDeltaTime;
        }
        //Collided
        if (collided) {
            timeDifference = timeDown;
            collided = false;
        }
    }

    private bool PerfectParry () {
        if (timeDifference <= 0.16 && timeDifference != 0) {
            return true;
        }
        else {
            return false;
        }
    }

}
