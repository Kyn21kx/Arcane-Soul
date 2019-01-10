using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spells : MonoBehaviour {

    // Goal: Allow the player to cast spells in relative position to the camera, manage the cooldown for all these spells and make a selection system
    public enum Types {Fire, Water, Magnetic, Electric};
    public enum Abilities {Heavy1, Heavy2, Heavy3, Heavy4};
    #region Variables
    public int current_spell = 0;
    public Types typeSelector;
    public Abilities abilitySelector;
    public bool heavyCast = false;
    Fireball fireball;
    RangedAbility rangedAbility;
    public GameObject fball, waterAttack, electricAttack, magneticAttack, fireTornado, HeavyElectric1;
    GameObject Player;
    public TextMeshProUGUI txt;
    public GameObject AbilitySelectorGUI;
    public bool Casted = false;
    #endregion

    //Set the spells as disabled

    private void Start() {
        Player = GameObject.FindGameObjectWithTag("Player");
        fireball = GameObject.Find("Fireball Holder").GetComponent<Fireball>();
        rangedAbility = GameObject.Find("RangedAbilityHolder").GetComponent<RangedAbility>();
    }

    //Tab changes the spell quickly // Shift + Tab Opens a roulette

    private void Update() {
        //Add GUI to select the spells
        //txt.text = "Spell: " + current_spell;
        //Replace with roulette when you press down Tab (FOR THE ABILITIES)
        #region TabSelecting
        //Disable Aim
        if (Input.GetKeyDown(KeyCode.Tab)) {
            AbilitySelectorGUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0.4f;
        }
        if (Input.GetKeyUp(KeyCode.Tab)) {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            AbilitySelectorGUI.SetActive(false);
        }
        #endregion
        TypeSelecting();
        Cast();

    }

    void TypeSelecting () {
        //Add controller input
        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1)) {
            typeSelector = Types.Fire;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2)) {
            typeSelector = Types.Water;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3)) {
            typeSelector = Types.Magnetic;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4) || Input.GetKeyUp(KeyCode.Keypad4)) {
            typeSelector = Types.Electric;
        }
    }

    private void Cast () {
        aim aim = GetComponent<aim>();
        if (aim.aiming) {
            //Si se detecta un control entonces el disparo es con RB o R1
            #region Basic Attack
            //Cambiar la cantidad de maná por cada hechizo
            if (Input.GetMouseButton(0) && Player.GetComponent<ManaManager>().manaAmount >= 20) {
                Casted = true;
                switch (typeSelector) {
                    case Types.Fire:
                        if (fireball.readytoCast) {
                            fireball.Casted();
                            Instantiate(fball, GameObject.Find("Fireball Holder").transform.position, GameObject.Find("Fireball Holder").transform.rotation);
                        }
                        break;
                    case Types.Water:
                        if (fireball.readytoCast) {
                            fireball.Casted();
                            Instantiate(waterAttack, GameObject.Find("Fireball Holder").transform.position, GameObject.Find("Fireball Holder").transform.rotation);
                        }
                        break;
                    case Types.Magnetic:
                        if (fireball.readytoCast) {
                            fireball.Casted();
                            Instantiate(magneticAttack, GameObject.Find("Fireball Holder").transform.position, GameObject.Find("Fireball Holder").transform.rotation);
                        }
                        break;
                    case Types.Electric:
                        if (fireball.readytoCast) {
                            fireball.Casted();
                            Instantiate(electricAttack, GameObject.Find("Fireball Holder").transform.position, GameObject.Find("Fireball Holder").transform.rotation);
                        }
                        break;
                    default:
                        break;
                }
            }
            #endregion
            #region Heavy Attack (Ability)
            if (Input.GetMouseButton(1)) {
                switch (abilitySelector) {
                    case Abilities.Heavy1:
                        
                        break;
                    case Abilities.Heavy2:
                        break;
                    case Abilities.Heavy3:
                        break;
                    case Abilities.Heavy4:
                        if (rangedAbility.readyToCast) {
                            rangedAbility.readyToCast = false;
                            HeavyElectric1.SetActive(true);
                            StartCoroutine(rangedAbility.CoolDown());
                        }
                        break;
                    default:
                        break;
                }
            }
            #endregion
        }
    }

    public void AbilitySelection (int id) {
        switch (id) {
            case 1:
                abilitySelector = Abilities.Heavy1;
                break;
            case 2:
                abilitySelector = Abilities.Heavy2;
                break;
            case 3:
                abilitySelector = Abilities.Heavy3;
                break;
            case 4:
                abilitySelector = Abilities.Heavy4;
                break;
        }
    }

}
