using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
    public GameObject[] Lives = new GameObject[3];
    int life_count;
    public bool lifeGone;

    // Start is called before the first frame update
    void Start()
    {
        life_count = 3;
        lifeGone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (life_count <= 0)
            lifeGone = true;
    }

    public void Decrease_Life()
    {
        if(life_count > 0)
        {
            life_count--;
            Destroy(Lives[life_count], 0.5f);
            Resources.UnloadAsset(Lives[life_count]);
        }
    }
}
