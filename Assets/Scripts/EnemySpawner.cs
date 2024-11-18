using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;



public class EnemySpawner : MonoBehaviourPun,IPunObservable
{
    public Enemy enemyPrefab;
    public Transform[] spawnPoints;

    public float damageMax = 40f;//�ִ���ݷ�
    public float damageMin = 20f;//�ּҰ��ݷ�

    public float healthMax = 200f;
    public float healthMin = 100f;

    public float speedMax = 3f;
    public float speedMin = 2.0f;

    public Color strongEnemyColor = Color.red;
    //���� �� �Ǻλ�

    private List<Enemy> enemies = new List<Enemy>();
    private int wave;
    private int enemyCount = 0;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(enemies.Count);//���� ������ ��Ʈ��ũ�� ������
            stream.SendNext(wave);
        }
        else
        {   //����Ʈ ������Ʈ �б� �κ��� �����
            //���� ������ ��Ʈ��ũ�� ���� �ޱ�
            enemyCount = (int)stream.ReceiveNext();
            wave = (int)stream.ReceiveNext();
        }
    }
    void Awake() //���� �� ����ȭ�Ǿ ������ �鰬�ٰ� �ٽ� ������ȭ �Ǿ� �� Ŭ���̾�Ʈ�� �÷� ���� ������
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor);
    }
    void Update()
    {   //ȣ��Ʈ�� ���� ���� ����
        //�ٸ� Ŭ���̾�Ʈ�� ȣ��Ʈ�� ������ ���� ����ȭ�ؼ� �����ȴ�.
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
        {   //Ŭ���̾�Ʈ�� �� ����Ʈ�� ���� ���� �� �� �����Ƿ�
            //ȣ��Ʈ�� ������ enemyCount�� �̿��� �� �� ǥ��
            Uimanager.Instance.UpdateWaveText(wave, enemyCount);
        }
    }
    void SpawnWave()//���� ���̺꿡 ���缭 �� ����
    {
        wave ++;    //���� ���̺� *1.5�� ���������� �ݿø�
        int spawnCount = Mathf.RoundToInt(wave*1.5f);
        for (int i = 0; i < spawnCount; i++)
        {
            float enemyIntensity = Random.Range(0f, 1f);//���� ����
            CreateEnemy(enemyIntensity);
        }
    }
    void CreateEnemy(float intensity)//���� �����ϰ� ������ ����� �Ҵ�
    {
        //intensity�� ������� ���� �ɷ�ġ�� ����
        float health = Mathf.Lerp(healthMin, healthMax, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        //�� ���������� �� ����



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
