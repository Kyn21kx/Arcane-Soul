using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tracker : MonoBehaviour {

    #region Variables
    public Transform player, enemy;
    public bool locking;
    float fieldofView = 60f;
    #endregion

    private void FixedUpdate() {
        Tracker();
    }

    private void Tracker () {
        Vector2 viewportPlayer = Camera.main.WorldToViewportPoint(player.position);
        Vector2 viewportTarget = Camera.main.WorldToViewportPoint(enemy.position);
        float viewportDistance = Vector2.Distance(viewportPlayer, viewportTarget);
        Debug.Log(viewportDistance);
        Vector3 dir = enemy.position - Camera.main.transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, rot, 50f * Time.fixedDeltaTime);
        if (viewportDistance >= 0.8f) {
            Camera.main.fieldOfView += 80f * Time.fixedDeltaTime;
        }
        else if (viewportDistance <= 0.6) {
            Camera.main.fieldOfView -= 80f * Time.fixedDeltaTime;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, fieldofView, 100f);
        }
    }

}
