using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is a class that can be subclassed by any other class to make sprites of the class automatically sort themselves by the y-axis.
/// </summary>

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Sortable : MonoBehaviour
{
	SpriteRenderer sortedSprite;
	public bool sortinActive = true;
	public float minimumDistance = 0.2f;
	int lastSortOrder = 0;


	// Start is called before the first frame update
	protected virtual void Start()
	{
		sortedSprite = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	protected virtual void LateUpdate()
	{
		int newSortOrder = (int)(-transform.position.y / minimumDistance);
		if (lastSortOrder != newSortOrder)
		{
			sortedSprite.sortingOrder = newSortOrder;
		}
	}
}
