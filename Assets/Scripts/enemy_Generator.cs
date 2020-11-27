using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy_Generator : MonoBehaviour
{
    public GameObject enemyIndicator;
    public GameObject Enemy;
    public Text enemyText;
    int EnemyCount;
    public bool EnemyExtinguished;
    
    // Start is called before the first frame update
    void Start()
    {
        var enemy0 = Instantiate(Enemy, new Vector3(250.0f, 0, 250.0f), Quaternion.identity);
        var indicator0 = Instantiate(enemyIndicator, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        enemy0.name = "kick_0";
        indicator0.transform.parent = enemy0.transform;
        indicator0.transform.position = enemy0.transform.position + new Vector3(0.0f, 400.0f, 0.0f);

        var enemy1 = Instantiate(Enemy, new Vector3(-200.0f, 0, -150.0f), Quaternion.identity);
        var indicator1 = Instantiate(enemyIndicator, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        enemy1.name = "kick_1";
        indicator1.transform.parent = enemy1.transform;
        indicator1.transform.position = enemy1.transform.position + new Vector3(0.0f, 400.0f, 0.0f);

        EnemyCount = 2;
        EnemyExtinguished = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        enemyText.text = string.Format("{0:N0}", EnemyCount);
        if (EnemyCount <= 0)
            EnemyExtinguished = true;
    }

    public void Invoking()
    {
        Invoke("Invoke_Generate", 8.0f);
    }

    void Invoke_Generate()
    {
        Generate_Enemy();
        Invoke("Invoke_Enemy", 8.0f);
    }

    void Generate_Enemy()
    {
        var enemy = Instantiate(Enemy, new Vector3(Random.Range(-500, 500), 0, Random.Range(-500, 500)), Quaternion.identity);
        var indicator = Instantiate(enemyIndicator, new Vector3(0.0f, 0.0f, 0.0f) + new Vector3(0.0f, 400.0f, 0.0f), Quaternion.identity);
        enemy.name = "kick_" + (EnemyCount - 1);
        indicator.transform.parent = enemy.transform;
        indicator.transform.position = enemy.transform.position;
        EnemyCount++;
    }

    public void Decrease_count()
    {
        if(EnemyCount > 0)
            EnemyCount--;
    }
}
