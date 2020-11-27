using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    public void gameClear()
    {
        SceneManager.LoadScene("gameClear");
    }
}
