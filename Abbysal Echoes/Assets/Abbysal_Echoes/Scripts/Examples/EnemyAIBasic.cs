using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Configuration")]
    [SerializeField] NavMeshAgent agent; // Componente que permite al objeto tener IA.
    [SerializeField] Transform target; // Transform del objeto a perseguir.
    [SerializeField] LayerMask targetLayer; // Capa de detección del target.
    [SerializeField] LayerMask groundLayer; // Capa de detección del suelo.

    [Header("Patroling Stats")]
    public Vector3 walkPoint; // Dirección a la que se movera la IA si no se detecta al target.
    [SerializeField] float walkPointRange; // Distancia máxima de dirección a generar.
    [SerializeField] bool walkPointSet; // Determina si la IA ha llegado al objetivo

    [Header("Attack Configuration")]
    public float timeBetweenAttacks; // Tiempo de espera entre ataques.
    private bool alredyAttacked; // Determina si ya ha atacado.

    // Variables para ataques a distancia:
    [SerializeField] GameObject projectile; // Referencia de la bala física.
    [SerializeField] Transform shootPoint; // Punto desde donde se genera la bala.
    [SerializeField] float shootSpeedZ; // velocidad frontal de la bala.
    [SerializeField] float shootSpeedY; // Velocidad vertical de la bala (solo si le afecta la gravedad).

    [Header("States & Detection")]
    [SerializeField] float sightRange; // Distancia de detección del target de la IA.
    [SerializeField] float attackRange; // Distancia de ataque.
    [SerializeField] bool targetInSightRange; // Determina si el target esta a distancia de detección.
    [SerializeField] bool targetInAttacktRange; // Determina si el target esta a distancia de ataque.

    [Header("Animations")]
    private Animator animator;
    private bool isDead = false;

    [SerializeField] private float health = 100f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        target = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();


        /* Rigidbody rb = GetComponent<Rigidbody>();
         if (rb != null)
         {
             rb.freezeRotation = true;
             rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
             // Si solo quieres congelar X:
             // rb.constraints = RigidbodyConstraints.FreezeRotationX;
         }*/

        if (animator == null)
        {
            Debug.LogError("No se encontró un Animator en " + gameObject.name + " o sus hijos.");
        }

    }
    private void LateUpdate()
    {
       // transform.rotation = Quaternion.Euler(-89.98f, 0f, 0f);
        /*Vector3 direction = agent.velocity.normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }*/
    }
    private void Update()
    {
        if (isDead) return;

        EnemyStateUpdater();
        targetInSightRange = Physics.CheckSphere(transform.position, sightRange, targetLayer);
        ChaseTarget();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Swimming_Bake_003"))
        {
            Debug.Log("Nadando...");
        }
    }

    void EnemyStateUpdater()
    {


        // Revisar si el target esta en los rangos de detección y/o ataque:

        //targetInAttacktRange = Physics.CheckSphere(transform.position, attackRange, targetLayer);

        // Cambios dinámicos de estado de la IA:
        // Orden de prioridades: ataque > persecución > patrulla.
        if (!targetInSightRange && !targetInAttacktRange)
        {
            Patroling();
        }
        if (targetInSightRange)
        {
            ChaseTarget();
        }
        /* if (targetInSightRange && targetInAttacktRange)
         {
             AttackTarget();
         }*/
    }

    void Patroling()
    {

        //animator.SetBool("IsAttacking", false);
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Swimming_Bake_003"))
        {
            animator.SetTrigger("Swim");
        }

        if (!walkPointSet)
        {
            // Genera un punto de caminado nuevo:
            SearchWalkPoint();
        }
        else
        {
            // Mueve el agente al nuevo punto de caminado:
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1)
        {
            walkPointSet = false;
        }

        animator.Play("Enemy_Swimming_Bake_003");

    }

    void SearchWalkPoint()
    {
        // Generación de nuevo punto de caminado:
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        // Fijación nuevo punto de caminado:
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Comprobación de si el nuevo punto de caminado es válido:
        /*if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }*/

        if (Physics.Raycast(walkPoint + Vector3.up * 5f, Vector3.down, 10f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    void ChaseTarget()
    {
        agent.SetDestination(target.position);
        Debug.Log("Chasing Target pos: " + target.position + " | Agent pos: " + transform.position);
        animator.Play("Enemy_Swimming_Bake_003");
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Swimming_Bake_003"))
        {
            animator.SetTrigger("Swim");
        }

    }

    void AttackTarget()
    {
        // Antes de atacar...:
        agent.SetDestination(transform.position); // Evita que se mueva.
        transform.LookAt(target);

        animator.SetBool("IsAttacking", true);
        animator.Play("Enemy_Attack_Bake");

        if (!alredyAttacked)
        {
            Rigidbody rb = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * shootSpeedZ, ForceMode.Impulse);
            //rb.AddForce(transform.up * shootSpeedY, ForceMode.Impulse); // Solo si le afecta la gravedad.

            // Añade un intervalo entre ataques.
            alredyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alredyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        {
            animator.SetTrigger("TakeHit");
            animator.Play("Enemy_Hit_Bake");

            health -= damage;

            if (health <= 0 && !isDead)
            {
                Die();
            }
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
        animator.Play("Enemy_Death_Bake");
        isDead = true;
        agent.isStopped = true;
        Invoke(nameof(DeactivateEnemy), 3f);
    }
    void DeactivateEnemy()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Die(); // llama a la función que ya tienes
        }
    }

    // Función para que los Gizmos de detección (perseguir/ataque) se dibujen en la escena al seleccionar el objeto.
    private void OnDrawGizmosSelected()
    {
        // Dibuja el rango de ataque:
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Dibuja el rango de persecución:
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}