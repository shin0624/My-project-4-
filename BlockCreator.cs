 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCreator : MonoBehaviour
{
    public GameObject[] blockPrefabs;//����� ������ �迭
    private int block_count = 0; //������ ��� ����
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void createBlock(Vector3 block_position)
    {
        //������ �� ����� Ÿ��(���, ������)�� ���Ѵ�
        int next_block_type = this.block_count % this.blockPrefabs.Length;

        //����� �����ϰ� go�� ����
        GameObject go = GameObject.Instantiate(this.blockPrefabs[next_block_type]) as GameObject;

        go.transform.position = block_position;//����� ��ġ�� �̵�
        this.block_count++;//��� ���� ����
    }
}
