using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;



public class EnemySpawner : MonoBehaviourPun,IPunObservable
{
    public Enemy enemyPrefab;
    public Transform[] spawnPoints;

    public float damageMax = 40f;//최대공격력
    public float damageMin = 20f;//최소공격력

    public float healthMax = 200f;
    public float healthMin = 100f;

    public float speedMax = 3f;
    public float speedMin = 2.0f;

    public Color strongEnemyColor = Color.red;
    //강한 적 피부색

    private List<Enemy> enemies = new List<Enemy>();
    private int wave;
    private int enemyCount = 0;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(enemies.Count);//남은 적수를 네트워크로 보내기
            stream.SendNext(wave);
        }
        else
        {   //리모트 오브젝트 읽기 부분이 실행됨
            //남은 적수를 네트워크를 통해 받기
            enemyCount = (int)stream.ReceiveNext();
            wave = (int)stream.ReceiveNext();
        }
    }
    void Awake() //좀비 색 직렬화되어서 서버에 들갔다가 다시 역직렬화 되어 각 클라이언트에 컬러 값이 보내짐
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor);
    }
    void Update()
    {   //호스트만 적을 직접 생성
        //다른 클라이언트는 호스트가 생성한 적을 동기화해서 반응된다.
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                return;

            if (enemies.Count <= 0)
            {
                SpawnWave();


            }
            
        }
        UpdateUI();
    }
    void UpdateUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Uimanager.Instance.UpdateWaveText(wave, enemies.Count);

        }
        else
        {   //클라이언트는 적 리스트를 직접 갱신 할 수 없으므로
            //호스트가 보내준 enemyCount를 이용해 적 수 표시
            Uimanager.Instance.UpdateWaveText(wave, enemyCount);
        }
    }
    void SpawnWave()//현재 웨이브에 맞춰서 적 생성
    {
        wave ++;    //현재 웨이브 *1.5를 정수값으로 반올림
        int spawnCount = Mathf.RoundToInt(wave*1.5f);
        for (int i = 0; i < spawnCount; i++)
        {
            float enemyIntensity = Random.Range(0f, 1f);//적의 강도
            CreateEnemy(enemyIntensity);
        }
    }
    void CreateEnemy(float intensity)//적을 생성하고 추적할 대상을 할당
    {
        //intensity를 기반으로 적의 능력치가 결정
        float health = Mathf.Lerp(healthMin, healthMax, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        //적 프리팹으로 적 생성



        // Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name, spawnPoint.position, spawnPoint.rotation);
        Enemy enemy= createdEnemy.GetComponent<Enemy>();
        enemy.photonView.RPC("Setup", RpcTarget.All, health, damage, speed,skinColor);
        //enemy.Setup(health, damage, speed, skinColor);
        enemies.Add(enemy);

        enemy.onDeath += () => enemies.Remove(enemy);
        enemy.onDeath += () => StartCoroutine(DestroyAfter(enemy.gameObject, 10f));
    
        enemy.onDeath += () => GameManager.Instance.AddScore(100);
    }
    IEnumerator DestroyAfter(GameObject target, float delay)
    { 
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }
}
