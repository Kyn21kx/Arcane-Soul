using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent (typeof(Parry))]
public class HealthManager : MonoBehaviour {
    
    #region Variables
    public float Health = 100f;
    private float auxHealth;
    [SerializeField]
    Image healthBar;
    private float proportion;
    #endregion

    private void Start() {
        auxHealth = Health;
    }

    private void LateUpdate() {
        proportion = Health / auxHealth;
        healthBar.fillAmount = proportion;
        Die();
    }

    private void Die () {
        if (Health <= 0) {
            Destroy(gameObject, 0.5f);
            SceneManager.LoadScene(0);
        }
    }
}
