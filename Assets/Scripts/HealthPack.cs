using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPack : MonoBehaviourPun, IItem
{
    public float health = 50f;
    public void Use(GameObject target)
    {
        LivingEntity living = target.GetComponent<LivingEntity>();
        if (living != null)
        {
            living.RestoreHealth(health);
            //living.photonView.RPC("RestoreHealth", RpcTarget.All, health);
        }
        //Destroy(gameObject);
        PhotonNetwork.Destroy(gameObject);
        
    }
}
