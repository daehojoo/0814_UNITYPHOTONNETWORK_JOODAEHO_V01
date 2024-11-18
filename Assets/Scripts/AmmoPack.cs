using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class AmmoPack : MonoBehaviourPun,IItem
{
    public int ammo = 30;

    public void Use(GameObject target)
    {
        PlayerShooter shooter = target.GetComponent<PlayerShooter>();
        if (shooter != null && shooter.gun != null)
        {
            //shooter.gun.ammoRemain += ammo;
            shooter.gun.photonView.RPC("AddAmmo", RpcTarget.All, ammo);
        }
        //Destroy(gameObject);
        PhotonNetwork.Destroy(gameObject);//모든 클라이언트에서 삭제
    }
}
