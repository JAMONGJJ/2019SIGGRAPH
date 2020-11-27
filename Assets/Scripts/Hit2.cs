using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hit2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Body1")
        {
            Debug.Log("Body1 Collided");
            GameObject.FindWithTag("GameManager").GetComponent<Particles>().ParticleActive2(gameObject.transform.position);

            GameObject.FindWithTag("GameManager").GetComponent<Life>().Decrease_Life();
            GameObject.FindWithTag("Loader").GetComponent<OBJLoad>().characterHit();
        }
    }
}
