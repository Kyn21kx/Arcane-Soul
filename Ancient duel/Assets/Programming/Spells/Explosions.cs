using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosions : MonoBehaviour {
    
    public void AreaExplosion(float radius, float expDamage, Transform hitEnemy) {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log(enemies);
        GameObject[] affectedEnemies = null;
        float projectileDistance;
        int i = 0;
        foreach (GameObject enemy in enemies) {
            projectileDistance = Vector3.Distance(gameObject.transform.position, hitEnemy.position);
            if (projectileDistance <= radius) {
                affectedEnemies[i] = enemy;
            }
            i++;
        }
        foreach (var enemy in affectedEnemies) {
            if (enemy != hitEnemy) {
                enemy.GetComponent<BasicEn_Manager>().health -= expDamage;
            }
        }
        Debug.Log(affectedEnemies);
    }

}
