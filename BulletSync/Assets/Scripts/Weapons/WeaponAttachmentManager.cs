﻿using UnityEngine;

namespace BulletSync.Character
{
    // Weapon Attachment Manager. Handles equipping and storing a Weapon's Attachments.
    public class WeaponAttachmentManager : WeaponAttachmentManagerBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Scope")]

        [Tooltip("Determines if the ironsights should be shown on the weapon model.")]
        [SerializeField]
        private bool scopeDefaultShow = true;
        
        [Tooltip("Default Scope!")]
        [SerializeField]
        private ScopeBehaviour scopeDefaultBehaviour;
        
        [Header("Muzzle")]
        [Tooltip("Selected Muzzle Index.")]
        [SerializeField]
        private int muzzleIndex;

        [Tooltip("All possible Muzzle Attachments that this Weapon can use!")]
        [SerializeField]
        private MuzzleBehaviour[] muzzleArray;
        
        [Header("Magazine")]

        [Tooltip("Selected Magazine Index.")]
        [SerializeField]
        private int magazineIndex;

        [Tooltip("All possible Magazine Attachments that this Weapon can use!")]
        [SerializeField]
        private Magazine[] magazineArray;

        #endregion

        #region FIELDS

        // Equipped Scope.
        private ScopeBehaviour scopeBehaviour;
        // Equipped Muzzle.
        private MuzzleBehaviour muzzleBehaviour;
        // Equipped Magazine.
        private MagazineBehaviour magazineBehaviour;

        #endregion

        #region UNITY FUNCTIONS

        protected override void Awake()
        {
            //Check if we have no scope. This could happen if we have an incorrect index.
            if (scopeBehaviour == null)
            {
                //Select Default Scope.
                scopeBehaviour = scopeDefaultBehaviour;
                //Set Active.
                scopeBehaviour.gameObject.SetActive(scopeDefaultShow);
            }
            
            //Select Muzzle!
            muzzleBehaviour = muzzleArray.SelectAndSetActive(muzzleIndex);

            //Select Magazine!
            magazineBehaviour = magazineArray.SelectAndSetActive(magazineIndex);
        }        

        #endregion

        #region GETTERS

        public override ScopeBehaviour GetEquippedScope() => scopeBehaviour;
        public override ScopeBehaviour GetEquippedScopeDefault() => scopeDefaultBehaviour;

        public override MagazineBehaviour GetEquippedMagazine() => magazineBehaviour;
        public override MuzzleBehaviour GetEquippedMuzzle() => muzzleBehaviour;

        #endregion
    }
}