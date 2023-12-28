// Copyright 2021, Infima Games. All Rights Reserved.

using UnityEngine;

namespace BulletSync.Character
{
    // Abstract Inventory Class. Helpful so you can implement your own inventory system!
    public abstract class InventoryBehaviour : MonoBehaviour
    {
        #region GETTERS

        // Returns the index that is before the current index. Very helpful in order to figure out
        // what the next weapon to equip is.
        public abstract int GetLastIndex();
        // Returns the next index after the currently equipped one. Very helpful in order to figure out
        // what the next weapon to equip is.
        public abstract int GetNextIndex();
        /// Returns the currently equipped WeaponBehaviour.
        public abstract WeaponBehaviour GetEquipped();
        /// Returns the currently equipped index. Meaning the index in the weapon array of the equipped weapon.
        public abstract int GetEquippedIndex();
        
        #endregion
        
        #region METHODS

        /// <summary>
        /// Init. This function is called when the game starts. We don't use Awake or Start because we need the
        /// PlayerCharacter component to run this with the index it wants to equip!
        /// </summary>
        /// <param name="equippedAtStart">Inventory index of the weapon we want to equip when the game starts.</param>
        public abstract void Init(int equippedAtStart = 0);
        
        /// <summary>
        /// Equips a Weapon.
        /// </summary>
        /// <param name="index">Index of the weapon to equip.</param>
        /// <returns>Weapon that was just equipped.</returns>
        public abstract WeaponBehaviour Equip(int index);

        #endregion
    }
}