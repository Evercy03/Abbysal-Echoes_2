using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class Boss : MonoBehaviour
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

    private int hitCount = 0;
    public int maxHits = 5;

    [SerializeField] GameObject enemyBody;
    [Header("Animations")]
    private Animator animator;
    private bool isDead = false;

    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        target = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();


        if (animator == null)
        {
            Debug.LogError("No se encontró un Animator en " + gameObject.name + " o sus hijos.");
        }

    }
    private void LateUpdate()
    {
    }
    private void Update()
    {
        if (isDead) return;

        EnemyStateUpdater();
        targetInSightRange = Physics.CheckSphere(transform.position, sightRange, targetLayer);
        ChaseTarget();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Swim"))
        {
            Debug.Log("Nadando...");
        }
    }

    void EnemyStateUpdater()
    {

        if (!targetInSightRange && !targetInAttacktRange)
        {
            Patroling();
        }
        if (targetInSightRange)
        {
            ChaseTarget();
        }

    }

    void Patroling()
    {

        //animator.SetBool("IsAttacking", false);
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Swim"))
        {
            animator.SetTrigger("IsMoving");
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

        animator.Play("Boss_Swim");

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
        Debug.Log(target.position + transform.position);
        animator.Play("Boss_Swim");
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Swim"))
        {
            animator.SetTrigger("IsMoving");
        }

    }

    void AttackTarget()
    {
        // Antes de atacar...:
        agent.SetDestination(transform.position); // Evita que se mueva.
        transform.LookAt(target);

        animator.SetBool("Attack", true);
        animator.Play("Boss_Attack");

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


    /*private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Colisión detectada con:" + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("¡Impacto con proyectil!");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

    }*/

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Colisión detectada con: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Projectile"))
        {
            hitCount++;
            Debug.Log("Impactos recibidos: " + hitCount);

            if (hitCount >= maxHits)
            {
                Debug.Log("¡Boss derrotado!");
                Destroy(gameObject); // Destruye al enemigo
            }

            // Si quieres que el proyectil desaparezca al golpear:
            // Destroy(collision.gameObject);
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



