using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class En_Shoot : MonoBehaviour {

    #region Variables
    public GameObject spell;
    [SerializeField]
    private Transform player;
    #endregion
    //Inicialización
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate() {
        
    }

    private void Shoot () {

    }

}
