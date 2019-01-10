﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour {
    #region Variables
    Image manaBar;
    Spells spells;
    public int fball = 20, enWhip = 20, electShield = 20, magneticBasic = 20, waterAttack = 20, electricHeavy = 30;
    public float manaAmount = 150f;
    public float manaAux;
    public int regenerationAmount = 20;
    public bool reduced = false;
    float cntr = 1.5f;
    #endregion

    private void Start() {
        manaBar = GameObject.FindGameObjectWithTag("Mana").GetComponent<Image>();
        spells = GameObject.FindGameObjectWithTag("Player").GetComponent<Spells>();
        manaAux = manaAmount;
    }

    private void Update() {
        EvaluateSpells();
    }

    private void EvaluateSpells() {
        #region Drenar Maná para los ataques básicos
        if (!GameObject.Find("Fireball Holder").GetComponent<Fireball>().readytoCast && Input.GetMouseButtonUp(0) && (manaAmount >= fball || manaAmount >= waterAttack)) { //Continuar añadiendo todos los hechizos
            cntr = 1.5f;
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
            reduced = true;
        }
        #endregion

        #region Drenar Maná para ataques fuertes
        if (GameObject.Find("RangedAbilityHolder").GetComponent<RangedAbility>().readyToCast && GameObject.FindGameObjectWithTag("Player").GetComponent<aim>().aiming && Input.GetMouseButton(1) && manaAmount >= 30f) { //Continuar añadiendo todos los hechizos
            cntr = 1.5f;
            switch (spells.abilitySelector) {

                case Spells.Abilities.Heavy1:

                    break;
                case Spells.Abilities.Heavy2:

                    break;
                case Spells.Abilities.Heavy3:

                    break;
                case Spells.Abilities.Heavy4:
                    StartCoroutine(AdjustElectricHeavy1());
                    break;
                default:
                    break;
            }
            reduced = true;
        }
        #endregion

        if (reduced) {
            if (cntr <= 1.5f && cntr > 0f) {
                cntr -= Time.fixedDeltaTime;
                cntr = Mathf.Clamp(cntr, 0f, 1.5f);
            }
            if (cntr <= 0f) {
                if (manaAmount <= manaAux) {
                    manaAmount += (20f * Time.fixedDeltaTime);
                    manaAmount = Mathf.Clamp(manaAmount, 0f, manaAux);
                    System.Convert.ToInt64(manaAmount);
                }
            }
            
        }
    }

    private IEnumerator AdjustElectricHeavy1 () {
        yield return new WaitForSecondsRealtime(3f);
        manaAmount -= electricHeavy;
    }

}
