using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyMovement : EnemyMovement  
{

    Vector2 chargeDirection;
    // Start is called before the first frame update
    protected new void Start()
    {
        

        chargeDirection = (player.transform.position - transform.position).normalized;
    }

	public override void Move()
	{
        transform.position += (Vector3)chargeDirection * stats.ActualStats.moveSpeed * Time.deltaTime;
	}
}
