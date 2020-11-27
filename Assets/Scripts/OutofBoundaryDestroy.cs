using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutofBoundaryDestroy : MonoBehaviour
{
    public float boundary;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.x > boundary || gameObject.transform.position.x < -boundary
            || gameObject.transform.position.z > boundary || gameObject.transform.position.z < -boundary)
        {
            Destroy(gameObject, 0.0f);
            Resources.UnloadAsset(gameObject);
            GameObject.FindWithTag("GameManager").GetComponent<enemy_Generator>().Decrease_count();
        }
    }
}
