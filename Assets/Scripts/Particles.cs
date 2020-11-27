using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    public ParticleSystem particle_WeaponHitUnityChang;
    public ParticleSystem particle_UnityChangHitWeapon;
    
    public void ParticleActive1(Vector3 pos)
    {
        particle_WeaponHitUnityChang.transform.position = pos;
        particle_WeaponHitUnityChang.Play();
    }

    public void ParticleActive2(Vector3 pos)
    {
        particle_UnityChangHitWeapon.transform.position = pos;
        particle_UnityChangHitWeapon.Play();
    }
}
