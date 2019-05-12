using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour {
    #region Variables
    Image manaBar;
    public float maxMana;
    public bool reduced = false;
    public bool outOfMana;
    float cntr = 0f;
    private float proportion;
    #endregion
    #region Stats variables
    public float manaAmount = 150f;
    public float regenerationAmount = 10f;
    #endregion

    private void Start() {
        manaBar = GameObject.FindGameObjectWithTag("Mana").GetComponent<Image>();
        maxMana = manaAmount;
    }

    private void FixedUpdate() {
        proportion = manaAmount / maxMana;
        manaBar.fillAmount = proportion;
        Regenerate();
    }

    public void Reduce (float cost) {
        manaAmount -= cost;
        cntr = 0f;
        reduced = true;
    }

    public void Regenerate () {
        if (reduced) {
            cntr += Time.fixedDeltaTime;
            if (cntr >= 1.5f) {
                manaAmount += regenerationAmount * Time.fixedDeltaTime;
                manaAmount = Mathf.Clamp(manaAmount, 0f, maxMana);
            }
        }
    }

}

