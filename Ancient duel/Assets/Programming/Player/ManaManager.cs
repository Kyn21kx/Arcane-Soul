using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour {
    public enum CAST { FIRE, LIGHTING, SHIELD };
    #region Variables
    Image manaBar;
    Spells spells;
    CAST selected;
    public float fball = .1f, enWhip = .2f, electShield = .4f;
    #endregion

    private void Start() {
        manaBar = GameObject.FindGameObjectWithTag("Mana").GetComponent<Image>();
        spells = GameObject.FindGameObjectWithTag("Player").GetComponent<Spells>();
    }

    private void Update() {
        EvaluateSpells();
        switch (selected) {
            case CAST.FIRE:
                break;
            case CAST.LIGHTING:
                break;
            case CAST.SHIELD:
                break;
            default:
                break;
        }

    }

    private void EvaluateSpells () {
        switch (spells.current_spell) {
            default:
                break;
        }
    }

}
