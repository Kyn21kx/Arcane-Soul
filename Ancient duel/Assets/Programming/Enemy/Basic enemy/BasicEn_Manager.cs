using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEn_Manager : MonoBehaviour {

    /* Goal: Health System that recieves damage from spells
     *  Recieve Impact by spells
     *  AI (Detect and attack)
     */
    public enum EnemyType {PatrolOn4, ElitePatrol, EliteSingle, SingleBasic, FireProof};
    #region Variables
    public EnemyType enemyType;
    public float health = 100f;
    private Transform player;
    private NavMeshAgent ai;
    public int level = 0;
    public bool ranged = false;
    bool Detect;
    float distanceToCheckPoint;
    public bool detected = false;
    public int positionIndex = 0;
    public GameObject[] covers;
    public Transform[] checkPoints;
    public GameObject[] SpellsAvailable;
    public float radius = 8f;
    public int coverIndex;
    public int selectedSpell = 0;
    RaycastHit hit;
    #endregion

    private void Start() {
        ai = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        covers = GameObject.FindGameObjectsWithTag("Cover");
    }

    private void FixedUpdate() {
        Detect_Player();
        Die();
        CompareCoverDistance();
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
        
    }

    //Get an array of the objects near the AI, compare each one's distance and go to the closest one
    private void CompareCoverDistance () {
        
        float distanceToCover;
        float minorDistance = float.MaxValue;
        for (int i = 0; i < covers.Length; i++) {
            distanceToCover = Vector3.Distance(ai.transform.position, covers[i].transform.position);
            if (distanceToCover < minorDistance) {
                minorDistance = distanceToCover;
                Debug.Log(minorDistance);
                coverIndex = i;
            }
        }
        
    }

    private void Attack () {
        //Add mana to the enemy and CD for the shot
        if (ranged && detected) {
            //Switch enemyType
            bool ready = true;
            RaycastHit hit;
            switch (enemyType) {
                case EnemyType.PatrolOn4:
                    Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 30f);
                    if (ready && hit.transform.CompareTag("Player")) {
                        Instantiate(SpellsAvailable[selectedSpell], gameObject.transform.position, gameObject.transform.rotation);
                        ready = false;
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

}
