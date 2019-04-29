using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Parry))]
public class HealthManager : MonoBehaviour {
    
    #region Variables
    public float Health = 100f;
    #endregion

    private void LateUpdate() {
        Die();
    }

    public void TakeDamage (float damage) {
        if (GetComponent<Parry>().perfect) {
            damage = 0f;
        }
        Health -= damage;
    }

    private void Die () {
        Debug.Log(Health);
        if (Health <= 0f) {
            Destroy(gameObject);
        }
    }
}
