using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;

    Vector2 knockBackVelocity;
    float knockBackDuration;




    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<PlayerMove>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (knockBackDuration>0)
        {
            transform.position += (Vector3)knockBackVelocity * Time.deltaTime;
            knockBackDuration -= Time.deltaTime;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);

        }

    }
    public void KnockBack(Vector2 velocity, float duration)
    {
        if (knockBackDuration > 0)
            return;
        knockBackVelocity = velocity;
        knockBackDuration = duration;

    }
}
