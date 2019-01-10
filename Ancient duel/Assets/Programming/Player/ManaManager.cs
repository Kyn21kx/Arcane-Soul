using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour {
    #region Variables
    Image manaBar;
    Spells spells;
    public int fball = 20, enWhip = 20, electShield = 20, magneticBasic = 20, waterAttack = 20;
    public int manaAmount = 100;
    public int manaAux;
    #endregion

    private void Start() {
        manaBar = GameObject.FindGameObjectWithTag("Mana").GetComponent<Image>();
        spells = GameObject.FindGameObjectWithTag("Player").GetComponent<Spells>();
        manaAux = manaAmount;
    }

    private void Update() {
        EvaluateSpells();
        
    }

    private void EvaluateSpells () {
        #region Drenar Maná para los ataques básicos
        if (!GameObject.Find("Fireball Holder").GetComponent<Fireball>().readytoCast && Input.GetMouseButtonUp(0) && manaAmount >= 20) {

            switch (spells.typeSelector) {
                case Spells.Types.Fire:
                    manaAmount -= fball;
                    break;
                case Spells.Types.Water:
                    manaAmount -= waterAttack;
                    break;
                case Spells.Types.Magnetic:
                    manaAmount -= magneticBasic;
                    break;
                case Spells.Types.Electric:
                    manaAmount -= enWhip;
                    break;
            }
        }
        else if (manaAmount <= 19) {
            GameObject.Find("Fireball Holder").GetComponent<Fireball>().readytoCast = false;
            StartCoroutine(Regenerate_Mana());
        }
        #endregion
    }

    IEnumerator Regenerate_Mana () {
        yield return new WaitForSecondsRealtime(5f);
        if (manaAmount < manaAux) {
            manaAmount++;
        }
        yield return new WaitForSecondsRealtime(2f);
    }

}
