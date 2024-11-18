using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;  //������ �ó׸ӽ� ����ī�޶�� ����ȭ �ϱ� ����

public class CameraSetUp : MonoBehaviourPun//MonoBehaviour ��ɿ��� ���� �並 �߰��� ������Ƽ�̴�.
{       


    void Start()
    {
        //���� �ڽ��� ���� �÷��̾��� ==
        if (photonView.IsMine)//���� ��Ʈ��ũ ���� ���� �䰡 �ڱ��ڽ��� ���̶��
        {
            //���� �ִ� �ó׸ӽ� ����ī�޶� ã��
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
            //����ī�޶��� ��������� �ڽ��� Ʈ���� ������ ����
            followCam.Follow = transform;
            followCam.LookAt = transform;
        }
    }
}
