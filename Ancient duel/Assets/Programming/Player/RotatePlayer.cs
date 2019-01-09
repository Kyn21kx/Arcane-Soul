using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayer : MonoBehaviour {

    #region Variables
    public float camRotationY;
    public Transform transformCam;
    #endregion

    private void Start() {
        transformCam = Camera.main.gameObject.GetComponent<Transform>();  
    }

    private void Update() {
        camRotationY = transformCam.rotation.y;
        
    }

}
