using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbinqAnimation : MonoBehaviour
{
    public float frequence;
    public float magnitude;
    public Vector3 direction;
    Vector3 initialPosition;
    PickUp pickUp;


    private void Start()
    {
        pickUp = GetComponent<PickUp>();
        initialPosition = transform.position;

    }

    private void Update()
    {
        if (pickUp && !pickUp.hasBeenCollected)
        {
            //Sine function for smooth bobbing effect
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequence) * magnitude;
        }
    }
}
