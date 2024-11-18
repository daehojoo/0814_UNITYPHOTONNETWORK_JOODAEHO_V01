using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;//����Ƽ �� ���� ���۳�Ʈ
using Photon.Realtime;//���� ���� ���ö��̺귯��
using UnityEngine.UI;

//������ ����(���� ����)�� Mach Making �� ���� ���


public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";//���� ����
    public Text connectionInfoText;//��Ʈ��ũ ���� ǥ��
    public Button joinButton;



    void Start()
    {   //���ӿ� �ʿ��� ����(���� ����) ����
        PhotonNetwork.GameVersion = gameVersion;
        //������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;
        connectionInfoText.text = "������ ������ ���� ��.........";
    }
    //������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "�¶��� : ������ ������ ����Ϸ�...";

    }
    //������ ���� ���� ���н� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "�������� : ������ ������ ������� ����...";

    }
    public void Connect()//�� ���� �õ� joinBtn�� ������ �� ����
    {
        joinButton.interactable = false;//�ߺ� ���ӽõ��� ��������
        if (PhotonNetwork.IsConnected)//������ ������ ���� �� �̶��
        {
            connectionInfoText.text = "�뿡 ����...";
            PhotonNetwork.JoinRandomRoom();//�ƹ��濡�� �����Ѵ�.//�̰��� ���� ��ġ ����ŷ�̶��Ѵ�.
        }
        else
        {
            connectionInfoText.text = "�������� : ������ ������ ������� ����...";
            PhotonNetwork.ConnectUsingSettings();//������ �������� ������ �õ�
        }

    }
    //�� ���� ��� ���� �� ������ ������ ��� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "����� ����, ���ο� �� ����...";
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 });//�� �̸��� �� �ִ� ���� �ο��� ����
        //������ �� ����� Ȯ�� �ϴ� ����� ������ �����Ƿ� ���� �̸���
        //�Է� ���� �ʰ� null�� �Է� �ߴ�.
        //����� ������ ���� ���� ���� ������� �����ϸ�
        //���� ������ Ŭ���̾�Ʈ�� ȣ��Ʈ ��Ȱ�� �ô´�/  ȣ��Ʈ = ������Ŭ���̾�Ʈ = (���尳��)

    }
    //�뿡 ���� �Ϸ�� ��� �ڵ� ����
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "�� ���� ����";
        PhotonNetwork.LoadLevel("Main");//��� �� �����ڰ� MainScene�� �ε� �ϰ� �Ѵ�.
    }
}
