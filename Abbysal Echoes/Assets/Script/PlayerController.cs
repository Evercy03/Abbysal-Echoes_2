using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 4f;

    private Transform cameraFollowTransform;
    private Rigidbody rb;
    private Vector2 moveInput;
    private float verticalInput;
    private Animator animator;

    public GameObject projectilePrefab;
    public Transform handPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        cameraFollowTransform = Camera.main.transform;
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Movimiento relativo a la cámara
        Vector3 horizontalMove = cameraFollowTransform.forward * moveInput.y + cameraFollowTransform.right * moveInput.x;
        Vector3 verticalMove = Vector3.up * verticalInput;

        Vector3 moveDirection = (horizontalMove + verticalMove).normalized;

        rb.MovePosition(rb.position + moveDirection * playerSpeed * Time.fixedDeltaTime);

        // Rotación horizontal (solo eje Y)
        if (moveDirection != Vector3.zero)
        {
            Vector3 flatDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
            if (flatDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }
        }

        // Animación
        bool isMoving = moveInput.magnitude > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
        animator.SetBool("IsMoving", isMoving);

        // Inclinación hacia adelante cuando está nadando
        if (isMoving)
        {
            Quaternion tilt = Quaternion.Euler(80f, transform.rotation.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, tilt, Time.fixedDeltaTime * 5f);
        }
        else
        {
            Quaternion upright = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, upright, Time.fixedDeltaTime * 5f);
        }
    }

    void ShootProjectile()
    {
        Instantiate(projectilePrefab, handPosition.position, handPosition.rotation);
        // También puedes añadir animaciones o sonidos aquí
    }

   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(3);
            Debug.Log("Habrías cambiado de escena");
        }
    }*/
    #region
    // Movimiento horizontal (WASD o stick izquierdo)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Movimiento vertical (subir y bajar)
    public void OnVertical(InputAction.CallbackContext context)
    {
        verticalInput = context.ReadValue<float>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Aquí va el ataque (ej. lanzar proyectil)
            Debug.Log("¡Ataque realizado!");
            ShootProjectile(); // Llama a tu método de ataque
        }
    }
    #endregion

}