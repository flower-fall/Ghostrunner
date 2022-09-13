using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMotion : MonoBehaviour
{
    [Header("Debug")]
    public int motionState = 0; //当前运动状态：站立、走路、射击
    public bool isGround = false;   //是否在地面
    public bool isWall = false;   //是否碰到墙壁

    [Header("Basic")]
    public Transform enemyModel = null;
    public Transform groundCheckPoint = null;   //地面监测中心
    public Sprite bullets = null; //子弹图像
    public Vector2 speedLockX = Vector2.zero;   //x轴速度上限
    public Vector2 speedLockY = Vector2.zero;   //y轴速度上限

    [Header("Alter")]
    public float moveSpeed = 0.3f;
    public float gravity = 9.5f;
    public float speedLimit = 20.0f;

    public float groundCheckWidth = 0.8f;   //地面监测宽
    public float groundCheckHeight = 0.1f;  //地面监测高

    private Vector3 speed = Vector3.zero;   //三个方向的速度
    private int facing = 1; //朝向,1右，-1左，仅与人物朝向绘图有关
    private int moveDirection = 1;  //移动方向,-1左1右，与人移动有关

    //运动状态
    private const int MOTION_STAND = 0; //站立
    private const int MOTION_WALK = 1;  //走路
    private const int MOTION_CATCHWALL = 2;  //贴墙
    private const int MOTION_SHOOT = 3; //射击

    //子弹对象
    private GameObject bullteObject = null;

    private void Awake()
    {
        //Camera.main.GetComponent<CameraController>().Init(transform);
        EnemyStickWall esw = GetComponent<EnemyStickWall>();
        if (esw)
        {
            esw.EnemyStickWallEvent += CatchWall;
        }
    }

    void Start()
    {
        //设置速度上限
        speedLockX = new Vector2(-speedLimit, speedLimit);
        speedLockY = new Vector2(-speedLimit, speedLimit);
    }

    // Update is called once per frame
    void Update()
    {
        GroundDetect(); //地面监测
       // WallDetect();   //墙壁监测
        Walk(moveDirection);        //走路方向
        Move();
    }

    private void WallDetect()
    {
        if(motionState > MOTION_STAND)
        {
            return;
        }

        Vector2 wallCheckSize = enemyModel.GetComponent<BoxCollider2D>().size;   //检查边界
        wallCheckSize.y = (float)(wallCheckSize.y * 0.9);   //垂直方向适当缩小
        Vector3 checkPos = this.transform.position; //当前角色的中心
        Collider2D colliderInfo = Physics2D.OverlapBox(checkPos, wallCheckSize, 0, LayerMask.GetMask("Ground"));

        if(colliderInfo != null)
        {
            if(speed.x != 0)    //碰到墙壁就改变方向
            {
                facing = -facing;
            }
            isWall = true;
        }
        else
        {
            isWall = false;
        }
    }

    private void GroundDetect()
    {
        if (motionState > MOTION_STAND)
        {
            return;
        }
        Vector2 groundCheckSize = new Vector2(groundCheckWidth, groundCheckHeight);
        Vector3 checkPos = groundCheckPoint.transform.position;
        Collider2D colliderInfo = Physics2D.OverlapBox(checkPos, groundCheckSize, 0, LayerMask.GetMask("Ground"));
        if(colliderInfo != null)
        {
            if (speed.y < 0)
            {
                speed.y = 0;
            }
            isGround = true;
        }
        else {
            if (speed.y < 0)    //正在下降
            {
                speed.y -= 3.0f * gravity * Time.deltaTime; //3倍重力
            }
            else    //正在上升
            {
                speed.y -= 3.0f * gravity * Time.deltaTime; //1倍重力
            }
            isGround = false;
        }
    }

    private void CatchWall(int f)
    {
        if (motionState > MOTION_STAND)
        {
            return;
        }
        /**
         * f为0就解锁速度，-1锁定左边速度，1锁定右边速度
         */
        if (f == 0)
        {
            motionState = MOTION_STAND;
            FreeSpeed(1);
        }
        else if (f == -1)
        {
            LockSpeed(1, new Vector2(0, speedLockX.y));
            moveDirection = -moveDirection;
        }
        else if (f == 1)
        {
            LockSpeed(1, new Vector2(speedLockX.x, 0));
            moveDirection = -moveDirection;
        }
        /**
         * 传入的f如果与面朝方向相同，并且位移方向与朝向方向相同
         * 则执行“贴墙”操作，锁定y的速度
         * 否者解锁y速度
         */
        /*if (facing == f && moveDirection * facing > 0)
        {
            motionState = MOTION_CATCHWALL;
            LockSpeed(2, Vector2.zero);
        }
        else
        {
            motionState = MOTION_STAND;
            FreeSpeed(2);
        }*/

    }

    private void Walk(int dir)  //改变朝向和当前的速度方向
    {
        if (motionState > MOTION_WALK)
        {
            return;
        }
        FlipEnemy(dir); //敌人朝向
        speed.x = dir * moveSpeed;
    }

    private void Move() //移动位置
    {
        if (speed.x > speedLockX.y)
        {
            speed.x = speedLockX.y;
            facing = 1;
        }
        else if (speed.x < speedLockX.x)
        {
            speed.x = speedLockX.x;
            facing = -1;
        }
        if (speed.y > speedLockY.y)
        {
            speed.y = speedLockY.x;
        }
        else if (speed.y < speedLockY.x)
        {
            speed.y = speedLockY.x;
        }

        transform.position += speed * Time.deltaTime;
    }

    private void FlipEnemy(int f)
    {
        //1为右，-1为左
        if (facing != f)
        {
            facing = f;
            if (facing == 1)
            {
                enemyModel.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                enemyModel.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    private void LockSpeed(int dir, Vector2 spe) //锁定速度，1锁定x，2锁定y
    {
        if (dir == 1)
        {
            speedLockX = spe;
        }
        else
        {
            speedLockY = spe;
        }
    }

    private void FreeSpeed(int dir) //解锁速度，11解锁x， 2解锁y
    {
        if (dir == 1)
        {
            speedLockX = new Vector2(-speedLimit, speedLimit);
        }
        else
        {
            speedLockY = new Vector2(-speedLimit, speedLimit);
        }
    }
}
