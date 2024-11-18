
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startingHealth = 100;
    public float currentHealth { get; protected set; }//상속받은 클래스에서는 public //상속받지않은 클래스에서는 private 작용
    public bool dead { get; protected set; }//사망여부
    public event Action onDeath;//action 반환하지 않는 함수를 대리

    [PunRPC] //host(방장) => 모든 클라이언트 순서 방향으로 사망 상태를 동기화하는 메서드
    public void ApplyUpdatedHealth(float newhealth, bool newDead)
    { 
        currentHealth = newhealth;
        dead = newDead;
    }

    //LivingEntity 클래스는 IDamageable 을 상속하므로 OnDamage 메서드를 반드시 구현해야 한다.
    protected virtual void OnEnable()//virtual 선언시 상속받은 개체에서 override 사용하여 재정의를 가능하게 만든다.
    {       //물려 받을 가상 함수
        dead = false;
        currentHealth = startingHealth;


    }
    //데미지 처리
    [PunRPC]//호스트에서 단독 실행되고 호스트를 통해 다른 클라이언트에서 일괄 실행된다.
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (PhotonNetwork.IsMasterClient)//호스트 방장
        {
            currentHealth -= damage;//데미지 만큼 체력 감소
            photonView.RPC("ApplyUpdatedHealth",RpcTarget.Others,currentHealth,dead);
            //호스트에서 클라이언트로 동기화
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);
        }
        if (currentHealth <= 0 && !dead)
        {
            Die();
        }
        
    }
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)//체력 회복 기능
    {
        if (dead) return;//이미 죽었을 시, 해당 안됨
        if (PhotonNetwork.IsMasterClient)
        {//호스트일때만 체력이추가
            currentHealth += newHealth;
            //서버에서 클라이언트로 동기화
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, currentHealth,dead);

            //다른 클라이언트도 RestoreHealth를 실행하도록
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

