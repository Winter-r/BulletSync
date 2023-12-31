﻿using System.Globalization;

namespace BulletSync.Character.Interface
{
    /// <summary>
    /// Total Ammunition Text.
    /// </summary>
    public class TextAmmunitionTotal : ElementText
    {
        #region METHODS
        
        /// <summary>
        /// Tick.
        /// </summary>
        protected override void Tick()
        {
            //Total Ammunition.
            float ammunitionTotal = equippedWeapon.GetAmmunitionTotal();
            
            //Update Text.
            textMesh.text = ammunitionTotal.ToString(CultureInfo.InvariantCulture);
        }
        
        #endregion
    }
}