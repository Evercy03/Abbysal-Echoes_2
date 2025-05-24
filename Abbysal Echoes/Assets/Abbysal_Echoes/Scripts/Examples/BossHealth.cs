using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health System Configuratio")]
    [SerializeField] int maxHealth;
    [SerializeField] int currentHealth;

    [Header("Feedback Configuration")]
    [SerializeField] Material baseMat;
    [SerializeField] Material damagedMat;
    [SerializeField] GameObject deathEffect; //Aquí está por ver el hecho de que el boss tenga un efecto de muerte. 

    MeshRenderer bossRend;

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

    public void TakeDamage (int damage)
    {
        currentHealth += damage;
        bossRend.material = damagedMat;
        Invoke(nameof(ResetDamageMaterial), 0.2f);
    }

    private void ResetDamageMaterial ()
    {
        bossRend.material = baseMat;
    }

}
