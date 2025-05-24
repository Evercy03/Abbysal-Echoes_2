using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  
    public float speed = 20f;
    public int amount = 25;
    public float lifeTime = 3f;
    public LayerMask enemyLayer;

    void Start()
    {
        Destroy(gameObject, lifeTime); // destruir tras un tiempo

    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(amount);
            }
            Destroy(gameObject); // destruir proyectil tras impactar
        }
    }
}

