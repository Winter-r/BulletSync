using UnityEngine;

namespace BulletSync.Character
{
	public abstract class CharacterBehaviour : MonoBehaviour
	{
		#region UNITY

		protected virtual void Awake(){}

		protected virtual void Start(){}

		protected virtual void Update(){}

		protected virtual void LateUpdate(){}

		#endregion
		
		#region GETTERS

		// Returns the player character's main camera.
		public abstract Camera GetCameraWorld();
		
		// Returns a reference to the Inventory component.
		public abstract InventoryBehaviour GetInventory();

		// Returns true if the Crosshair should be visible.
		public abstract bool IsCrosshairVisible();

		// Returns true if the character is running.
		public abstract bool IsRunning();
		
		// Returns true if the character is aiming.
		public abstract bool IsAiming();

		// Returns true if the game cursor is locked.
		public abstract bool IsCursorLocked();

		// Returns true if the tutorial text should be visible on the screen.
		public abstract bool IsTutorialTextVisible();

		// Returns the Movement Input
		public abstract Vector2 GetInputMovement();

		// Returns the Look Input.
		public abstract Vector2 GetInputLook();
		
		#endregion

		#region ANIMATION

		// Ejects a casing from the equipped weapon.
		public abstract void EjectCasing();
		
		// Fills the character's equipped weapon's ammunition by a certain amount, or fully if set to -1.
		public abstract void FillAmmunition(int amount);

		// Sets the equipped weapon's magazine to be active or inactive!
		public abstract void SetActiveMagazine(int active);
		
		// Reload Animation Ended.
		public abstract void AnimationEndedReload();

		// Inspect Animation Ended.
		public abstract void AnimationEndedInspect();

		// Holster Animation Ended.
		public abstract void AnimationEndedHolster();

		#endregion
	}
}