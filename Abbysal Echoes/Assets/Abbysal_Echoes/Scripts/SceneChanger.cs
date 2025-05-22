using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private bool gameEnded = false;

    void Update()
    {
        if (!gameEnded)
        {
            CheckBossDeath();
        }
    }

    // Este método debe ser llamado desde el script que controle la muerte del jugador
    public void OnPlayerDeath()
    {
        if (!gameEnded)
        {
            gameEnded = true;
            LoadScene("You Loose");
        }
    }

    void CheckBossDeath()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        if (boss == null)
        {
            gameEnded = true;
            LoadScene("You Win");
        }
    }

    void LoadScene(string sceneName)
    {
        Debug.Log("Cambiando a escena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
