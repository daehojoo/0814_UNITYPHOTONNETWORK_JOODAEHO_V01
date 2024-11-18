using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//IK : �޼� �������� ���⿡ ��Ȯ�ϰ� ���� �ǰ�
//�Ѿ� �߻�
public class PlayerShooter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot;//�� ��ġ ������
    public Transform leftHandMound;//���� ���� ������
    public Transform rightHandMound;//���� ������ ������
    private PlayerInPut playerInPut;
    private Animator animator;

    [Header("Ui����")]
    public Text ammoTxt;
    public Text remainAmmoTxt;


    private void OnEnable()
    {//���Ͱ� Ȱ��ȭ �� ��, �ѵ� �Բ� Ȱ��ȭ
        gun.gameObject.SetActive(true);


    }
    void Start()
    {
        
        animator = GetComponent<Animator>();
        playerInPut = GetComponent<PlayerInPut>();
        

    }
    void Update()
    {
        if (!photonView.IsMine) return;

        if (playerInPut.fire)
        {
            gun.Fire();
        }
        else if (playerInPut.reload)
        {
            if (gun.Reload())
            {
                animator.SetTrigger("reloadTrigger");
            }
        }
        UpdateUI();
    }

    void UpdateUI()//ź�� ǥ�� UI ������Ʈ
    {
        if (gun != null && Uimanager.Instance != null)
        {
            //ammoTxt.text = $"{gun.magAmmo}/{gun.ammoRemain}";
            Uimanager.Instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);


        }
    
    }
    private void OnAnimatorIK(int layerIndex)
    {
        gunPivot.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow);//���� ������ gunpivot�� 3D���� ������ �Ȳ�ġ ��ġ�� �̵�
        //IK�� ����Ͽ� �޼��� ��ġ�� ȸ���� ���� ���� �����̿� ����
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);//����ġ
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMound.rotation);
        //IK�� ����Ͽ� �޼��� ��ġ�� ȸ���� ���� ���� �����̿� ����
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMound.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMound.rotation);
    }
    private void OnDisable()
    {
        gun.gameObject.SetActive(false);

    }
}
