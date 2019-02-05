﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class BasicEn_Manager : MonoBehaviour {

    /* Goal: Health System that recieves damage from spells
     *  Recieve Impact by spells
     *  AI (Detect and attack)
     */
    public enum EnemyType {PatrolOn4, ElitePatrol, EliteSingle, SingleBasic, FireProof};
    #region Variables
    public EnemyType enemyType;
    public float Resistance;
    public float health = 100f;
    public float healthAux;
    private Transform player;
    private NavMeshAgent ai;
    public Canvas UISpawner;
    public int level = 0;
    public bool ranged = false;
    bool Detect;
    float distanceToCheckPoint;
    public bool detected = false;
    public int positionIndex = 0;
    public GameObject DamageTxt;
    public GameObject[] covers;
    public Transform[] checkPoints;
    public GameObject[] SpellsAvailable;
    public float radius = 8f;
    public GameObject Pivot;
    public GameObject SpellHolder;
    public GameObject[] PlayerReference;
    public int coverIndex;
    //Spell effects variables
    public float cntr;
    EffectsOnHit onHit;
    public bool wet, burn, stunned, bleeding;
    public int selectedSpell = 0;
    bool ready = true;
    public int tiempoDeArdor;
    public GameObject textMesh;
    public float damagePerBurn;
    #endregion

    private void Start() {
        ai = GetComponent<NavMeshAgent>();
        healthAux = health;
        onHit = GetComponent<EffectsOnHit>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        covers = GameObject.FindGameObjectsWithTag("Cover");
    }

    private void FixedUpdate() {
        Detect_Player();
        Die();
        CompareCoverDistance();
        if (wet) {
            wetTimer();
        }
        EffectsOnHit();
        DamageTaken();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Die () {
        if (health <= 0) {
            ai.isStopped = true;
            Debug.Log("Dead");
            //Insert animation and coroutine here
            ai.gameObject.SetActive(false);
        }

    }
    //Once detected, expand the radius and enter alert mode
    private void Detect_Player () {
        float Distance = Vector3.Distance(player.transform.position, ai.transform.position);
        //Detects the player
        if (Distance <= radius) {
            detected = true;
            if (ranged) {
                ai.SetDestination(covers[coverIndex].transform.position);
                Attack();
            }
            //If the player is running, the speed will increase
        }
        //Patrullar
        else {
            detected = false;   
        }

        if (!detected) {
            ai.SetDestination(checkPoints[positionIndex].transform.position);
            distanceToCheckPoint = Vector3.Distance(checkPoints[positionIndex].transform.position, ai.transform.position);
            if (distanceToCheckPoint < 5) {
                positionIndex++;
                Debug.Log("Llegado");
            }
            if (positionIndex >= checkPoints.Length) {
                positionIndex = 0;
            }
        }
    }

    private void DamageTaken () {
        if (health < healthAux) {
            var instance = Instantiate(textMesh);
            //            textMesh.GetComponent<TextMeshProUGUI>().text = (healthAux - healthAux).ToString();
            instance.GetComponentInChildren<TextProperties>().Text = (healthAux - health).ToString();
            instance.transform.SetParent(UISpawner.transform, false);
            Vector2 positionInScreen = Camera.main.WorldToScreenPoint(transform.position);
            instance.transform.position = positionInScreen;
            
            //float Offset = Random.Range(0.5f, 0.6f);
            //instance.transform.position *= Offset;
            healthAux = health;
        }
        
    }

    //Get an array of the objects near the AI, compare each one's distance and go to the closest one
    private void CompareCoverDistance () {
        float distanceToCover;
        float minorDistance = float.MaxValue;
        for (int i = 0; i < covers.Length; i++) {
            distanceToCover = Vector3.Distance(ai.transform.position, covers[i].transform.position);
            if (distanceToCover < minorDistance) {
                minorDistance = distanceToCover;
                //Debug.Log(minorDistance);
                coverIndex = i;
            }
        }
        
    }

    private void Attack () {
        //Add mana to the enemy and CD for the shot
        if (ranged && detected) {
            //Switch enemyType
            

            switch (enemyType) {
                case EnemyType.PatrolOn4:
                    if (ready) {
                        Vector3 direction = PlayerReference[0].transform.position - Pivot.transform.position;
                        Quaternion rotation = Quaternion.LookRotation(direction);
                        Pivot.transform.rotation = Quaternion.Lerp(Pivot.transform.rotation, rotation, 100f);
                        Instantiate(SpellsAvailable[selectedSpell], Pivot.transform.position, Pivot.transform.rotation);
                        StartCoroutine(BasicCD());
                    }
                    break;
                case EnemyType.ElitePatrol:
                    break;
                case EnemyType.EliteSingle:
                    break;
                case EnemyType.SingleBasic:
                    break;
                case EnemyType.FireProof:
                    break;
                default:
                    break;
            }
        }
    }

    public void wetTimer() {
        //Change time depending on level
        cntr -= Time.deltaTime;
        cntr = Mathf.Clamp(cntr, 0f, 5f);
        if (cntr <= 0f) {
            wet = false;
        }
    }

    private void EffectsOnHit () {
        if (bleeding) {
            onHit.Bleed(this);
            if (onHit.bleedTicks <= 0) {
                bleeding = false;
            }
        }
        if (burn) {
            onHit.Burn(this);
            if (onHit.burnTicks <= 0) {
                burn = false;
            }
        }
    }

    private IEnumerator BasicCD () {
        ready = false;
        yield return new WaitForSeconds(2f);
        ready = true;
    }

}
