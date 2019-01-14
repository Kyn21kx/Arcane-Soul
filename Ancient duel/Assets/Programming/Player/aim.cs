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
    public bool aiming;
    float autoAimSpeed = 5f;
    //Canvas crosshair;
    #endregion
    //Instead of Set active do camera.enabled
    private void Start() {
        Player = GameObject.FindGameObjectWithTag("Player");
        thirdPersonController = Player.GetComponent<vThirdPersonController>();
        mainCam = Camera.main;
        Properties = mainCam.GetComponent<vThirdPersonCamera>();
        //crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Canvas>();
        //crosshair.enabled = false;
    }

    void AimMode() {
        AutoAim();
        //Si se conecta un control cambiar el Input
        //Opción de apuntar en izquierda o derecha
        if (Input.GetKeyUp(KeyCode.Q) && !aiming) {
            mainCam.fieldOfView -= 20f;
            Properties.rightOffset += 0.2f;
            Properties.defaultDistance += 3.5f;
            Properties.height += 0.3f;
            //crosshair.enabled = true;
            thirdPersonController.freeRunningSpeed = thirdPersonController.freeRunningSpeed * (0.67f);
            thirdPersonController.freeSprintSpeed = thirdPersonController.freeSprintSpeed * (0.67f);
            aiming = true;
            //StartCoroutine(Stick());
        }
        else if (Input.GetKeyUp(KeyCode.Q) && aiming) {
            mainCam.fieldOfView = 47f;
            mainCam.GetComponent<vThirdPersonCamera>().rightOffset = 0f;
            Properties.defaultDistance = 4.6f;
            Properties.height = 2.73f;
            thirdPersonController.freeRunningSpeed = 7f;
            thirdPersonController.freeSprintSpeed = 8f;
            aiming = false;
            //StartCoroutine(Stick());
            //crosshair.enabled = false;
        }
    }

    private void Update() {
        AimMode();
    }

    void AutoAim() {
        RaycastHit hit;
        float distance = 60f;
        float JoystickValueX = Input.GetAxis("RightJoystickX");
        float JoystickValueY = Input.GetAxis("RightJoystickY");
        if (Player.GetComponent<InputManager>().xbox) {
            if (JoystickValueX != 0 || JoystickValueY != 0) {
                if ((aiming && Physics.Raycast(mainCam.transform.position, mainCam.transform.TransformDirection(Vector3.forward), out hit, distance)) && hit.transform.tag == "Enemy") {
                    Vector3 direction = hit.transform.position - mainCam.transform.position;
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, rotation, autoAimSpeed);
                    
                }
            }
        } 
    }

    IEnumerator Stick () {
        yield return new WaitForFixedUpdate();
    }

}
