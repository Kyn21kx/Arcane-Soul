using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

    #region Variables
    public float Health = 100f;
    #endregion

    private void Start() {
        
    }

    private void LateUpdate() {
        Die();
    }

    private void Die () {
        Debug.Log(Health);
        if (Health <= 0f) {
            Destroy(gameObject);
        }
    }
}
