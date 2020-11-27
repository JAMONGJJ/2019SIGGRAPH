using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGameisClear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((GameObject.FindWithTag("GameManager").GetComponent<TimeLimit>().timeOver == true &&
            GameObject.FindWithTag("GameManager").GetComponent<enemy_Generator>().EnemyExtinguished == false) ||
            GameObject.FindWithTag("GameManager").GetComponent<Life>().lifeGone == true)
            gameObject.GetComponent<GameOver>().gameOver();
        else if (GameObject.FindWithTag("GameManager").GetComponent<TimeLimit>().timeOver == false &&
            GameObject.FindWithTag("GameManager").GetComponent<Life>().lifeGone == false &&
            GameObject.FindWithTag("GameManager").GetComponent<enemy_Generator>().EnemyExtinguished == true)    
            gameObject.GetComponent<GameClear>().gameClear();
    }
}
