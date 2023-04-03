using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("The movement speed of the character")]
    [SerializeField] private float movementSpeed = 5.0f;

    [Header("The rotation speed of the character")]
    [SerializeField] private float rotationSpeed = 150.0f;

    [Header("The main camera used for raycasting")]
    [SerializeField] private Camera mainCamera;

    [Header("The layer mask used for ground detection")]
    [SerializeField] private LayerMask groundLayer;

    // The PaperCollector component used for checking if the character is currently collecting paper
    private PaperCollector paperCollector;

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        // Get references to the required components
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        paperCollector = GetComponent<PaperCollector>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        Vector3 targetPosition = Vector3.zero;
        bool inputDetected = false;

        // Check for mouse input if the character is not collecting paper
        if (Input.GetMouseButton(0) && !paperCollector.IsCollecting)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, groundLayer))
            {
                targetPosition = hit.point;
                inputDetected = true;
            }
        }

        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, groundLayer))
                {
                    targetPosition = hit.point;
                    inputDetected = true;
                }
            }
        }

        // Move and rotate the character towards the target position if input was detected
        if (inputDetected)
        {
            MoveAndRotate(targetPosition);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    // Move and rotate the character towards the target position
    private void MoveAndRotate(Vector3 targetPosition)
    {
        // Calculate the direction towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        // Check if there is a clear path towards the target position
        RaycastHit hitInfo;
        bool hasClearPath = !Physics.Raycast(transform.position, direction, out hitInfo, Vector3.Distance(transform.position, targetPosition), groundLayer);

        // Move and rotate the character if there is a clear path towards the target position
        if (hasClearPath)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (direction != Vector3.zero && distance > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            }

            // Move the character towards the target position
            if (distance > 0.1f)
            {
                float step = movementSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
                animator.SetFloat("Speed", 1);
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }
        }
    }
}
