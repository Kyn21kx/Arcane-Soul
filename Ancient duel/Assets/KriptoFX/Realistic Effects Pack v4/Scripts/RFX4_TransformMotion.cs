using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RFX4_TransformMotion : MonoBehaviour {
    public enum ActiveSpell { Fireball, WaterBall, MagneticBasic, ElectricBasic, FireMeteor, ElectricHeavy1};
    public ActiveSpell selectedSpell;
    public float Distance = 30;
    public float Speed = 1;
    public float Dampeen = 0;
    public float MinSpeed = 1;
    public float TimeDelay = 0;
    public int level = 1;
    public float damage;
    public LayerMask CollidesWith = ~0;
   
    public GameObject[] EffectsOnCollision;
    BasicEn_Manager Enemy;
    public bool isEnemySpell;
    public float CollisionOffset = 0;
    public float DestroyTimeDelay = 5;
    public bool CollisionEffectInWorldSpace = true;
    public GameObject[] DeactivatedObjectsOnCollision;
    [HideInInspector] public float HUE = -1;
    [HideInInspector] public List<GameObject> CollidedInstances; 

    private Vector3 startPositionLocal;
    Transform t;
    private Vector3 oldPos;
    private bool isCollided;
    private bool isOutDistance;
    private Quaternion startQuaternion;
    private float currentSpeed;
    private float currentDelay;
    private const float RayCastTolerance = 0.3f;
    private bool isInitialized;
    private bool dropFirstFrameForFixUnityBugWithParticles;
    public event EventHandler<RFX4_CollisionInfo> CollisionEnter;

    void Start()
    {
        t = transform;
        startQuaternion = t.rotation;
        startPositionLocal = t.localPosition;
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
        currentSpeed = Speed;
        currentDelay = 0;
        startQuaternion = t.rotation;
        t.localPosition = startPositionLocal;
        oldPos = t.TransformPoint(startPositionLocal);
        OnCollisionDeactivateBehaviour(true);
        dropFirstFrameForFixUnityBugWithParticles = true;
      
    }

    void Update()
    {
        if (!dropFirstFrameForFixUnityBugWithParticles)
        {
            UpdateWorldPosition();
        }
        else dropFirstFrameForFixUnityBugWithParticles = false;
    }

    void UpdateWorldPosition()
    {
        currentDelay += Time.deltaTime;
        if (currentDelay < TimeDelay)
            return;

        var frameMoveOffset = Vector3.zero;
        var frameMoveOffsetWorld = Vector3.zero;
        if (!isCollided && !isOutDistance)
        {
            currentSpeed = Mathf.Clamp(currentSpeed - Speed*Dampeen*Time.deltaTime, MinSpeed, Speed);
            var currentForwardVector = Vector3.forward*currentSpeed*Time.deltaTime;
            frameMoveOffset = t.localRotation*currentForwardVector;
            frameMoveOffsetWorld = startQuaternion*currentForwardVector;
        }

        var currentDistance = (t.localPosition + frameMoveOffset - startPositionLocal).magnitude;

        RaycastHit hit;
        if (!isCollided && Physics.Raycast(t.position, t.forward, out hit, 10, CollidesWith))
        {
            if (frameMoveOffset.magnitude + RayCastTolerance > hit.distance)
            {
                isCollided = true;
                t.position = hit.point;
                oldPos = t.position;
                OnCollisionBehaviour(hit);
                OnCollisionDeactivateBehaviour(false);
                #region Effects On collision with Player
                if (isEnemySpell && hit.transform.CompareTag("Player")) {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<HealthManager>().Health -= damage;
                }
                #endregion

                #region Effects on collision with enemy
                else {
                    if (hit.transform.CompareTag("Enemy")) {
                        Enemy = hit.transform.GetComponent<BasicEn_Manager>();
                        LevelManager levelManager = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelManager>();
                        switch (selectedSpell) {
                            case ActiveSpell.Fireball:
                                #region Evaluate Level
                                level = levelManager.fireBall;
                                #endregion
                                if (Enemy.wet) {
                                    damage = 0;
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
                                break;
                            case ActiveSpell.ElectricHeavy1:
                                #region Evaluate Level

                                #endregion
                                break;
                        }
                        hit.transform.GetComponent<BasicEn_Manager>().health -= damage;
                    }
                }
                #endregion
                return;
            }
        }

        if (!isOutDistance && currentDistance > Distance)
        {
            isOutDistance = true;
            t.localPosition = startPositionLocal + t.localRotation*Vector3.forward*Distance;
            oldPos = t.position;
            return;
        }

        t.position = oldPos + frameMoveOffsetWorld;
        oldPos = t.position;
    }



    void OnCollisionBehaviour(RaycastHit hit)
    {
        var handler = CollisionEnter;
        if (handler != null)
            handler(this, new RFX4_CollisionInfo {Hit = hit});
        CollidedInstances.Clear();
        foreach (var effect in EffectsOnCollision)
        {
            var instance = Instantiate(effect, hit.point + hit.normal * CollisionOffset, new Quaternion()) as GameObject;
            CollidedInstances.Add(instance);
            if (HUE > -0.9f)
            {  
                RFX4_ColorHelper.ChangeObjectColorByHUE(instance, HUE);
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
            effect.SetActive(active);
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

    public enum RFX4_SimulationSpace
    {
        Local,
        World
    }

    public class RFX4_CollisionInfo : EventArgs
    {
        public RaycastHit Hit;
    }
}