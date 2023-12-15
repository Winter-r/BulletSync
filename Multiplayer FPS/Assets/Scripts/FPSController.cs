using System;
using System.Collections;
using UnityEngine;

public class FPSController : MonoBehaviour
{
	#region Variables

	public bool CanMove { get; private set; } = true;
	private bool IsSprinting => canSprint && !IsSliding && Input.GetKey(sprintKey);
	private bool ShouldJump => controller.isGrounded && !IsSliding && Input.GetKeyDown(jumpKey);
	private bool ShouldCrouch => !duringCrouchAnimation && controller.isGrounded && Input.GetKeyDown(crouchKey);

	[Header("Functional Options")]
	[SerializeField] private bool canSprint = true;
	[SerializeField] private bool canJump = true;
	[SerializeField] private bool canCrouch = true;
	[SerializeField] private bool canHeadbob = true;
	[SerializeField] private bool willSlideOnSlopes = true;
	[SerializeField] private bool canZoom = true;
	[SerializeField] private bool canInteract = true;
	[SerializeField] private bool useFootsteps = true;
	[SerializeField] private bool useStamina = true;

	[Header("Controls")]
	[SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;
	[SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
	[SerializeField] private KeyCode interactKey = KeyCode.E;
	[SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;

	[Header("Movement Parameters")]
	[SerializeField] private float walkSpeed = 5.0f;
	[SerializeField] private float sprintSpeed = 8.0f;
	[SerializeField] private float crouchSpeed = 2.5f;
	[SerializeField] private float slideSpeed = 8f;

	[Header("Look Parameters")]
	[SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
	[SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
	[SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
	[SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

	[Header("Health Parameters")]
	[SerializeField] private float maxHealth = 100;
	[SerializeField] private float timeBeforeHealthRegen = 3;
	[SerializeField] private float healthIncrement = 1;
	[SerializeField] private float healthTimeIncrement = 0.1f;
	private float currentHealth;
	private Coroutine regeneratingHealth;
	public static Action<float> OnTakeDamage;
	public static Action<float> OnDamage;
	public static Action<float> OnHeal;

	[Header("Stamina Paramaters")]
	[SerializeField] private float maxStamina = 50;
	[SerializeField] private float staminaUseMultiplier = 5;
	[SerializeField] private float timeBeforeStaminaRegen = 5;
	[SerializeField] private float staminaIncrement = 2;
	[SerializeField] private float staminaTimeIncrement = 0.1f;
	private float currentStamina;
	private Coroutine regeneratingStamina;
	public static Action<float> OnStaminaChange;

	[Header("Jumping Parameters")]
	[SerializeField] private float jumpForce = 8.0f;
	[SerializeField] private float gravity = 30.0f;

	[Header("Crouch Parameters")]
	[SerializeField] private float crouchHeight = 0.5f;
	[SerializeField] private float standingHeight = 2f;
	[SerializeField] private float timeToCrouch = 0.25f;
	[SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);
	[SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
	private bool isCrouching;
	private bool duringCrouchAnimation;

	[Header("Headbob Parameters")]
	[SerializeField] private float walkBobSpeed = 14f;
	[SerializeField] private float walkBobAmount = 0.05f;
	[SerializeField] private float sprintBobSpeed = 18;
	[SerializeField] private float sprintBobAmount = 0.1f;
	[SerializeField] private float crouchBobSpeed = 8f;
	[SerializeField] private float crouchBobAmount = 0.025f;
	private float defaultYPos = 0f;
	private float timer;

	[Header("Zoom Parameters")]
	[SerializeField] private float timeToZoom = 0.3f;
	[SerializeField] private float zoomFOV = 30f;
	private float defaultFOV;
	private Coroutine zoomRoutine;

	[Header("Footstep Parameters")]
	[SerializeField] private float baseStepSpeed = 0.5f;
	[SerializeField] private float crouchStepMultiplier = 1.5f;
	[SerializeField] private float sprintStepMultiplayer = 0.6f;
	[SerializeField] private AudioSource footstepAudioSource = default;
	[SerializeField] private AudioClip[] tileClips = default;
	[SerializeField] private AudioClip[] woodClips = default;
	[SerializeField] private AudioClip[] metalClips = default;
	[SerializeField] private AudioClip[] grassClips = default;
	private float footstepTimer = 0;
	private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : IsSprinting ? baseStepSpeed * sprintStepMultiplayer : baseStepSpeed;

	// Sliding Parameters
	private Vector3 hitPointNormal;
	private bool IsSliding
	{
		get
		{
			if (controller.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
			{
				hitPointNormal = slopeHit.normal;
				return Vector3.Angle(hitPointNormal, Vector3.up) > controller.slopeLimit;
			}
			else
			{
				return false;
			}
		}
	}

	[Header("Interaction")]
	[SerializeField] private Vector3 interactionRayPoint = default;
	[SerializeField] private float interactionDistance = default;
	[SerializeField] private LayerMask interactionLayer = default;
	private Interactable currentInteractable;

	private Camera playerCamera;
	private CharacterController controller;

	private Vector3 moveDirection;
	private Vector2 currentInput;

	private float rotationX = 0;

	#endregion

	private void OnEnable()
	{
		OnTakeDamage += ApplyDamage;
	}

	private void OnDisable()
	{
		OnTakeDamage -= ApplyDamage;
	}

	private void Awake()
	{
		playerCamera = GetComponentInChildren<Camera>();
		controller = GetComponent<CharacterController>();
		defaultYPos = playerCamera.transform.localPosition.y;
		defaultFOV = playerCamera.fieldOfView;
		currentHealth = maxHealth;
		currentStamina = maxStamina;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		if (CanMove)
		{
			HandleMovementInput();
			HandleMouseLook();

			if (canJump)
				HandleJump();

			if (canCrouch)
				HandleCrouch();

			if (canHeadbob)
				HandleHeadbob();

			if (canZoom)
				HandleZoom();

			if (useFootsteps)
				HandleFootsteps();

			if (canInteract)
			{
				HandleInteractionCheck();
				HandleInteractionInput();
			}

			if (useStamina)
				HandleStamina();

			ApplyFinalMovements();
		}
	}

	private void HandleMovementInput()
	{
		currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"),
								   (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

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

	private void ApplyDamage(float dmg)
	{
		currentHealth -= dmg;
		OnDamage?.Invoke(currentHealth);

		if (currentHealth <= 0)
			KillPlayer();
		else if (regeneratingHealth != null)
			StopCoroutine(regeneratingHealth);

		regeneratingHealth = StartCoroutine(RegenerateHealth());
	}

	private void KillPlayer()
	{
		currentHealth = 0;
		OnDamage?.Invoke(currentHealth);

		if (regeneratingHealth != null)
			StopCoroutine(regeneratingHealth);

		print("DEAD");
	}

	private void HandleStamina()
	{
		if (IsSprinting && currentInput != Vector2.zero)
		{
			if (regeneratingStamina != null)
			{
				StopCoroutine(regeneratingStamina);
				regeneratingStamina = null;
			}

			currentStamina -= staminaUseMultiplier * Time.deltaTime;

			if (currentStamina < 0)
				currentStamina = 0;

			OnStaminaChange?.Invoke(currentStamina);

			if (currentStamina <= 0)
				canSprint = false;
		}

		if (!IsSprinting && currentStamina < maxStamina && regeneratingStamina == null)
		{
			regeneratingStamina = StartCoroutine(RegenerateStamina());
		}
	}

	private void HandleJump()
	{
		if (ShouldJump)
		{
			moveDirection.y = jumpForce;
		}
	}

	private void HandleCrouch()
	{
		if (ShouldCrouch)
			StartCoroutine(CrouchStand());
	}

	private void HandleHeadbob()
	{
		if (!controller.isGrounded)
			return;

		if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
		{
			timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
			playerCamera.transform.localPosition = new Vector3(
				playerCamera.transform.localPosition.x,
				defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
				playerCamera.transform.localPosition.z
			);
		}
	}

	private void HandleZoom()
	{
		if (Input.GetKeyDown(zoomKey))
		{
			if (zoomRoutine != null)
			{
				StopCoroutine(zoomRoutine);
				zoomRoutine = null;
			}

			zoomRoutine = StartCoroutine(ToggleZoom(true));
		}

		if (Input.GetKeyUp(zoomKey))
		{
			if (zoomRoutine != null)
			{
				StopCoroutine(zoomRoutine);
				zoomRoutine = null;
			}

			zoomRoutine = StartCoroutine(ToggleZoom(false));
		}
	}

	private void HandleInteractionCheck()
	{
		if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
		{
			if (hit.collider.gameObject.layer == 6 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
			{
				hit.collider.TryGetComponent(out currentInteractable);

				if (currentInteractable)
					currentInteractable.OnFocus();
			}
		}
		else if (currentInteractable)
		{
			currentInteractable.OnLoseFocus();
			currentInteractable = null;
		}
	}

	private void HandleInteractionInput()
	{
		if (
			Input.GetKeyDown(interactKey)
			&& currentInteractable != null
			&& Physics.Raycast(
				playerCamera.ViewportPointToRay(interactionRayPoint),
				out RaycastHit hit,
				interactionDistance,
				interactionLayer
				)
			)
		{
			currentInteractable.OnInteract();
		}
	}

	private void HandleFootsteps()
	{
		if (!controller.isGrounded)
			return;
		if (currentInput == Vector2.zero)
			return;

		footstepTimer -= Time.deltaTime;

		if (footstepTimer <= 0)
		{
			footstepAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
			if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
			{
				switch (hit.collider.tag)
				{
					case "Footsteps/WOOD":
						footstepAudioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, woodClips.Length - 1)]);
						break;
					case "Footsteps/METAL":
						footstepAudioSource.PlayOneShot(metalClips[UnityEngine.Random.Range(0, metalClips.Length - 1)]);
						break;
					case "Footsteps/GRASS":
						footstepAudioSource.PlayOneShot(grassClips[UnityEngine.Random.Range(0, grassClips.Length - 1)]);
						break;
					default:
						footstepAudioSource.PlayOneShot(tileClips[UnityEngine.Random.Range(0, tileClips.Length - 1)]);
						break;
				}
			}

			footstepTimer = GetCurrentOffset;
		}
	}

	private IEnumerator CrouchStand()
	{
		if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
			yield break;

		duringCrouchAnimation = true;

		float timeElapsed = 0;
		float targetHeight = isCrouching ? standingHeight : crouchHeight;
		float currentHeight = controller.height;
		Vector3 targetCenter = isCrouching ? standingCenter : crouchCenter;
		Vector3 currentCenter = controller.center;

		while (timeElapsed < timeToCrouch)
		{
			controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
			controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		controller.height = targetHeight;
		controller.center = targetCenter;

		isCrouching = !isCrouching;

		duringCrouchAnimation = false;
	}

	private IEnumerator ToggleZoom(bool isEnter)
	{
		float targetFOV = isEnter ? zoomFOV : defaultFOV;
		float startingFOV = playerCamera.fieldOfView;
		float timeElapsed = 0;

		while (timeElapsed < timeToZoom)
		{
			playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		playerCamera.fieldOfView = targetFOV;
		zoomRoutine = null;
	}

	private IEnumerator RegenerateHealth()
	{
		yield return new WaitForSeconds(timeBeforeHealthRegen);
		WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

		while (currentHealth < maxHealth)
		{
			currentHealth += healthIncrement;

			if (currentHealth > maxHealth)
				currentHealth = maxHealth;

			OnHeal?.Invoke(currentHealth);
			yield return timeToWait;
		}

		regeneratingHealth = null;
	}

	private IEnumerator RegenerateStamina()
	{
		yield return new WaitForSeconds(timeBeforeStaminaRegen);
		WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);

		while (currentStamina < maxStamina)
		{
			if (currentStamina > 0)
				canSprint = true;

			currentStamina += staminaIncrement;

			if (currentStamina > maxStamina)
				currentStamina = maxStamina;

			OnStaminaChange?.Invoke(currentStamina);


			yield return timeToWait;
		}

		regeneratingStamina = null;
	}

	private void ApplyFinalMovements()
	{
		if (!controller.isGrounded)
			moveDirection.y -= gravity * Time.deltaTime;

		if (controller.velocity.y < -1 && controller.isGrounded)
			moveDirection.y = 0;

		if (willSlideOnSlopes && IsSliding)
			moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * Mathf.Clamp(Vector3.Angle(hitPointNormal, Vector3.up) / controller.slopeLimit, 1, 2) * slideSpeed;

		controller.Move(moveDirection * Time.deltaTime);
	}
}