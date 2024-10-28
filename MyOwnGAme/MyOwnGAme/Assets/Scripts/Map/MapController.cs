using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapController : MonoBehaviour
{

    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRaiuds;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPosition;


    [Header("Optimization")]
    public List<GameObject> spawnedChunks;

    GameObject latestChunk;
    public float maxOpDist;
    float opDist;
    float optimizerCooldown;
    public float optimizerCooldownDur;

    void Start()
    {
        playerLastPosition = player.transform.position;
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    private void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string dierctionName = GetDirectionName(moveDir);

        CheckAndSpawnChunk(dierctionName);
        
        if (dierctionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }
        else if (dierctionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }
        else if (dierctionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }
        else if (dierctionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }
    }

    void CheckAndSpawnChunk(string direction)
    {
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRaiuds, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(direction).position);
        }
    }

    string GetDirectionName(Vector3 direction) // find solution can be better
    {
        
        direction = direction.normalized;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            //Moving horizontaly more than verticaly
            if (direction.y > 0.5f)
            {
                return direction.x > 0 ? "RightUp" : "LeftUp";
            }
            else if (direction.y < -0.5f)
            {
                return direction.x > 0 ? "RightDown" : "LeftDown";
            }
            else
            {
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            // Moving vertically more than horizontally
            if (direction.x > 0.5f)
            {
                return direction.y > 0 ? "RightUp" : "RightDown";
            }
            else if (direction.x < -0.5f)
            {
                return direction.y > 0 ? "LeftUp" : "LeftDown";
            }
            else
            {
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    private void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }
    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDur;
        }
        else
        {
            return;
        }
        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
