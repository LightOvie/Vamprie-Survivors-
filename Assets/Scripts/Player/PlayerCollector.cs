using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollecctor;
    public float pullSpeed;

    private void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollecctor = GetComponent<CircleCollider2D>();    
    }

    private void Update()
    {
        playerCollecctor.radius = player.Magnet;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ICollectible collectible))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(forceDirection*pullSpeed);

            collectible.Collect();
        }
    }
}
