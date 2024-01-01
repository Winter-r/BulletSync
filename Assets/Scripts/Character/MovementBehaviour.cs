using UnityEngine;

namespace BulletSync
{
    // Abstract movement class. Handles interactions with the main movement component.
    public abstract class MovementBehaviour : MonoBehaviour
    {
        #region UNITY

        // Awake.
        protected virtual void Awake(){}

        // Start.
        protected virtual void Start(){}

        // Update.
        protected virtual void Update(){}

        // Fixed Update.
        protected virtual void FixedUpdate(){}

        // Late Update.
        protected virtual void LateUpdate(){}

        #endregion
    }
}