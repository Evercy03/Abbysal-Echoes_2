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

    void Update()
    {
        // Verifica constantemente si el boss ha sido destruido
        if (!bossDefeated && GameObject.FindGameObjectWithTag(bossTag) == null)
        {
            bossDefeated = true;
            Debug.Log("Boss derrotado, cambiando a escena: " + sceneOnBossDeath);
            SceneManager.LoadScene(sceneOnBossDeath);
        }
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
