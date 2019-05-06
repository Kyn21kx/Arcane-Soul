using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using TMPro;

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
    public bool collided;
    [SerializeField]
    float timeDown;
    public float dmg;
    [SerializeField]
    bool startTime;
    public bool perfect;
    public bool blocking;
    public float healthDifference;
    #endregion
    //Testing vars
    [SerializeField]
    int cntr = 0;
    public TextMeshProUGUI text;
    
    private void Start() {
        health = GetComponent<HealthManager>().Health;
        auxHealth = health;
        
    }
    //Test for UI
    private void LateUpdate() {
        text.text = health.ToString();
    }


    private void FixedUpdate() {
        health = GetComponent<HealthManager>().Health;
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
        VibDown();
    }
    private void CountDown () {
        if (startTime) {
            timeDown += Time.fixedDeltaTime;
        }
        //Collided
        if (collided) {
            timeDifference = timeDown;
            perfect = PerfectParry();
            if (perfect) {
                cntr++;
                vib = true;
                dmg = 0f;
                perfect = false;
                timeDifference = 0f;
            }
            else if (blocking) {
                dmg *= 0.5f;
            }
            GetComponent<HealthManager>().Health -= dmg;
            collided = false;
        }
    }

    

    private bool PerfectParry () {
        //0.16f
        if (timeDifference <= 0.16f && timeDifference != 0f) {
            return true;
        }
        else {
            return false;
        }
    }
    #region Global Aux variables
    float vibrationTimer = 0f;
    bool vib = false;
    #endregion
    
    private void VibDown () {
        if (vib) {
            GamePad.SetVibration(PlayerIndex.One, 0.3f, 0.3f);
            vibrationTimer += Time.fixedDeltaTime;
            if (vibrationTimer > 0.5f) {
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                vibrationTimer = 0;
                vib = false;
            }
            
        }
        
    }
}
