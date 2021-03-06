﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.CharacterController;

public class RangedAbility : MonoBehaviour {

    #region Variables
    public bool readyToCast = true;
    public float CD;
    private vThirdPersonCamera mainCamera;
    #endregion

    private void Start() {
        mainCamera = Camera.main.GetComponent<vThirdPersonCamera>();
    }

    public IEnumerator CoolDown (float time, bool IsSetActive, GameObject @object) {
        yield return new WaitForSeconds(time);
        if (IsSetActive) {
            @object.SetActive(false);
        }
        readyToCast = true;
    }

    public void AdjustCamera() {
        
    }

}
