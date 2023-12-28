using UnityEngine;

namespace BulletSync.Character
{
    // Weapon Attachment Manager Behaviour.
    public abstract class WeaponAttachmentManagerBehaviour : MonoBehaviour
    {
        #region UNITY FUNCTIONS

        /// <summary>
        /// Awake.
        /// </summary>
        protected virtual void Awake(){}

        /// <summary>
        /// Start.
        /// </summary>
        protected virtual void Start(){}

        /// <summary>
        /// Update.
        /// </summary>
        protected virtual void Update(){}

        /// <summary>
        /// Late Update.
        /// </summary>
        protected virtual void LateUpdate(){}

        #endregion
        
        #region GETTERS

        // Returns the equipped scope.
        public abstract ScopeBehaviour GetEquippedScope();
        // Returns the equipped scope default.
        public abstract ScopeBehaviour GetEquippedScopeDefault();
        
        // Returns the equipped magazine.
        public abstract MagazineBehaviour GetEquippedMagazine();
        // Returns the equipped muzzle.
        public abstract MuzzleBehaviour GetEquippedMuzzle();
        
        #endregion
    }
}