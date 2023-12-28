using UnityEngine;

namespace BulletSync.Character
{
    /// <summary>
    /// Magazine Behaviour.
    /// </summary>
    public abstract class MagazineBehaviour : MonoBehaviour
    {
        #region GETTERS
        
        /// <summary>
        /// Returns The Total Ammunition.
        /// </summary>
        public abstract int GetAmmunitionTotal();
        /// <summary>
        /// Returns the Sprite used on the Character's Interface.
        /// </summary>
        public abstract Sprite GetSprite();

        #endregion
    }
}