using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Variables
    public bool xbox, ps4, pc;

    #endregion

    private void Update() {
        if (Input.GetButton("StartXbox")) {
            xbox = true;
            ps4 = false;
            pc = false;
            //Time.timeScale = 1;
        }
        else if (Input.GetButton("StartPS4")) {
            xbox = false;
            ps4 = true;
            pc = false;
            //Time.timeScale = 1;
        }
        else if (Input.anyKey) {
            xbox = false;
            ps4 = false;
            pc = true;
            //Time.timeScale = 1;
        }
    }

}
