using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit1 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            Debug.Log("Body2 Collided");
            Destroy(gameObject);
            Resources.UnloadAsset(gameObject);
            GameObject.FindWithTag("GameManager").GetComponent<Particles>().ParticleActive1(gameObject.transform.position);
            GameObject.FindWithTag("GameManager").GetComponent<enemy_Generator>().Decrease_count();
        }
    }
}
