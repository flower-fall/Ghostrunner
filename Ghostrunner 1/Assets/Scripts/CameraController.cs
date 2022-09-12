using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Alter")]
    public float moveSpeed = 3.2f;
    public float maxDistanceLimitX = 10.0f;
    public float maxDistanceLimitY = 6.0f;
    public float minDistanceLimit = 0.2f;

    private Transform player = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Init(Transform player) {
        this.player = player;
    }

    public void Move() {
        if (!player) {
            return;
        }

        Vector3 pos = transform.position;
        if ((pos - player.position).magnitude > minDistanceLimit) {
            pos += (player.position - pos) * moveSpeed * Time.deltaTime;
            if (player.position.x - pos.x > maxDistanceLimitX) {
                pos.x = player.position.x - maxDistanceLimitX;
            }
            else if (pos.x - player.position.x > maxDistanceLimitX) {
                pos.x = player.position.x + maxDistanceLimitX;
            }
            if (player.position.y - pos.y > maxDistanceLimitY) {
                pos.y = player.position.y - maxDistanceLimitY;
            }
            else if (pos.y - player.position.y > maxDistanceLimitY) {
                pos.y = player.position.y + maxDistanceLimitY;
            }
            pos.z = -10;
            transform.position = pos;
        }
    }

}
