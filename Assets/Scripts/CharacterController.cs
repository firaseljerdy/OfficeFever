using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 150.0f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer;
    private PaperCollector paperCollector;


    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
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

        // Mouse input
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

        // Touch input
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

        if (inputDetected)
        {
            MoveAndRotate(targetPosition);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private void MoveAndRotate(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        RaycastHit hitInfo;
        bool hasClearPath = !Physics.Raycast(transform.position, direction, out hitInfo, Vector3.Distance(transform.position, targetPosition), groundLayer);

        if (hasClearPath)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (direction != Vector3.zero && distance > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            }

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
