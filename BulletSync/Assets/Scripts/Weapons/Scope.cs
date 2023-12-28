using System;
using UnityEngine;

namespace BulletSync.Character
{
	// Weapon Scope.
	public class Scope : ScopeBehaviour
	{
		#region FIELDS SERIALIZED

		[Header("Interface")]

		[Tooltip("Interface Sprite.")]
		[SerializeField]
		private Sprite sprite;

		[Header("Spread")]
		[Tooltip("Decrease spread by how much whilst aiming")]
		[SerializeField, Range(0, 1)]
		private float spreadMultiplier = 0.5f;

		#endregion

		#region GETTERS

		public override Sprite GetSprite() => sprite;

		#endregion

		#region METHODS

		public override float AdjustSpreadMultiplier(float weaponBaseSpread)
		{
			return weaponBaseSpread * spreadMultiplier;
		}

		#endregion
	}
}