using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Escenas")]
    public string sceneOnTriggerEnter = "ElenaTest";  // Escena que se carga al tocar el trigger
    public string sceneOnBossDeath;     // Escena que se carga al matar al boss
    public string sceneOnPlayerDeath;
    public PlayerHealth playerHealth;

    [Header("Configuración")]
    public string bossTag = "Boss";     // Tag que debe tener el boss
    public string playerTag = "Player"; // Tag del jugador

    private bool bossDefeated = false;
    [Header("Nombre de la escena del juego")]
    public string gameSceneName = "NombreDeTuPrimeraEscena";

    [Header("Nombre de la escena de créditos (opcional)")]
    public string creditsSceneName = "CreditsScene";

    private GameObject boss;
    private bool bossWasFound = false;


    void Start()
    {
        if (GameState.playerDied)
        {
            Debug.Log("Entramos a la LoseScene por muerte del jugador, no evaluamos más.");
            this.enabled = false;  
            
        }
    }


    public void PlayGame()
    {
        Debug.Log("Cargando juego...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // Esto es para salir del modo juego en el editor
#endif
    }



    void Update()
    {
        if (SceneManager.GetActiveScene().name == sceneOnPlayerDeath) return;

        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;

        // 1. Primero revisa si el jugador murió
        if (playerHealth != null && playerHealth.playerHealth <= 0)
        {
            GameState.playerDied = true;
            Debug.Log("Jugador ha muerto. Cargando escena de derrota.");
            SceneManager.LoadScene(sceneOnPlayerDeath);
            return;

        }

        // 2. Solo entonces revisa si el boss fue derrotado
        if (!bossWasFound)
        {
            boss = GameObject.FindGameObjectWithTag(bossTag);
            if (boss != null)
            {
                bossWasFound = true;
            }
        }

        if (bossWasFound && !bossDefeated && boss == null)
        {
            bossDefeated = true;
            Debug.Log("Boss derrotado, cambiando a escena: " + sceneOnBossDeath);
            SceneManager.LoadScene(sceneOnBossDeath);
        }
    }

    public static class GameState
    {
        public static bool playerDied = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Jugador entró al trigger, cargando escena: " + sceneOnTriggerEnter);
            SceneManager.LoadScene(sceneOnTriggerEnter);
        }
    }
}
