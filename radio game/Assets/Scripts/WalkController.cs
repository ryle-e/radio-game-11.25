using UnityEngine;

public class WalkController : MonoBehaviour
{
    [SerializeField] private Transform camTransform;
    [SerializeField] private CharacterController character;
    [SerializeField] private ShakeController camShake;

    [SerializeField] private AudioSource stepSource;
    [SerializeField] private AudioClip stepSound;

    [SerializeField] private float lookSpeed;
    [SerializeField] private float bodyLookSpeed;
    [SerializeField] private float shakeChangeTime = .2f;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float stepInterval;

    [SerializeField] private float lookDistance = 1.5f;
    [SerializeField] private LayerMask lookLayers;

    [SerializeField] private Vector2 yRotLimits;

    private bool isMoving;
    private float smoothMag;
    private float stepProg;

    private LeverController lastLever;

    [HideInInspector] public bool canMove = true;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canMove = true;

        stepProg = stepInterval - Time.deltaTime / 2;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!canMove)
            return;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, lookDistance, lookLayers))
        {
            lastLever = hit.collider.GetComponentInParent<LeverController>();

            lastLever.Highlighted = true;

            if (Input.GetMouseButtonDown(0))
            {
                lastLever.Interact();
            }
        }
        else if (lastLever != null)
        {
            lastLever.Highlighted = false;
        }

            isMoving = false;

        if (Input.mousePositionDelta != Vector3.zero)
        {
            character.transform.Rotate(Vector3.up, bodyLookSpeed * Input.mousePositionDelta.x);
            camTransform.Rotate(Input.mousePositionDelta.y * -lookSpeed, 0, 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            character.Move(character.transform.forward * walkSpeed * Time.deltaTime);
            isMoving = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            character.Move(-character.transform.forward * walkSpeed * Time.deltaTime);
            isMoving = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            character.Move(character.transform.right * walkSpeed * Time.deltaTime);
            isMoving = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            character.Move(-character.transform.right * walkSpeed * Time.deltaTime); 
            isMoving = true;
        }

        if (isMoving)
            camShake.Magnitude = Mathf.SmoothDamp(camShake.Magnitude, 0.2f, ref smoothMag, shakeChangeTime);
        else
            camShake.Magnitude = Mathf.SmoothDamp(camShake.Magnitude, 0, ref smoothMag, shakeChangeTime);

        if (stepProg > stepInterval)
        {
            stepProg -= stepInterval;
            stepSource.PlayOneShot(stepSound);
        }
        else if (isMoving)
        {
            stepProg += Time.deltaTime;
        }
        else
        {
            stepProg = stepInterval - Time.deltaTime/2;
        }
    }
}
