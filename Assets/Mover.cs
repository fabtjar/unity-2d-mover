using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public LayerMask hitMask;
    public Vector2 velocity;
    public float checkDist = 0.5f;
    public CollisionInfo collisionInfo;

    private const float skinWidth = 0.015f;
    private const float minMove = 0.001f;

    private BoxCollider2D box;
    private RayOrigins rayOrigins;
    private Vector2Int rayCount;
    private Vector2 raySpacing;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    private void UpdateRayOrigins()
    {
        Bounds bounds = box.bounds;
        bounds.Expand(skinWidth * -2);

        rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        rayOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        rayOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = box.bounds;
        bounds.Expand(skinWidth * -2);

        rayCount.x = Mathf.CeilToInt(box.size.x / checkDist);
        rayCount.y = Mathf.CeilToInt(box.size.y / checkDist);

        raySpacing.x = bounds.size.x / rayCount.x;
        raySpacing.y = bounds.size.y / rayCount.y;

    }

    private void FixedUpdate()
    {
        UpdateRayOrigins();
        collisionInfo.Reset();

        Vector3 move = velocity * Time.fixedDeltaTime;
        CollideX(ref move);
        CollideY(ref move);
        transform.position += move;
    }

    private void CollideX(ref Vector3 move)
    {
        if (Mathf.Abs(move.x) < minMove)
        {
            move.x = 0;
            return;
        }

        Vector2 start = move.x < 0 ? rayOrigins.bottomLeft : rayOrigins.bottomRight;

        float rayLength = Mathf.Abs(move.x) + skinWidth;
        float dir = Mathf.Sign(move.x);

        for (int i = 0; i <= rayCount.x; i += 1)
        {
            Vector2 pos = start + i * raySpacing.y * Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.right * dir, rayLength, hitMask);
            Debug.DrawRay(pos, Vector2.right * dir, hit ? Color.red : Color.white);
            if (hit)
            {
                rayLength = hit.distance;
                collisionInfo.left = dir < 0;
                collisionInfo.right = dir > 0;
            }
        }

        move.x = (rayLength - skinWidth) * dir;
    }

    private void CollideY(ref Vector3 move)
    {
        if (Mathf.Abs(move.y) < minMove)
        {
            move.y = 0;
            return;
        }

        Vector2 start = move.y < 0 ? rayOrigins.bottomLeft : rayOrigins.topLeft;

        // X would have changed position in other check.
        start.x += move.x;

        float rayLength = Mathf.Abs(move.y) + skinWidth;
        float dir = Mathf.Sign(move.y);

        for (int i = 0; i <= rayCount.y; i += 1)
        {
            Vector2 pos = start + i * raySpacing.x * Vector2.right;
            RaycastHit2D hit = Physics2D.Raycast(pos,Vector2.up * dir, rayLength, hitMask);
            Debug.DrawRay(pos, Vector2.up * dir, hit ? Color.red : Color.white);
            if (hit.collider != null)
            {
                rayLength = hit.distance;
                collisionInfo.bottom = dir < 0;
                collisionInfo.top = dir > 0;
            }
        }

        move.y = (rayLength - skinWidth) * dir;
    }

    private struct RayOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool top, bottom, left, right;

        public void Reset()
        {
            top = bottom = left = right = false;
        }
    }
}
