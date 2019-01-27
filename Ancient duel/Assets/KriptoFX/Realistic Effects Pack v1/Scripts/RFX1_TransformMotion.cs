using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RFX1_TransformMotion : MonoBehaviour {
    public float Distance = 30;
    public float Speed = 1;
    public enum ActiveSpell { Fireball, WaterBall, MagneticBasic, ElectricBasic, FireMeteor, ElectricHeavy1};
    public enum EnemySpellType { None, Follower, Fixed };
    public EnemySpellType enemySpellType;
    public ActiveSpell selectedSpell;
    //public float Dampeen = 0;
    //public float MinSpeed = 1;
    public float TimeDelay = 0;
    public bool isEnemySpell = false;
    public float RandomMoveRadius = 0;
    public float RandomMoveSpeedScale = 0;
    public float damage;
    public GameObject Target;

    public LayerMask CollidesWith = ~0;


    public GameObject[] EffectsOnCollision;
    BasicEn_Manager Enemy;
    public float CollisionOffset = 0;
    public float DestroyTimeDelay = 5;
    public bool CollisionEffectInWorldSpace = true;
    public GameObject[] DeactivatedObjectsOnCollision;
    [HideInInspector] public float HUE = -1;
    [HideInInspector] public List<GameObject> CollidedInstances;

    private Vector3 startPosition;
    private Vector3 startPositionLocal;
    Transform t;
    Transform targetT;
    private Vector3 oldPos;
    private bool isCollided;
    private bool isOutDistance;
    public bool isFollowing = false;
    private Quaternion startQuaternion;
    [SerializeField]
    private float distanceToPlayer;
    //private float currentSpeed;
    private float currentDelay;
    public int level;
    private const float RayCastTolerance = 0.15f;
    Vector3 VectorAux;
    private bool isInitialized;
    private bool dropFirstFrameForFixUnityBugWithParticles;
    public event EventHandler<RFX1_CollisionInfo> CollisionEnter;
    Vector3 randomTimeOffset;

    void Start()
    {
        //if (isEnemySpell)
        //Target = GameObject.FindGameObjectWithTag("Player");
        t = transform;
        if (Target != null) targetT = Target.transform;
        startQuaternion = t.rotation;
        startPositionLocal = t.localPosition;
        startPosition = t.position;
        oldPos = t.TransformPoint(startPositionLocal);
        Initialize();
        isInitialized = true;
    }

    void OnEnable()
    {
        if (isInitialized) Initialize();
    }

    void OnDisable()
    {
        if (isInitialized) Initialize();
    }

    private void Initialize()
    {
        isCollided = false;
        isOutDistance = false;
        //currentSpeed = Speed;
        currentDelay = 0;
        startQuaternion = t.rotation;
        t.localPosition = startPositionLocal;
        oldPos = t.TransformPoint(startPositionLocal);
        OnCollisionDeactivateBehaviour(true);
        dropFirstFrameForFixUnityBugWithParticles = true;
        randomTimeOffset = Random.insideUnitSphere * 10;
    }

    void Update()
    {
        if (!dropFirstFrameForFixUnityBugWithParticles)
        {
            UpdateWorldPosition();
            //SetTarget();
        }
        else dropFirstFrameForFixUnityBugWithParticles = false;
    }

    void UpdateWorldPosition()
    {
        currentDelay += Time.deltaTime;
        if (currentDelay < TimeDelay)
            return;

        Vector3 randomOffset = Vector3.zero;
        if (RandomMoveRadius > 0)
        {
        
            randomOffset = GetRadiusRandomVector() * RandomMoveRadius;
            if (Target != null)
            {
                if(targetT==null) targetT = Target.transform;
                var fade = Vector3.Distance(t.position, targetT.position) / Vector3.Distance(startPosition, targetT.position);
                randomOffset *= fade;
            }
        }

        var frameMoveOffset = Vector3.zero;
        var frameMoveOffsetWorld = Vector3.zero;
        if (!isCollided && !isOutDistance)
        {
            //currentSpeed = Mathf.Clamp(currentSpeed - Speed*Dampeen*Time.deltaTime, MinSpeed, Speed);

            var currentForwardVector = (Vector3.forward + randomOffset) * Speed * Time.deltaTime;
            frameMoveOffset = t.localRotation * currentForwardVector;
            frameMoveOffsetWorld = startQuaternion * currentForwardVector;
            if (Target != null) {
                var forwardVec = (targetT.position - t.position).normalized; currentForwardVector = (forwardVec + randomOffset) * Speed * Time.deltaTime;
                frameMoveOffset = currentForwardVector;
                frameMoveOffsetWorld = currentForwardVector;
                VectorAux = forwardVec;
            }
        }

        var currentDistance = (t.localPosition + frameMoveOffset - startPositionLocal).magnitude;
        Debug.DrawRay(t.position, frameMoveOffsetWorld.normalized * (Distance - currentDistance));
        RaycastHit hit;
        if (!isCollided && Physics.Raycast(t.position, frameMoveOffsetWorld.normalized, out hit, Distance, CollidesWith))
        {
            if (frameMoveOffset.magnitude + RayCastTolerance > hit.distance)
            {
                isCollided = true;
                t.position = hit.point;
                oldPos = t.position;
                OnCollisionBehaviour(hit);
                OnCollisionDeactivateBehaviour(false);
                //Applied only with water attack
                #region Effects On collision with Player
                if (isEnemySpell && hit.transform.CompareTag("Player")) {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<HealthManager>().Health -= damage;
                }
                #endregion

                #region Effects on collision with enemy
                else {
                    if (hit.transform.CompareTag("Enemy")) {
                        #region Initialize variables
                        Enemy = hit.transform.GetComponent<BasicEn_Manager>();
                        FireEffects fireEffects = gameObject.GetComponent<FireEffects>();
                        LevelManager levelManager = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelManager>();
                        #endregion
                        switch (selectedSpell) {
                            case ActiveSpell.Fireball:
                                #region Evaluate Level
                                level = levelManager.fireBall;
                                #endregion
                                if (Enemy.wet) {
                                    damage = 0;
                                }
                                else {
                                    ApplyBurn(5, 1f, Enemy);
                                }
                                break;
                            case ActiveSpell.WaterBall:
                                #region Evaluate Level
                                level = levelManager.waterBall;
                                #endregion
                                Enemy.wet = true;
                                Enemy.cntr = 5f;
                                break;
                            case ActiveSpell.MagneticBasic:
                                #region Evaluate Level
                                level = levelManager.magneticBasic;
                                #endregion
                                break;
                            case ActiveSpell.ElectricBasic:
                                #region Evaluate Level
                                level = levelManager.electricBasic;
                                #endregion
                                if (Enemy.wet) {
                                    //Increase damage multipler with level
                                    damage += damage;
                                }
                                break;
                            case ActiveSpell.FireMeteor:
                                #region Evaluate Level
                                
                                #endregion
                                if (Enemy.wet) {
                                    damage = 0;
                                }
                                AreaExplosion(5f, 5f, hit.transform);
                                break;
                            case ActiveSpell.ElectricHeavy1:
                                #region Evaluate Level

                                #endregion
                                if (Enemy.wet) {
                                    //Increase damage multipler with level
                                    damage += damage;
                                }
                                break;
                        }
                        hit.transform.GetComponent<BasicEn_Manager>().health -= damage;
                    }
                }
                #endregion
                return;
            }
        }
      
        if (!isOutDistance && currentDistance + RayCastTolerance > Distance)
        {
            isOutDistance = true;
            OnCollisionDeactivateBehaviour(false);

            if (Target == null)
                t.localPosition = startPositionLocal + t.localRotation*(Vector3.forward + randomOffset)*Distance;
            else
            {
                var forwardVec = (targetT.position - t.position).normalized;
                t.position = startPosition + forwardVec * Distance;
            }
            oldPos = t.position;
            return;
        }
      
        t.position = oldPos + frameMoveOffsetWorld;
        oldPos = t.position;
    }

    Vector3 GetRadiusRandomVector()
    {
        var x = Time.time * RandomMoveSpeedScale + randomTimeOffset.x;
        var vecX = Mathf.Sin(x / 7 + Mathf.Cos(x / 2)) * Mathf.Cos(x / 5 + Mathf.Sin(x));

        x = Time.time * RandomMoveSpeedScale + randomTimeOffset.y;
        var vecY = Mathf.Cos(x / 8 + Mathf.Sin(x / 2)) * Mathf.Sin(Mathf.Sin(x / 1.2f) + x * 1.2f);

        x = Time.time * RandomMoveSpeedScale + randomTimeOffset.z;
        var vecZ = Mathf.Cos(x * 0.7f + Mathf.Cos(x * 0.5f)) * Mathf.Cos(Mathf.Sin(x * 0.8f) + x * 0.3f);


        return new Vector3(vecX, vecY, vecZ);
    }

    void OnCollisionBehaviour(RaycastHit hit)
    {
        var handler = CollisionEnter;
        if (handler != null)
            handler(this, new RFX1_CollisionInfo {Hit = hit});
        CollidedInstances.Clear();
        foreach (var effect in EffectsOnCollision)
        {
            var instance = Instantiate(effect, hit.point + hit.normal * CollisionOffset, new Quaternion()) as GameObject;
            CollidedInstances.Add(instance);
            if (HUE > -0.9f)
            {
                var color = instance.AddComponent<RFX1_EffectSettingColor>();
                var hsv = RFX1_ColorHelper.ColorToHSV(color.Color);
                hsv.H = HUE;
                color.Color = RFX1_ColorHelper.HSVToColor(hsv);
            }
            instance.transform.LookAt(hit.point + hit.normal + hit.normal * CollisionOffset);
            if (!CollisionEffectInWorldSpace) instance.transform.parent = transform;
            Destroy(instance, DestroyTimeDelay);
        }
    }

    void OnCollisionDeactivateBehaviour(bool active)
    {
        foreach (var effect in DeactivatedObjectsOnCollision)
        {
           if(effect!=null) effect.SetActive(active);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
            return;

        t = transform;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(t.position, t.position + t.forward*Distance);

    }
    bool Following;
    bool OneTimeFollow = false;
    void SetTarget () {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        float distance = Vector3.Distance(Player.transform.position, gameObject.transform.position);
        Debug.Log(distance);
        switch (enemySpellType) {
            case EnemySpellType.None:
                break;
            case EnemySpellType.Follower:
                if (Player.layer == 8 && !OneTimeFollow) {
                    Following = true;
                }
                else if (Player.layer == 11) {
                    Following = false;
                    OneTimeFollow = true;
                }

                if (Following) {
                    Target = GameObject.FindGameObjectWithTag("Spot");
                }
                else {
                    Target = null;
                }
                
                break;
            case EnemySpellType.Fixed:
                break;
            default:
                break;
        }
    }
    #region Additional Effects
    public void AreaExplosion(float radius, float expDamage, Transform hitEnemy) {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log(enemies);
        float projectileDistance;
        int i = 0;
        foreach (GameObject enemy in enemies) {
            projectileDistance = Vector3.Distance(gameObject.transform.position, hitEnemy.position);
            if (projectileDistance <= radius && enemy != hitEnemy.gameObject) {
                enemy.GetComponent<BasicEn_Manager>().health -= expDamage;
            }
            i++;
        }
    }

    public void ApplyBurn(int tiempo, float damage, BasicEn_Manager en_Manager) {
        for (float i = 0; i < tiempo * Time.deltaTime; i += Time.deltaTime) en_Manager.health -= damage;
    }
    #endregion
    public enum RFX4_SimulationSpace
    {
        Local,
        World
    }

    public class RFX1_CollisionInfo : EventArgs
    {
        public RaycastHit Hit;
    }
}