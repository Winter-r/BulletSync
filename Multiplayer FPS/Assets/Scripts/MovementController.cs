using UnityEngine;

public class MovementController : MonoBehaviour
{
	public bool canMove { get; private set; } = true;
	public bool isSprinting => canSprint && Input.GetKey(sprintKey);
	private bool shouldJump => controller.isGrounded && Input.GetKeyDown(jumpKey);

	[Header("Functional Options")]
	[SerializeField] private bool canSprint = true;
	[SerializeField] private bool canJump = true;

	[Header("Controls")]
	[SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;

	[Header("Movement Parameters")]
	[SerializeField] private float walkSpeed = 3.0f;
	[SerializeField] private float sprintSpeed = 6.0f;

	[Header("Look Parameters")]
	[SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
	[SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
	[SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
	[SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

	[Header("Jumping Parameters")]
	[SerializeField] private float jumpForce = 8.0f;
	[SerializeField] private float gravity = 30.0f;

	private Camera playerCamera;
	private CharacterController controller;

	private Vector3 moveDirection;
	private Vector2 currentInput;

	private float rotationX = 0;

	private void Awake()
	{
		playerCamera = GetComponentInChildren<Camera>();
		controller = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		if (canMove)
		{
			HandleMovementInput();
			HandleMouseLook();
			
			if (canJump)
			{
				HandleJump();
			}

			ApplyFinalMovements();
		}
	}

	private void HandleMovementInput()
	{
		currentInput = new Vector2((isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"),
								   (isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

		float moveDirectionY = moveDirection.y;
		moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x)
						+ (transform.TransformDirection(Vector3.right) * currentInput.y);
		moveDirection.y = moveDirectionY;
	}

	private void HandleMouseLook()
	{
		rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
		rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
		playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
		transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
	}
	
	private void HandleJump()
	{
		if (shouldJump)
		{
			moveDirection.y = jumpForce;
		}
	}

	private void ApplyFinalMovements()
	{
		if (!controller.isGrounded)
			moveDirection.y -= gravity * Time.deltaTime;

		controller.Move(moveDirection * Time.deltaTime);
	}
}