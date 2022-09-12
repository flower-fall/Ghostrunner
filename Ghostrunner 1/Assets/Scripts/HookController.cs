using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HookController : MonoBehaviour
{
    private float moveSpeed = 20.0f;
    private float existTime = 5.0f;
    private float maxLength = 10.0f;
    private Vector2 size = Vector2.zero;

    private Transform player = null;
    private Transform hook = null;
    private Transform rope = null;
    private SpriteRenderer ropeSprite = null; 

    private Vector3 dir = Vector3.zero;
    private bool isWorking = false;
    private bool isCatch = false;
    Coroutine routine = null;

    // Start is called before the first frame update
    void Start()
    {
        size = new Vector2(0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWorking) {
            return;
        }
        Move();
    }

    private void Move() {
        hook.position += dir * moveSpeed * Time.deltaTime;
        rope.position = (player.position + hook.position) / 2;
        ropeSprite.size = new Vector2((player.position - hook.position).magnitude, 1.0f);
        if (ropeSprite.size.x > maxLength) {
            Destroy(gameObject);
        }
        RotationModify();
        HookCatchDetect();
    }

    private void RotationModify() {
        Vector3 dir = (hook.position - player.position).normalized;
        float ang = 0;
        if (dir.x > -1e-3 && dir.x < 1e-3) {
            ang = 90.0f;
        }
        else {
            ang = 180.0f * Mathf.Atan(dir.y / dir.x) / Mathf.PI;
        }
        rope.rotation = Quaternion.Euler(0, 0, ang);
    }

    public void Init(Transform p, Transform a, Transform b, Vector3 c) {
        transform.position = p.position;
        player = p;
        hook = a;
        rope = b;
        dir = c;
        ropeSprite = rope.GetComponent<SpriteRenderer>();
        isWorking = true;
        routine = StartCoroutine(TimeOut());
    }

    IEnumerator TimeOut() {
        float t = 0;
        while (true) {
            t += Time.deltaTime;
            if (t >= existTime) {
                Destroy(gameObject);
            }
            yield return 0;
        }
    }

    private void HookCatchDetect() {
        Vector3 checkPos = hook.position;
        Collider2D colliderInfo = null;
        if (!isCatch) {
            colliderInfo = Physics2D.OverlapBox(checkPos, size, 0, LayerMask.GetMask("Ground"));
            if (colliderInfo != null) {
                moveSpeed = 0;
                Vector3 dir = (hook.position - player.position).normalized;
                player.GetComponent<PlayerController>().HookCatch(dir);
                StopCoroutine(routine);
                isCatch = true;
                StartCoroutine(TimeOut());
            }
        }
        else {
            colliderInfo = Physics2D.OverlapBox(checkPos, size, 0, LayerMask.GetMask("Player"));
            if (colliderInfo != null) {
                Destroy(gameObject);
            }
        }
    }

}
