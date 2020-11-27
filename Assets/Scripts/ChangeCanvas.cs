using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCanvas : MonoBehaviour
{
    bool changed;
    public Canvas MainCanvas;
    public GameObject startImage;

    // Start is called before the first frame update
    void Start()
    {
        changed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !changed)
        {
            gameObject.GetComponent<enemy_Generator>().enabled = true;
            gameObject.GetComponent<TimeLimit>().enabled = true;
            gameObject.GetComponent<CheckGameisClear>().enabled = true;
            GameObject.FindWithTag("Loader").GetComponent<OBJLoad>().enabled = true;
            startImage.SetActive(false);
            gameObject.GetComponent<TimeLimit>().Count_Start();
            GameObject.FindWithTag("Loader").GetComponent<OBJLoad>().ReLoad_Texture();
            changed = true;
        }
    }
}
