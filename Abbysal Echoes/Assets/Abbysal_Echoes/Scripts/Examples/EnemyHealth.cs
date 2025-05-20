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
            currentHealth = 0;
            deathEffect.SetActive(true);
            deathEffect.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        enemyRend.material = damagedMat;
        Invoke(nameof(ResetDamageMeterial), 0.2f);
    }

    private void ResetDamageMeterial()
    {
        enemyRend.material = baseMat;
    }
}