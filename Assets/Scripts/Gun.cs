using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : MonoBehaviourPun, IPunObservable
{                   //발사준비됨, 탄창이 빔, 재장전 중
    public enum State { Ready, Empty, Reloading }
    public State state  { get; private set; }
    
    public Transform fireTr;
    public ParticleSystem muzzleFlash;
    public ParticleSystem shellEjectEffect;
    private LineRenderer lineRenderer;
    public AudioSource gunSource;
    public AudioClip gunClip;
    public AudioClip reloadClip;
    public Animator animator;
    public float damage = 25f;
    private float fireDistance = 50f;

    public int ammoRemain = 100;//총 탄알 수
    public int magCapacity = 25;//탄창 최대치
    public int magAmmo;//현재 탄창 내 탄 수
    public float timeBetFire = 0.12f;//발사 지연시간
    public float reloadTime = 1.8f;//재장전 시간
    public float lastFireTime;//이전 격발시점

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        gunSource = GetComponent<AudioSource>();
        lineRenderer.positionCount = 2;//사용 할 점을 두개로 변경
        lineRenderer.enabled = false;
       
    }
    private void OnEnable()
    {
        magAmmo = magCapacity;
        state = State.Ready;
        lastFireTime = 0f;
    }

    public void Fire()//발사시도
    {
        
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();            
        }
    }
    private void Shot()//격발
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;

        //Ray ray = new Ray(fireTr.position, fireTr.forward);
        if (Physics.Raycast(fireTr.position, fireTr.forward, out hit, fireDistance))
        {
            // 충돌된 위치와 정보를 출력
            hitPosition = hit.point;
            IDamageable target = hit.collider.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(damage, hitPosition, hit.normal);
            }
        }
        else
        {
            // 레이캐스트가 충돌하지 않은 경우
            hitPosition = fireTr.position + fireTr.forward * fireDistance;
        }

        magAmmo = Mathf.Clamp(magAmmo, 0, 25);
        magAmmo --;
        StartCoroutine(ShotEffect(hitPosition));
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient);
        //실제 발사 처리는 호스트에 대리
        if (magAmmo <= 0)
        {
            magAmmo = 0;
            state = State.Empty;
            Reload();
            return;        
        }
        
    }
    [PunRPC]//호스트에서 실제로 발사처리한다.
    private void ShotProcessOnServer()
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;
        if (Physics.Raycast(fireTr.position, fireTr.forward, out hit, fireDistance))
        {
            hitPosition = hit.point;
            IDamageable target = hit.collider.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(damage, hitPosition, hit.normal);
            }
        }
        else
        {
            // 레이캐스트가 충돌하지 않은 경우
            hitPosition = fireTr.position + fireTr.forward * fireDistance;
        }
        photonView.RPC("ShotEffectProcessOnClient", RpcTarget.All,hitPosition);
    }
    [PunRPC]
    void ShotEffectProcessOnClient(Vector3 hitPos)
    {
        StartCoroutine(ShotEffect(hitPos));
    }
    IEnumerator ShotEffect(Vector3 hitPosition)
    {
        
        muzzleFlash.Play();
        shellEjectEffect.Play();
        gunSource.PlayOneShot(gunClip, 1.0f);
        lineRenderer.SetPosition(0, fireTr.position);
        lineRenderer.SetPosition(1, hitPosition);
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.3f);
        lineRenderer.enabled = false;
    }
    public bool Reload()
    {
        if (state == State.Reloading || magAmmo >= magCapacity || magAmmo >= magCapacity)
        {//이미 재장전 중이거나 남은 탄알이 없거나 탄창에 탄알이 이미 가득한 경우 재장전 할 수 없다
            return false;
        }
        StartCoroutine(ReloadRoutine());
        return true;
    }
    IEnumerator ReloadRoutine()
    {
        state = State.Reloading;
        gunSource.PlayOneShot(reloadClip, 1.0f);
        animator.SetTrigger("reloadTrigger");
        yield return new WaitForSeconds(reloadTime);
        int ammoToFill = magCapacity - magAmmo;//탄창에 채울 탄을 계산
        if (ammoRemain < ammoToFill)//탄창에 채워야할 탄알이 남은 탄알보다 많다면 채워야할 탄알수를 남은 탄알수에 맞춰서 줄임
        {
            ammoToFill = ammoRemain;
        }
        magAmmo += ammoToFill;//탄창을 채움
        ammoRemain -= ammoToFill;
        state = State.Ready;
    }
   
    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemain += ammo;    
    }
    //주기적으로 자동 실행되는 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)//송신 로컬의 움직임 회전 이동 등
        {
            stream.SendNext(ammoRemain);
            //남은 탄알 수를 네트워크로 통해 보내고 (송신)
            stream.SendNext(magAmmo);
            //탄창 내의 탄알 수를 네트워크로 송신
            stream.SendNext(state);
            //현재 총의 상태를 네트워크로 보내기

        }
        else if (stream.IsReading)//다른 네트워크유저에게 총의 정보가 수신될때
        {          
            ammoRemain = (int)stream.ReceiveNext();
            magAmmo = (int)stream.ReceiveNext();
            state = (State)stream.ReceiveNext();
        }
    }
}
