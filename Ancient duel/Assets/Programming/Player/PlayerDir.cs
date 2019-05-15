using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDir : MonoBehaviour {

    #region Variables
    [SerializeField]
    Vector2 xyInput;
    #endregion

    private void Update() {
        xyInput = GetComponent<SmoothMovement>().input;
        xyInput = xyInput.normalized;
        Identify();
    }

    private void Identify () {
        Debug.Log("Forward.x: " + transform.forward.x);
        Debug.Log("xyInput.y: " + xyInput.x);
    }

}
