using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;


public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items;//ź������ ������ hpȸ�� ������
    public Transform playerTr;
    public float maxDist = 5f;//������ ���� ���� �Ÿ�
    public float timeBetSpawnMax = 7f; //������ ���� �ð����� �ִ�ġ
    public float timeBetSpawnMin = 2f;
    private float timeBetSpawn;
    private float lastSpawnTime;

    void Start()
    {
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0;
        //playerTr = GameObject.FindWithTag("Player").transform;
    }
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        //ȣ��Ʈ�� ������ ����
        if (Time.time >= lastSpawnTime + timeBetSpawn && playerTr != null)
        { 
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            Spawn();
        
        }
    }
    void Spawn()
    {
        Vector3 spawnPos = GetRandomPointOnNavMesh(Vector3.zero,maxDist*2);
        spawnPos += Vector3.up * 0.5f;
        GameObject selectedItem = items[Random.Range(0,items.Length)];
        //GameObject item = Instantiate(selectedItem, spawnPos, Quaternion.identity);
        //Destroy(item,5f);
        //��Ʈ��ũ���� ��� Ŭ���̾�Ʈ�� �ش� ������ ����
        GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPos, Quaternion.identity);
        StartCoroutine(DestroyAfter(item,5f));
    }
    IEnumerator DestroyAfter(GameObject target, float delay)
    { 
        yield return new WaitForSeconds(delay);
        if (target != null)
        { 
            PhotonNetwork.Destroy(target);
        }
    }
    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
    {
        Vector3 randomPos = Random.insideUnitSphere * distance + center;//insideUnitSphere �������� 1�� �� �ȿ��� ������ ��ġ ���
        //Vector3 randomPoss = Random.insideUnitCircle * distance + center;
        NavMeshHit hit;//navmesh�� ��������� ������
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);
        //maxdistance �ݰ� �ȿ��� randomPos�� ���� ����� �׺� �޽� ���� ������ ã�´�.
        return hit.position;
    }
}
