using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRootScript : MonoBehaviour
{
    public GameObject treePrefab;
    public float spawnXOffset = 5.0f;
    public float spawnInterval = 5.0f;
    private float lastSpawnTime;


    void Start()
    {
        lastSpawnTime = Time.time;//���� �� �ð� �ʱ�ȭ
       
    }

    
    void Update()
    {
       if(Time.time - lastSpawnTime >= spawnInterval)
        {
            SpawnTree();
            lastSpawnTime = Time.time;
        }
    }

    private void SpawnTree()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnXOffset, transform.position.y, transform.position.z);
        GameObject tree = Instantiate(treePrefab, spawnPosition, Quaternion.identity);
        tree.transform.SetParent(transform);

       
    }
}
