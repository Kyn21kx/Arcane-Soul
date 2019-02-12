using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aim : MonoBehaviour {

    #region Variables
    [SerializeField]
    Camera mainCam;
    GameObject Player;
    vThirdPersonController thirdPersonController;
    vThirdPersonCamera Properties;
    public GameObject holder;
    public bool aiming;
    float autoAimSpeed = 5f;
    Canvas crosshair;
    #endregion
    //Instead of Set active do camera.enabled
    private void Start() {
        Player = GameObject.FindGameObjectWithTag("Player");
        thirdPersonController = Player.GetComponent<vThirdPersonController>();
        mainCam = Camera.main;
        Properties = mainCam.GetComponent<vThirdPersonCamera>();
        crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Canvas>();
        crosshair.enabled = false;
    }

    void AimMode() {
       AutoAim();
        //Si se conecta un control cambiar el Input
        //Opción de apuntar en izquierda o derecha
        if (Input.GetKeyUp(KeyCode.Q) && !aiming) {
            mainCam.fieldOfView -= 20f;
            Properties.rightOffset = 0.1f;
            Properties.defaultDistance = 7f;
            Properties.height = 3.78f;
            crosshair.enabled = true;
            thirdPersonController.freeRunningSpeed = thirdPersonController.freeRunningSpeed * (0.5f);
            thirdPersonController.freeSprintSpeed = thirdPersonController.freeSprintSpeed * (0.5f);
            aiming = true;
            //StartCoroutine(Stick());
        }
        else if (Input.GetKeyUp(KeyCode.Q) && aiming) {
            mainCam.fieldOfView = 47f;
            mainCam.GetComponent<vThirdPersonCamera>().rightOffset = 0f;
            Properties.defaultDistance = 7.53f;
            Properties.height = 3.73f;
            thirdPersonController.freeRunningSpeed = 7f;
            thirdPersonController.freeSprintSpeed = 8f;
            aiming = false;
            //StartCoroutine(Stick());
            crosshair.enabled = false;
            
        }
    }

    private void Update() {
        AimMode();
        //FixCrosshairPosition();
    }

    void AutoAim() {
        RaycastHit hit;
        Transform origin, to;
        origin = mainCam.transform;
        float distance = 60f;
        if ((aiming && Physics.Raycast(mainCam.transform.position, mainCam.transform.TransformDirection(Vector3.forward), out hit, distance)) && hit.transform.CompareTag("Enemy")) {
            to = hit.transform;
            /*Vector3 dir = hit.transform.position - mainCam.transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, rot, 50f * Time.fixedDeltaTime);
            */
            //Get position of the enemy
            //Adjust the spell holder to it
            
        }
        else {
            
            Properties.xMouseSensitivity = 3.5f;
            Properties.yMouseSensitivity = 3.5f;
        }
    }

    private void FixCrosshairPosition () {
        var crossPosition = crosshair.GetComponentInChildren<RectTransform>();
        crossPosition.transform.position = mainCam.WorldToScreenPoint(holder.transform.position);
    }

}
