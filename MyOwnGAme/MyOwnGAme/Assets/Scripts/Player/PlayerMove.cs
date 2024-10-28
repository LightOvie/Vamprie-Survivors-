using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove :MonoBehaviour
{
    public const float DEFAULT_MOVESPEED = 5f;
    
   
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMovedVector;

    Rigidbody2D rb;

    PlayerStats player;


    // Start is called before the first frame update
    protected void Start()
    {
      
        player=GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
    }
    private void FixedUpdate()
    {
        Move();
    }
    void InputManager()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x!=0) // last moved x
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f) ;
        }
        if (moveDir.y != 0) // last moved y
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f,lastVerticalVector);
        }

        if (moveDir.x!=0 && moveDir.y!=0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }

       
      
    }

    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rb.velocity = DEFAULT_MOVESPEED * player.Stats.moveSpeed * moveDir;
    
    }
}
