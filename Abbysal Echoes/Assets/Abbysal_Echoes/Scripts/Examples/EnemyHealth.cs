using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health System Configuration")]
    [SerializeField] int maxHealth;
    [SerializeField] int currentHealth;

    [Header("Feedback Configuration")]
    [SerializeField] Material baseMat;
    [SerializeField] Material damagedMat;
    [SerializeField] GameObject deathEffect;

    // Autorrefernecias privadas
    MeshRenderer enemyRend;

    private void Awake()
    {
        enemyRend = GetComponent<MeshRenderer>();
        baseMat = enemyRend.material;

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            
            deathEffect.SetActive(true);
            deathEffect.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Daño recibido: " + amount + " | Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

        void Die()
    {
        Debug.Log("¡Enemigo eliminado!");
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void ResetDamageMaterial()
    {
        enemyRend.material = baseMat;
    }
}



