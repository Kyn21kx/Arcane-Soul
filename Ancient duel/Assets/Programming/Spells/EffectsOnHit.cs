using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsOnHit : MonoBehaviour {
    //All effects when an specific spell hits an enemy, except for explosion
    #region Variables
    public float bleedDamage = 10f, burnDamage = 10f, slowAmount = 0.2f;
    //Bleeding variables
    public float seconds;
    public int bleedTicks;
    #endregion
    
    public void Bleed(BasicEn_Manager enemy) {
        //Set seconds to 1
        if (seconds <= 0f && bleedTicks > 0) {
            bleedTicks--;
            enemy.health -= (bleedDamage / 5);
            Debug.Log("One second passed");
            seconds = 1f;
        }
        seconds -= Time.deltaTime;
        //Debug.Log("Test");
        seconds = Mathf.Clamp(seconds, 0f, 1f);
    }

    public void SlowDown() {

    }

    public void Burn() {

    }
    
}
