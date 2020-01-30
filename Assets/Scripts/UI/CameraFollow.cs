using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform objToFollow; // The Object we're following.

    public float lerpScale = 5f; // How much we scale our smooth lerp movement. 


    void Update() {
        if (objToFollow == null) {
            GameObject maybePlayer = GameObject.FindGameObjectWithTag("Player"); // Try to attach to the player.
            if (maybePlayer != null) {
                objToFollow = maybePlayer.transform;
            }
            return; // Don't try to follow if we don't have a target.
        }
        Vector3 targetPos = Vector3.Lerp(transform.position, objToFollow.transform.position, Time.fixedDeltaTime*lerpScale);
        transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }
}
