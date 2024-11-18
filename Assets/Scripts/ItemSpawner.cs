using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;


public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items;//탄약증가 아이템 hp회복 아이템
    public Transform playerTr;
    public float maxDist = 5f;//아이템 생성 최장 거리
    public float timeBetSpawnMax = 7f; //아이템 스폰 시간간격 최대치
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
        //호스트만 아이템 생성
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
        //네트워크에서 모든 클라이언트에 해당 아이템 생성
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
        Vector3 randomPos = Random.insideUnitSphere * distance + center;//insideUnitSphere 반지름이 1인 원 안에서 랜덤한 위치 출력
        //Vector3 randomPoss = Random.insideUnitCircle * distance + center;
        NavMeshHit hit;//navmesh의 결과정보를 저장함
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);
        //maxdistance 반경 안에서 randomPos에 가장 가까운 네비 메시 위의 한점을 찾는다.
        return hit.position;
    }
}
