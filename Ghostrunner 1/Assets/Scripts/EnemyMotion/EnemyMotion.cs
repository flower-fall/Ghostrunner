using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMotion : MonoBehaviour
{
    [Header("Debug")]
    public int motionState = 0; //当前运动状态：站立、走路、射击

    [Header("Basic")]
    public Transform ememyMode = null;
    public Sprite bullets = null; //子弹
    public Vector2 speedLockX = Vector2.zero;   //x轴速度上限
    public Vector2 speedLockY = Vector2.zero;   //y轴速度上限

    [Header("Alter")]
    public float moveSpeed = 0.3f;
    public float gravity = 9.5f;
    public float speedLimit = 20.0f;

    public float groundCheckWidth = 0.8f;
    public float groundCheckHeight = 0.1f;

    private Vector3 speed = Vector3.zero;   //三个方向的速度
    private int facing = 1; //朝向
    private int moveDirection = 0;  //移动方向,-1左1右

    //运动状态
    private const int MOTION_STAND = 0; //站立
    private const int MOTION_WALK = 1;  //走路
    private const int MOTION_SHOOT = 2; //射击

    //子弹对象
    private GameObject bullteObject = null;

    private void Awake()
    {
        Camera.main.GetComponent<CameraController>().Init(transform);
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
        Walk(1);
        Move();
    }

    private void CatchWall(int facing)
    {
        
    }

    private void Walk(int dir)
    {
        if (motionState > MOTION_WALK)
        {
            return;
        }

        speed.x = dir * moveSpeed;
    }

    private void Move()
    {

        if (speed.x > speedLockX.y)
        {
            speed.x = speedLockX.y;
        }
        else if (speed.x < speedLockX.x)
        {
            speed.x = speedLockX.x;
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
}
