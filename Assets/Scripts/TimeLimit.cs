using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLimit : MonoBehaviour
{
    public Text limitTimeText;
    float time;
    public float limitTime;
    public bool gameStart;
    public bool timeOver;

    // Start is called before the first frame update
    void Start()
    {
        timeOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStart)
        {
            limitTime -= Time.deltaTime;
        }
        limitTimeText.text = string.Format("{0:N1}", limitTime);
        if (limitTime <= 0.0f)
            timeOver = true;
    }

    public void Count_Start()
    {
        gameStart = true;
        GameObject.FindWithTag("GameManager").GetComponent<enemy_Generator>().Invoking();
    }
}
