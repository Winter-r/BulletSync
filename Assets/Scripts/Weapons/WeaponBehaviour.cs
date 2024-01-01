using UnityEngine;

namespace BulletSync.Character
{
	public abstract class WeaponBehaviour : MonoBehaviour
	{
		#region UNITY

		/// Awake.
		protected virtual void Awake() { }

		/// Start.
		protected virtual void Start() { }

		/// Update.
		protected virtual void Update() { }

		/// Late Update.
		protected virtual void LateUpdate() { }

		#endregion

		#region GETTERS
		
		// Returns the Hipfire spread accuracy
		public abstract float GetHipfireSpread();

		/// Returns the sprite to use when displaying the weapon's body.
		public abstract Sprite GetSpriteBody();

		/// Returns the holster audio clip.
		public abstract AudioClip GetAudioClipHolster();
		/// Returns the unholster audio clip.
		public abstract AudioClip GetAudioClipUnholster();

		/// Returns the reload audio clip.
		public abstract AudioClip GetAudioClipReload();
		/// Returns the reload empty audio clip.
		public abstract AudioClip GetAudioClipReloadEmpty();

		/// Returns the fire empty audio clip.
		public abstract AudioClip GetAudioClipFireEmpty();

		/// Returns the fire audio clip.
		public abstract AudioClip GetAudioClipFire();

		/// Returns Current Ammunition. 
		public abstract int GetAmmunitionCurrent();
		/// Returns Total Ammunition.
		public abstract int GetAmmunitionTotal();

		/// Returns the Weapon's Animator component.
		public abstract Animator GetAnimator();

		/// Returns true if this weapon shoots in automatic.
		public abstract bool IsAutomatic();
		/// Returns true if the weapon has any ammunition left.
		public abstract bool HasAmmunition();

		/// Returns true if the weapon is full of ammunition.
		public abstract bool IsFull();
		/// Returns the weapon's rate of fire.
		public abstract float GetRateOfFire();

		/// Returns the RuntimeAnimationController the Character needs to use when this Weapon is equipped!
		public abstract RuntimeAnimatorController GetAnimatorController();
		/// Returns the weapon's attachment manager component.
		public abstract WeaponAttachmentManagerBehaviour GetAttachmentManager();

		#endregion

		#region METHODS

		/// Fires the weapon.
		/// <param name="spreadMultiplier">Value to multiply the weapon's spread by. Very helpful to account for aimed spread multipliers.</param>
		public abstract void Fire();
		// Reloads the weapon.
		public abstract void Reload();

		/// Fills the character's equipped weapon's ammunition by a certain amount, or fully if set to -1.
		public abstract void FillAmmunition(int amount);

		/// Ejects a casing from the weapon. This is commonly called from animation events, but can be called from anywhere.
		public abstract void EjectCasing();

		#endregion
	}
}