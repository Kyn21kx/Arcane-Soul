using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {

    #region Varibles

    #endregion
    #region statsVars
    public float speed = 20f;
    #endregion

    private void FixedUpdate() {
        _Dash();
    }

    private void _Dash () {
        if (Input.GetButtonDown("B")) {
            //transform.Translate(speed * Time.fixedDeltaTime, 0, 0);
        }
    }

}
