using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Debug")]
    public int motionState = 0;
    public bool isGround = false;   //是否在地面
    public Vector2 speedLockX = Vector2.zero;   //
    public Vector2 speedLockY = Vector2.zero;
    public KeyCode respawn = KeyCode.P; //

    [Header("Basic")]
    public Transform playerModel = null;
    public Transform groundCheckPoint = null;
    public Sprite hookSprite = null;
    public Sprite ropeSprite = null;

    [Header("Alter")]
    public float moveSpeed = 3.0f;
    public float firstJumpSpeed = 10.0f;
    public float secondJumpSpeed = 5.0f;
    public float hookMoveSpeed = 20.0f;
    public float gravity = 9.8f;
    public float speedLimit = 20.0f;

    public float groundCheckWidth = 0.8f;
    public float groundCheckHeight = 0.1f;

    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode jump = KeyCode.Space;
    public KeyCode hook = KeyCode.LeftShift;

    private Vector3 speed = Vector3.zero;
    private int jumpCount = 0;
    private bool isHooking = false;
    private int facing = 1;
    private int moveDirection = 0;
    //private int motionState = 0;

    //运动状态
    private const int MOTION_NORMAL = 0;
    private const int MOTION_CATCHWALL = 3;
    private const int MOTION_HOOK = 6;

    private GameObject hookObject = null;

    private void Awake() {
        Camera.main.GetComponent<CameraController>().Init(transform);
        PlayerStickWall psw = GetComponent<PlayerStickWall>();
        if (psw) {
            psw.PlayerStickWallEvent += CatchWall;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        speedLockX = new Vector2(-speedLimit, speedLimit);
        speedLockY = new Vector2(-speedLimit, speedLimit);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (!isHooking) {
            GroundDetect();
            KeyboardDetect();
        }
        else {
            HookBreakDetect();
        }
        */

        GroundDetect(); //地面检测

        HookBreakDetect();

        KeyboardDetect();

        MouseDetect();

        Move();
    }

    private void GroundDetect() {
        if (motionState > MOTION_NORMAL) {
            return;
        }

        Vector2 groundCheckSize = new Vector2(groundCheckWidth, groundCheckHeight);
        Vector3 checkPos = groundCheckPoint.transform.position;
        Collider2D colliderInfo = Physics2D.OverlapBox(checkPos, groundCheckSize, 0, LayerMask.GetMask("Ground"));
        if (colliderInfo != null) {
            if (speed.y < 0) {
                speed.y = 0;
            }
            jumpCount = 2;
            isGround = true;
        }
        else {
            if (speed.y < 0)
            {
                speed.y -= 3.0f * gravity * Time.deltaTime;
            }
            else
            {
                speed.y -= gravity * Time.deltaTime;
            }
            
            if (jumpCount == 2) {
                --jumpCount;
            }

            isGround = false;
        }
    }

    private void KeyboardDetect() {
        moveDirection = 0;
        if (Input.GetKey(moveLeft)) {
            --moveDirection;
            FlipPlayer(-1);
        }
        if (Input.GetKey(moveRight)) {
            ++moveDirection;
            FlipPlayer(1);
        }
        if (Input.GetKeyDown(jump)) {
            Jump();
        }
        if (Input.GetKeyDown(hook)) {
            Hook();
        }

        if (Input.GetKeyDown(respawn)) {
            transform.position = new Vector3(0, 0, 0);
            speed = Vector2.zero;
        }

        Walk(moveDirection);
    }

    private void MouseDetect() {

    }

    private void Walk(int dir) {
        if (motionState > MOTION_NORMAL) {
            return;
        }

        speed.x = dir * moveSpeed;
    }

    private void Jump() {
        if (motionState > MOTION_NORMAL) {
            return;
        }
        
        if (jumpCount == 2) {
            if (speed.y < firstJumpSpeed) {
                speed.y = firstJumpSpeed;
            }
            --jumpCount;
        }
        else if (jumpCount == 1) {
            if (speed.y < secondJumpSpeed) {
                speed.y = secondJumpSpeed;
            }
            --jumpCount;
        }
    }

    private void Move() {

        if (speed.x > speedLockX.y) {
            speed.x = speedLockX.y;
        }
        else if (speed.x < speedLockX.x) {
            speed.x = speedLockX.x;
        }
        if (speed.y > speedLockY.y) {
            speed.y = speedLockY.x;
        }
        else if (speed.y < speedLockY.x) {
            speed.y = speedLockY.x;
        }

        transform.position += speed * Time.deltaTime;
    }

    private void BasicMotion(int dir) {

    }

    private void Hook() {
        if (motionState > MOTION_HOOK) {
            return;
        }

        if (hookObject != null) {
            StopHook();
        }
        else {
            CreateHook();
        }
    }

    private void CreateHook() {

        if (hookObject != null) {
            Destroy(hookObject);
        }
        Rect rec = new Rect(0, 0, 1, 1);    //画布
        Vector2 piv = new Vector2(0.5f, 0.5f);  //锚点
        GameObject obj = new GameObject();
        GameObject a = new GameObject();
        GameObject b = new GameObject();
        ChangeToHook(a, b);
        a.transform.SetParent(obj.transform);
        b.transform.SetParent(obj.transform);
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = (new Vector3(mousePoint.x, mousePoint.y, 0) - transform.position).normalized;
        (obj.AddComponent<HookController>()).Init(transform, a.transform, b.transform, dir);
        hookObject = obj;
    }

    /*
    private void StopHook() {
        isHooking = false;
        if (hookObject != null) {
            Destroy(hookObject);
        }
    }
    */
    
    private void StopHook() {

        motionState = MOTION_NORMAL;
        if (hookObject != null) {
            Destroy(hookObject);
        }
    }

    private void ChangeToHook(GameObject a, GameObject b) {
        SpriteRenderer sr = a.AddComponent<SpriteRenderer>();
        sr.sprite = hookSprite;

        sr = b.AddComponent<SpriteRenderer>();
        sr.sprite = ropeSprite;

        sr.drawMode = SpriteDrawMode.Tiled; //绳子伸长动画
    }

    private void HookBreakDetect() {
        if (motionState != MOTION_HOOK) {
            return;
        }
        Vector2 selfCheckSize = new Vector2(transform.localScale.x, transform.localScale.y);
        Vector3 checkPos = transform.position;
        Collider2D colliderInfo = Physics2D.OverlapBox(checkPos, selfCheckSize, 0, LayerMask.GetMask("Ground"));
        if (colliderInfo != null) {
            StopHook();
        }
    }

    public void HookCatch(Vector3 dir) {
        if (motionState > MOTION_HOOK) {
            return;
        }
        motionState = MOTION_HOOK;
        FreeSpeed(1);
        FreeSpeed(2);
        //isHooking = true;
        speed.x = dir.x * hookMoveSpeed;
        speed.y = dir.y * hookMoveSpeed;
    }

    private void FlipPlayer(int f) {
        if (facing != f) {
            facing = f;
            if (facing == 1) {
                playerModel.GetComponent<SpriteRenderer>().flipX = false;
            }
            else {
                playerModel.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        
    }

    private void CatchWall(int f) {
        if (motionState > MOTION_CATCHWALL) {
            return;
        }

        if (f == 0) {
            motionState = MOTION_NORMAL;
            FreeSpeed(1);
        }
        else if (f == -1) {
            LockSpeed(1, new Vector2(0, speedLockX.y));
        }
        else if (f == 1) {
            LockSpeed(1, new Vector2(speedLockX.x, 0));
        }

        if (facing == f && moveDirection * facing > 0) {
            motionState = MOTION_CATCHWALL;
            LockSpeed(2, Vector2.zero);
        }
        else {
            motionState = MOTION_NORMAL;
            FreeSpeed(2);
        }

    }

    private void LockSpeed(int dir, Vector2 spe) {
        if (dir == 1) {
            speedLockX = spe;
        }
        else {
            speedLockY = spe;
        }
    }

    private void LockSpeedX(int dir) {
        if (dir == -1) {
            speedLockX.x = 0;
        }
        else {
            speedLockX.y = 0;
        }
    }

    private void FreeSpeed(int dir) {
        if (dir == 1) {
            speedLockX = new Vector2(-speedLimit, speedLimit);
        }
        else {
            speedLockY = new Vector2(-speedLimit, speedLimit);
        }
    }

}
