using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class TextProperties : MonoBehaviour {

    #region Variables
    public string Text;
    public bool burning;
    #endregion

    private void Start() {
        Destroy(this.gameObject, 0.917f);
    }

    void Update () {
        GetComponent<TextMeshProUGUI>().text = Text;
        if (Convert.ToInt16(Text) > 19) {
            GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
        if (Convert.ToInt16(Text) > 29) {
            GetComponent<TextMeshProUGUI>().color = Color.red;
        }
	}
	
}
