using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Parry))]
public class HealthManager : MonoBehaviour {
    
    #region Variables
    public float Health = 100f;
    private float auxHealth;
    #endregion

    private void Start() {
        auxHealth = Health;
    }

    private void LateUpdate() {
        //Die();
    }

    private void Die () {
        if (Health <= 0) {
            Destroy(gameObject);
        }
    }
}
