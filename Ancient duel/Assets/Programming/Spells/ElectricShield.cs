using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShield : MonoBehaviour {
    
    #region variables
    public GameObject spell;
    Camera mainCam;
    public bool ReadyToCast;
    GameObject Player;
    bool Casting = false;
    vThirdPersonCamera properties;
    float distance = 7.6f;
    float restoreDistance = 4.6f;
    #endregion
    //Maneja el estado inicial de las variables
    private void Start() {
        ReadyToCast = true;
        Player = GameObject.FindGameObjectWithTag("Player");
        mainCam = Camera.main;
        properties = mainCam.GetComponent<vThirdPersonCamera>();
    }

    private void Update() {
        //Si el jugador está en el suelo, y el hechizo se activó vamos a marcar a Player como Kinematic
        if (Player.GetComponent<vThirdPersonController>().isGrounded) {
            if (spell.activeInHierarchy) {
                Player.GetComponent<vThirdPersonController>().enabled = false;
                Player.GetComponent<vThirdPersonInput>().enabled = false;
                Player.GetComponent<Rigidbody>().isKinematic = true;
                properties.defaultDistance = distance;
                StartCoroutine(Timer());
            }
            else {
                properties.defaultDistance = restoreDistance;
            }
            if (Input.GetKey(KeyCode.E) && Casting) {
                StartCoroutine(Cancelled());
            }
        }
    }

    IEnumerator Timer () {
        Casting = true;
        yield return new WaitForSeconds(10f);
        ReadyToCast = false;
        Casting = false;
        Player.GetComponent<vThirdPersonController>().enabled = true;
        Player.GetComponent<vThirdPersonInput>().enabled = true;
        Player.GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(CD());
        spell.SetActive(false);
    }

    IEnumerator CD () {
        yield return new WaitForSeconds(15f);
        ReadyToCast = true;
    }

    IEnumerator Cancelled () {
        spell.SetActive(false);
        ReadyToCast = false;
        Player.GetComponent<vThirdPersonController>().enabled = true;
        Player.GetComponent<vThirdPersonInput>().enabled = true;
        Player.GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(10f);
        ReadyToCast = true;
    }

}
