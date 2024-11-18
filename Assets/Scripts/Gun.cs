using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : MonoBehaviourPun, IPunObservable
{                   //�߻��غ��, źâ�� ��, ������ ��
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

    public int ammoRemain = 100;//�� ź�� ��
    public int magCapacity = 25;//źâ �ִ�ġ
    public int magAmmo;//���� źâ �� ź ��
    public float timeBetFire = 0.12f;//�߻� �����ð�
    public float reloadTime = 1.8f;//������ �ð�
    public float lastFireTime;//���� �ݹ߽���

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        gunSource = GetComponent<AudioSource>();
        lineRenderer.positionCount = 2;//��� �� ���� �ΰ��� ����
        lineRenderer.enabled = false;
       
    }
    private void OnEnable()
    {
        magAmmo = magCapacity;
        state = State.Ready;
        lastFireTime = 0f;
    }

    public void Fire()//�߻�õ�
    {
        
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();            
        }
    }
    private void Shot()//�ݹ�
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;

        //Ray ray = new Ray(fireTr.position, fireTr.forward);
        if (Physics.Raycast(fireTr.position, fireTr.forward, out hit, fireDistance))
        {
            // �浹�� ��ġ�� ������ ���
            hitPosition = hit.point;
            IDamageable target = hit.collider.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(damage, hitPosition, hit.normal);
            }
        }
        else
        {
            // ����ĳ��Ʈ�� �浹���� ���� ���
            hitPosition = fireTr.position + fireTr.forward * fireDistance;
        }

        magAmmo = Mathf.Clamp(magAmmo, 0, 25);
        magAmmo --;
        StartCoroutine(ShotEffect(hitPosition));
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient);
        //���� �߻� ó���� ȣ��Ʈ�� �븮
        if (magAmmo <= 0)
        {
            magAmmo = 0;
            state = State.Empty;
            Reload();
            return;        
        }
        
    }
    [PunRPC]//ȣ��Ʈ���� ������ �߻�ó���Ѵ�.
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
            // ����ĳ��Ʈ�� �浹���� ���� ���
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
        {//�̹� ������ ���̰ų� ���� ź���� ���ų� źâ�� ź���� �̹� ������ ��� ������ �� �� ����
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
        int ammoToFill = magCapacity - magAmmo;//źâ�� ä�� ź�� ���
        if (ammoRemain < ammoToFill)//źâ�� ä������ ź���� ���� ź�˺��� ���ٸ� ä������ ź�˼��� ���� ź�˼��� ���缭 ����
        {
            ammoToFill = ammoRemain;
        }
        magAmmo += ammoToFill;//źâ�� ä��
        ammoRemain -= ammoToFill;
        state = State.Ready;
    }
   
    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemain += ammo;    
    }
    //�ֱ������� �ڵ� ����Ǵ� ����ȭ �޼���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)//�۽� ������ ������ ȸ�� �̵� ��
        {
            stream.SendNext(ammoRemain);
            //���� ź�� ���� ��Ʈ��ũ�� ���� ������ (�۽�)
            stream.SendNext(magAmmo);
            //źâ ���� ź�� ���� ��Ʈ��ũ�� �۽�
            stream.SendNext(state);
            //���� ���� ���¸� ��Ʈ��ũ�� ������

        }
        else if (stream.IsReading)//�ٸ� ��Ʈ��ũ�������� ���� ������ ���ŵɶ�
        {          
            ammoRemain = (int)stream.ReceiveNext();
            magAmmo = (int)stream.ReceiveNext();
            state = (State)stream.ReceiveNext();
        }
    }
}
