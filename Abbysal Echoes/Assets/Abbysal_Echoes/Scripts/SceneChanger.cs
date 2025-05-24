using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Escenas")]
    public string sceneOnTriggerEnter = "ElenaTest";  // Escena que se carga al tocar el trigger
    public string sceneOnBossDeath;     // Escena que se carga al matar al boss
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

    public void LoadCredits()
    {
        Debug.Log("Cargando créditos...");
        SceneManager.LoadScene(creditsSceneName);
    }

    void Update()
    {
        // Verifica constantemente si el boss ha sido destruido
        if (SceneManager.GetActiveScene().name == "MainMenuScene")
            return;

        // Buscar al boss solo una vez
        if (!bossWasFound)
        {
            boss = GameObject.FindGameObjectWithTag(bossTag);
            if (boss != null)
            {
                bossWasFound = true;
            }
        }

        // Si ya fue encontrado y ahora está destruido
        if (bossWasFound && !bossDefeated && boss == null)
        {
            bossDefeated = true;
            Debug.Log("Boss derrotado, cambiando a escena: " + sceneOnBossDeath);
            SceneManager.LoadScene(sceneOnBossDeath);
        }

        // Comprobar muerte del jugador
        if (playerHealth != null && playerHealth.playerHealth <= 0)
        {
            SceneManager.LoadScene("LoseScene");
        }
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
