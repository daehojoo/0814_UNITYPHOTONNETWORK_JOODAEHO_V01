
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startingHealth = 100;
    public float currentHealth { get; protected set; }//��ӹ��� Ŭ���������� public //��ӹ������� Ŭ���������� private �ۿ�
    public bool dead { get; protected set; }//�������
    public event Action onDeath;//action ��ȯ���� �ʴ� �Լ��� �븮

    [PunRPC] //host(����) => ��� Ŭ���̾�Ʈ ���� �������� ��� ���¸� ����ȭ�ϴ� �޼���
    public void ApplyUpdatedHealth(float newhealth, bool newDead)
    { 
        currentHealth = newhealth;
        dead = newDead;
    }

    //LivingEntity Ŭ������ IDamageable �� ����ϹǷ� OnDamage �޼��带 �ݵ�� �����ؾ� �Ѵ�.
    protected virtual void OnEnable()//virtual ����� ��ӹ��� ��ü���� override ����Ͽ� �����Ǹ� �����ϰ� �����.
    {       //���� ���� ���� �Լ�
        dead = false;
        currentHealth = startingHealth;


    }
    //������ ó��
    [PunRPC]//ȣ��Ʈ���� �ܵ� ����ǰ� ȣ��Ʈ�� ���� �ٸ� Ŭ���̾�Ʈ���� �ϰ� ����ȴ�.
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (PhotonNetwork.IsMasterClient)//ȣ��Ʈ ����
        {
            currentHealth -= damage;//������ ��ŭ ü�� ����
            photonView.RPC("ApplyUpdatedHealth",RpcTarget.Others,currentHealth,dead);
            //ȣ��Ʈ���� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);
        }
        if (currentHealth <= 0 && !dead)
        {
            Die();
        }
        
    }
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)//ü�� ȸ�� ���
    {
        if (dead) return;//�̹� �׾��� ��, �ش� �ȵ�
        if (PhotonNetwork.IsMasterClient)
        {//ȣ��Ʈ�϶��� ü�����߰�
            currentHealth += newHealth;
            //�������� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, currentHealth,dead);

            //�ٸ� Ŭ���̾�Ʈ�� RestoreHealth�� �����ϵ���
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);
        }
    }

    public virtual void Die()
    {
        if (onDeath != null)
            onDeath();
        dead = true;
    }

}

