using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform handPosition;
    public float attackRate = 1f;
    float nextAttackTime = 0f;

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0)) // clic izquierdo
            {
                ShootProjectile();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, handPosition.position, handPosition.rotation);
        // Puedes añadir efectos o animación aquí si quieres
    }
}
