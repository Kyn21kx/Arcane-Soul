using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_En : MonoBehaviour {
    //TODO: Movement (random), Hearing function.
    #region Variables
    public float speed = 3f;
    float walkRadius = 900f;
    public bool randomMove = false;
    NavMeshAgent agent;
    [SerializeField]
    Transform[] positions;
    float distanceToPos;
    int positionIndex = 0;
    bool timeOnce = false;
    float randomTimeToMove;
    public float hearingRadius = 10f;
    BasicEn_Manager _enemy;
    #endregion

    private void Start() {
        _enemy = GetComponent<BasicEn_Manager>();
        agent = GetComponent<NavMeshAgent>();
        randomTimeToMove = Random.Range(2f, 6f);
    }

    private void FixedUpdate() {
        Movement();
    }

    private void Movement () {
        #region Random
        if (randomMove) {
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
            Vector3 finalPos = hit.position;
            agent.SetDestination(finalPos);
        }
        #endregion
        #region Defined
        else {
            agent.SetDestination(positions[positionIndex].position);
            distanceToPos = Vector3.Distance(positions[positionIndex].position, gameObject.transform.position);
            if (distanceToPos <= 2) {
                randomTimeToMove -= Time.fixedDeltaTime;
                Mathf.Clamp(randomTimeToMove, 0f, 10f);
                if (randomTimeToMove <= 0) {
                    positionIndex++;
                    randomTimeToMove = Random.Range(2f, 6f);
                }
            }
            if (positionIndex >= positions.Length) {
                positionIndex = 0;
            }
        }
        #endregion
    }

    private void HearSpells() {
        GameObject[] spells = GameObject.FindGameObjectsWithTag("Spell");
        foreach (GameObject spell in spells) {
            //Distance of the spell and if it's within a radius
        }
    }

}
