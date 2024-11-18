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
                // Scene에서 GameManager 객체를 찾습니다.
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    // GameManager 객체가 없다면 새로 생성합니다.
                    GameObject singleton = new GameObject("GameManager");
                    instance = singleton.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    public GameObject playerPrefab; // 생성할 플레이어 캐릭터
    private int score = 0;//현재 점수
    public bool isGameOver
    {
        get;  set;        
    }
    void Awake()
    {
       if (instance != null)
            Destroy(gameObject);//자신을 파괴
    }
    private void Start()
    {

        //플레이어 캐릭터의 사망이벤트 발생시 게임 오버
        Vector3 randomSpawnPos = Random.insideUnitSphere * 3f;//플레이어가 생성 할 위치 랜덤지정
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
        if (stream.IsWriting)//로컬의 움직임을 송신
        {
            stream.SendNext(score);
            


        }
        else //다른 네트워크 유저의 움직임을 수신
        { 
            score = (int)stream.ReceiveNext();//네트워크를 통해 score값 받기
            Uimanager.Instance.UpdateScoreText(score);//동기화 해서 받은 점수를 UI로 표시
        

        }
    }
}
