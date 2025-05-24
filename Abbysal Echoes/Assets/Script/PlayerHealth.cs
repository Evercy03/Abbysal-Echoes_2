using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int playerHealth = 100;
    private bool isDead = false;

    void Update()
    {
        // Solo para pruebas: bajar salud con la tecla H
        if (Input.GetKeyDown(KeyCode.H))
        {
            playerHealth -= 10;
        }

        if (playerHealth <= 0)
        {
            FindObjectOfType<SceneChanger>();
        }

        if (!isDead && playerHealth <= 0)
        {
            isDead = true;
            FindObjectOfType<SceneChanger>();
        }
    }
}
