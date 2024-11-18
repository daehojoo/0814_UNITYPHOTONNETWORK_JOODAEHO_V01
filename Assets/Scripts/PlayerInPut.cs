using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//�Է°� �����Ӱ� �и��ؼ� ��ũ��Ʈ�� �����.
//�Է°� ���͸� ������
public class PlayerInPut : MonoBehaviourPun
{
    public string moveAxisName = "Vertical";
    public string rotateAxisName = "Horizontal";
    public string fireButton = "Fire1";
    public string reloadButton = "Reload";
    //Ű���� ������Ƽ �����
    public float move {  get; private set; }
    public float rotate { get; private set; }
    public bool fire { get; private set; }
    public bool reload { get; private set; }


    void Start()
    {
        




    }

    void Update()
    {
        if (!photonView.IsMine) return;//����䰡 �����÷��̾ �ƴϸ� �Է��� ���� �ʰ� ����
        
        if ( GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            move = 0f;
            rotate = 0f;
            fire = false;
            reload = false;
            return;
        }
        move = Input.GetAxis(moveAxisName);
        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButton);
        reload = Input.GetButtonDown(reloadButton);
        
    }
}
