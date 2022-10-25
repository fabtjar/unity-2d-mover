using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    public float grav = 40;
    public float maxYVel = 30;
    public float jumpSpeed = 15;

    private const float horMinMove = 0.2f;

    private Mover mover;
    private SpriteRenderer sprite;

    private void Awake()
    {
        mover = GetComponent<Mover>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(hor) > horMinMove)
        {
            hor = Mathf.Sign(hor);
            sprite.flipX = hor < horMinMove;
        }
        else
        {
            hor = 0;
        }

        mover.velocity.x = hor * speed;

        if (mover.collisionInfo.bottom || mover.collisionInfo.top)
        {
            mover.velocity.y = 0;
        }

        mover.velocity.y -= grav * Time.deltaTime;
        if (mover.velocity.y < -maxYVel)
        {
            mover.velocity.y = -maxYVel;
        }

        if (Input.GetButton("Jump") && mover.collisionInfo.bottom)
        {
            mover.velocity.y = jumpSpeed;
        }

        if (transform.position.y < -5.5f)
        {
            transform.position = transform.position + Vector3.up * 11f;
        }
    }
}
