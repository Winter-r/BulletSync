﻿using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace BulletSync.Character
{
	[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
	public class Movement : MovementBehaviour
	{
		#region FIELDS SERIALIZED

		[Header("Audio Clips")]

		[Tooltip("The audio clip that is played while walking.")]
		[SerializeField]
		private AudioClip audioClipWalking;

		[Tooltip("The audio clip that is played while running.")]
		[SerializeField]
		private AudioClip audioClipRunning;

		[Header("Speeds")]

		[SerializeField]
		private float speedWalking = 5.0f;

		[Tooltip("How fast the player moves while running."), SerializeField]
		private float speedRunning = 9.0f;

		#endregion

		#region PROPERTIES

		//Velocity.
		private Vector3 Velocity
		{
			//Getter.
			get => rigidBody.velocity;
			//Setter.
			set => rigidBody.velocity = value;
		}

		#endregion

		#region FIELDS

		// Attached Rigidbody.
		private Rigidbody rigidBody;
		// Attached CapsuleCollider.
		private CapsuleCollider capsule;
		// Attached AudioSource.
		private AudioSource audioSource;

		// True if the character is currently grounded.
		private bool grounded;

		// Player Character.
		private CharacterBehaviour playerCharacter;
		// The player character's equipped weapon.
		private WeaponBehaviour equippedWeapon;

		// Array of RaycastHits used for ground checking.
		private readonly RaycastHit[] groundHits = new RaycastHit[8];

		#endregion

		#region UNITY FUNCTIONS

		// Awake.
		protected override void Awake()
		{
			//Get Player Character.
			playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
		}

		// Initializes the FpsController on start.
		protected override void Start()
		{
			//Rigidbody Setup.
			rigidBody = GetComponent<Rigidbody>();
			rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
			//Cache the CapsuleCollider.
			capsule = GetComponent<CapsuleCollider>();

			//Audio Source Setup.
			audioSource = GetComponent<AudioSource>();
			audioSource.clip = audioClipWalking;
			audioSource.loop = true;
		}

		// Checks if the character is on the ground.
		private void OnCollisionStay()
		{
			if (playerCharacter != null)
			{
				PhotonView photonView = playerCharacter.GetPhotonView();
				if (photonView != null)
				{
					if (photonView.IsMine)
					{
						//Bounds.
						Bounds bounds = capsule.bounds;
						//Extents.
						Vector3 extents = bounds.extents;
						//Radius.
						float radius = extents.x - 0.01f;

						//Cast. This checks whether there is indeed ground, or not.
						Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
							groundHits, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);

						//We can ignore the rest if we don't have any proper hits.
						if (!groundHits.Any(hit => hit.collider != null && hit.collider != capsule))
							return;

						//Store RaycastHits.
						for (var i = 0; i < groundHits.Length; i++)
							groundHits[i] = new RaycastHit();

						//Set grounded. Now we know for sure that we're grounded.
						grounded = true;
					}
					else
					{
						Debug.LogWarning("PhotonView is not owned by the local player.");
					}
				}
				else
				{
					Debug.LogWarning("PhotonView is null on playerCharacter.");
				}
			}
			else
			{
				Debug.LogWarning("playerCharacter is null.");
			}
		}

		protected override void FixedUpdate()
		{
			//Move.
			MoveCharacter();

			//Unground.
			grounded = false;
		}

		// Moves the camera to the character, processes jumping and plays sounds every frame.
		protected override void Update()
		{
			//Get the equipped weapon!
			equippedWeapon = playerCharacter.GetInventory().GetEquipped();

			//Play Sounds!
			PlayFootstepSounds();

		}

		#endregion

		#region METHODS

		private void MoveCharacter()
		{
			#region Calculate Movement Velocity

			//Get Movement Input!
			Vector2 frameInput = playerCharacter.GetInputMovement();
			//Calculate local-space direction by using the player's input.
			var movement = new Vector3(frameInput.x, 0.0f, frameInput.y);

			//Running speed calculation.
			if (playerCharacter.IsRunning())
				movement *= speedRunning;
			else
			{
				//Multiply by the normal walking speed.
				movement *= speedWalking;
			}

			//World space velocity calculation. This allows us to add it to the rigidbody's velocity properly.
			movement = transform.TransformDirection(movement);

			#endregion

			//Update Velocity.
			Velocity = new Vector3(movement.x, 0.0f, movement.z);
		}

		// Plays Footstep Sounds. This code is slightly old, so may not be great, but it functions alright-y!
		private void PlayFootstepSounds()
		{
			//Check if we're moving on the ground. We don't need footsteps in the air.
			if (grounded && rigidBody.velocity.sqrMagnitude > 0.1f)
			{
				//Select the correct audio clip to play.
				audioSource.clip = playerCharacter.IsRunning() ? audioClipRunning : audioClipWalking;
				//Play it!
				if (!audioSource.isPlaying)
					audioSource.Play();
			}
			//Pause it if we're doing something like flying, or not moving!
			else if (audioSource.isPlaying)
				audioSource.Pause();
		}

		#endregion
	}
}