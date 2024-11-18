using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Uimanager : MonoBehaviour
{
    private static Uimanager instance;
    public static Uimanager Instance
    {
        get
        {
            if (instance == null)
            {
                // Scene���� GameManager ��ü�� ã���ϴ�.
                instance = FindObjectOfType<Uimanager>();
                if (instance == null)
                {
                    // GameManager ��ü�� ���ٸ� ���� �����մϴ�.
                    GameObject singleton = new GameObject("Uimanager");
                    instance = singleton.AddComponent<Uimanager>();
                }
            }
            return instance;
        }
    }
    public Text ammoTxt;
    public Text scoreTxt;
    public Text waveTxt;
    public GameObject gameoverUi;


    void Start()
    {




    }
    public void UpdateAmmoText(int magAmmo,int remainAmmo)
    {
        ammoTxt.text = $"{magAmmo}/{remainAmmo}";



    }
    public void UpdateScoreText(int newScore)
    {
        scoreTxt.text = $"Score : {newScore}";



    }
    public void UpdateWaveText(int newWave,int count)
    {
        waveTxt.text = $"Wave : {newWave}\nEnemyLeft : {count}";



    }
    public void SetActiveGameOverUI(bool active)
    { 
        gameoverUi.SetActive(active);
    
    
    }
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);//�ڱ��ڽ� �� �����


    }
}
