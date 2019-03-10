using System.Collections;
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
    float distanceToCheckPoint;
    public bool detected = false;
    public int positionIndex = 0;
    public GameObject DamageTxt;
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
    private float randMoveAroundPlayerX, randMoveAroundPlayerZ;
    float xDistance;
    //Variable when seen or heard something, but not quite detected the enemy
    public bool alert = false;
    bool arrivedToPosAux = false;
    #endregion

    private void Start() {
        ai = GetComponent<NavMeshAgent>();
        healthAux = health;
        onHit = GetComponent<EffectsOnHit>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate() {
        Die();
        DetectPlayer();
        if (wet) {
            wetTimer();
        }
        EffectsOnHit();
        DamageTaken();
        Attack();
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

    private void Attack () {
        //Add mana to the enemy and CD for the shot
        if (ranged && detected) {
            //Switch enemyType
            switch (enemyType) {
                case EnemyType.PatrolOn4:
                    if (ready) {
                        Vector3 direction = PlayerReference[0].transform.position - Pivot.transform.position;
                        Quaternion rotation = Quaternion.LookRotation(direction);
                        Pivot.transform.rotation = Quaternion.Lerp(Pivot.transform.rotation, rotation, 10000 * Time.fixedDeltaTime);
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
    //Variable para controlar el tiempo en el que el enemigo permanece en una posición antes de cambiar a otra, sin importar la distancia del jugador
    public float stayTime = 0f;
    private void DetectPlayer () {
        #region Evaluate distance and set boolean
        float disToPlayer = Vector3.Distance(player.transform.position, transform.position);
        //Evaluar la distancia de detección
        if (disToPlayer < radius) {
            detected = true;
        }
        #endregion
        if (detected) {
            //Expand radius
            #region Provisional variables
            float rangedMeleeDistance = 10f;
            #endregion
            xDistance = transform.position.x - player.position.x;
            switch (ranged) {
                case true:
                    //Evaluate Shooting Position
                    MovingWaitTime();
                    #region Melee section for ranged enemy
                    //Different interactions can be controlled by enums
                    if (disToPlayer < rangedMeleeDistance) {

                    }
                    #endregion
                    break;

                case false:
                    //Debug.Log("xDistance is: " + xDistance);
                    if (xDistance < -4) {
                        randMoveAroundPlayerX = -8f;
                        randMoveAroundPlayerZ = -8f;
                    }
                    else {
                        randMoveAroundPlayerX = 8f;
                        randMoveAroundPlayerZ = 8f;
                    }
                    //MoveAroundPlayer();
                    //If the AI finds a wall, then it'll move in the other direction
                    Vector3 newPos = new Vector3(player.position.x + randMoveAroundPlayerX, player.position.y, player.position.z + randMoveAroundPlayerZ);
                    ai.SetDestination(newPos);
                    break;
            }
            
        }
        else {

        }
    }

    private IEnumerator BasicCD () {
        ready = false;
        yield return new WaitForSeconds(2f);
        ready = true;
    }

    private void MovingWaitTime () {
        GameObject[] positionsAvailable = GameObject.FindGameObjectsWithTag("ShootingPosition");
        float minDis = float.MaxValue;
        Vector3 finalPositionAux = new Vector3(0, 0, 0);
        foreach (GameObject position in positionsAvailable) {
            float distance = Vector3.Distance(player.position, position.transform.position);
            if (distance < minDis) {
                minDis = distance;
                finalPositionAux = position.transform.position;
            }
        }
        //stayTime += Time.fixedDeltaTime;
        //if (stayTime > 1f) {
            ai.SetDestination(finalPositionAux);
        //}
    }

    #region Provisional Variables
    float timeDown = 0f;
    #endregion

    private void MoveAroundPlayer () {
        //Set parameters to define destinations
        //Set Time
        timeDown += Time.fixedDeltaTime;
        if (timeDown >= 2f) {
            randMoveAroundPlayerX = Random.Range(-30f, 30f);
            randMoveAroundPlayerZ = Random.Range(15f, 18f);
            timeDown = 0f;
        }
    }

}
