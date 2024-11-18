using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Scene���� GameManager ��ü�� ã���ϴ�.
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    // GameManager ��ü�� ���ٸ� ���� �����մϴ�.
                    GameObject singleton = new GameObject("GameManager");
                    instance = singleton.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    public GameObject playerPrefab; // ������ �÷��̾� ĳ����
    private int score = 0;//���� ����
    public bool isGameOver
    {
        get;  set;        
    }
    void Awake()
    {
       if (instance != null)
            Destroy(gameObject);//�ڽ��� �ı�
    }
    private void Start()
    {

        //�÷��̾� ĳ������ ����̺�Ʈ �߻��� ���� ����
        Vector3 randomSpawnPos = Random.insideUnitSphere * 3f;//�÷��̾ ���� �� ��ġ ��������
        randomSpawnPos.y += 0.9f;
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
        FindObjectOfType<PlayerHealth>().onDeath += EndGame;



    }
    public void AddScore(int newScore)
    {
        if (!isGameOver)
        {
            score += newScore;
            Uimanager.Instance.UpdateScoreText(score);
        }
    }
    
    public void EndGame()
    { 
        isGameOver = true;
        Uimanager.Instance.SetActiveGameOverUI(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)//������ �������� �۽�
        {
            stream.SendNext(score);
            


        }
        else //�ٸ� ��Ʈ��ũ ������ �������� ����
        { 
            score = (int)stream.ReceiveNext();//��Ʈ��ũ�� ���� score�� �ޱ�
            Uimanager.Instance.UpdateScoreText(score);//����ȭ �ؼ� ���� ������ UI�� ǥ��
        

        }
    }
}
