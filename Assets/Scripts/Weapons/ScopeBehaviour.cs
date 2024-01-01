using UnityEngine;

namespace BulletSync.Character
{
	// Scope Behaviour.
	public abstract class ScopeBehaviour : MonoBehaviour
	{
		#region GETTERS

		// Returns the Sprite used on the Character's Interface.
		public abstract Sprite GetSprite();
		
		#endregion
		
		#region METHODS

		public abstract float AdjustSpreadMultiplier(float WeaponBaseSpread);
	
		#endregion
	}	
}