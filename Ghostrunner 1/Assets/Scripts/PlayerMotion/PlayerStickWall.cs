using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStickWall : MonoBehaviour
{
    public delegate void PlayerStickWallEventHandle(int facing);

    public event PlayerStickWallEventHandle PlayerStickWallEvent;

    [Header("Basic")]
    public Vector2 wallCheckOffset = Vector2.zero;
    public Vector2 wallCheckSize = Vector2.zero;
    public float wallCheckInterval = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WallCheck();
    }

    private void WallCheck() {
        int flag = 0;
        Vector3 wallCheckPoint = transform.position;
        wallCheckPoint.x += wallCheckOffset.x;
        wallCheckPoint.y += wallCheckOffset.y;
        wallCheckPoint.x += wallCheckInterval / 2;
        Collider2D colliderInfo = Physics2D.OverlapBox(wallCheckPoint, wallCheckSize, 0, LayerMask.GetMask("Ground"));
        if (colliderInfo != null) {
            if (PlayerStickWallEvent != null) {
                PlayerStickWallEvent(1);
            }
        }
        else {
            ++flag;
        }

        wallCheckPoint.x -= wallCheckInterval;
        colliderInfo = Physics2D.OverlapBox(wallCheckPoint, wallCheckSize, 0, LayerMask.GetMask("Ground"));
        if (colliderInfo != null) {
            if (PlayerStickWallEvent != null) {
                PlayerStickWallEvent(-1);
            }
        }
        else {
            ++flag;
        }

        if (flag == 2) {
                PlayerStickWallEvent(0);
        }
    }

}
