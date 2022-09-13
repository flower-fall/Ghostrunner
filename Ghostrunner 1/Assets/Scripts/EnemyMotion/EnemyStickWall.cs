using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStickWall : MonoBehaviour
{
    public delegate void EnemyStickWallEventHandle(int facing); //委托

    public event EnemyStickWallEventHandle EnemyStickWallEvent;

    [Header("Basic")]
    public Vector2 wallCheckOffset = Vector2.zero;  //检测中心
    public Vector2 wallCheckSize = Vector2.zero;    //检测范围
    public float wallCheckInterval = 1.0f;  //检测间隔时间

    void Update()
    {
        WallCheck();
    }

    private void WallCheck()
    {
        int flag = 0;
        Vector3 wallCheckPoint = transform.position; //获取当前敌人位置中心
        wallCheckPoint.x += wallCheckOffset.x;  //修正中心位置
        wallCheckPoint.y += wallCheckOffset.y;

        //右边(0.5, 0) (0.1, 1.6)
        wallCheckPoint.x += wallCheckInterval / 2;  //再次修正中心位置
        Collider2D colliderInfo = Physics2D.OverlapBox(wallCheckPoint, wallCheckSize, 0, LayerMask.GetMask("Ground"));  //创建一个碰撞体用于检测
        if (colliderInfo != null)   //如果没有碰撞到
        {
            if (EnemyStickWallEvent != null)
            {
                EnemyStickWallEvent(1);
            }
        }
        else
        {
            ++flag;
        }

        //左边(-0.5, 0) (0.1, 1.6)
        wallCheckPoint.x -= wallCheckInterval;
        colliderInfo = Physics2D.OverlapBox(wallCheckPoint, wallCheckSize, 0, LayerMask.GetMask("Ground"));
        if (colliderInfo != null)
        {
            if (EnemyStickWallEvent != null)
            {
                EnemyStickWallEvent(-1);
            }
        }
        else
        {
            ++flag;
        }

        if (flag == 2)
        {
            EnemyStickWallEvent(0);
        }

    }
}
