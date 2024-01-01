using UnityEngine;
using UnityEngine.InputSystem;

namespace BulletSync.Character
{
	public class PlayerManager : MonoBehaviour
	{
		[SerializeField] private GameObject playerCameraSocket;
		[SerializeField] private Camera playerCameraDepth;
		[SerializeField] private CameraLook cameraLook;
		[SerializeField] private Character character;
		[SerializeField] private Movement movement;
		[SerializeField] private PlayerInput playerInput;
		[SerializeField] private AudioListener audioListener;

		public void IsLocalPlayer()
		{
			SetChildrenLayers(this.transform);
			// weird syntax ik
			// this basically means select "First Person View" from the camera's culling mask
			playerCameraDepth.cullingMask ^= 1 << LayerMask.NameToLayer("First Person View");
			// this basically means deselect "Enemy View" from the camera's culling mask
			playerCameraDepth.cullingMask &= ~(1 << LayerMask.NameToLayer("Enemy View"));

			character.enabled = true;
			movement.enabled = true;
			playerInput.enabled = true;
			cameraLook.enabled = true;
			audioListener.enabled = true;
			playerCameraSocket.SetActive(true);
		}

		private void SetChildrenLayers(Transform root)
		{
			foreach (Transform child in root)
			{
				if (child.gameObject.layer == LayerMask.NameToLayer("Enemy View"))
				{
					child.gameObject.layer = LayerMask.NameToLayer("First Person View");
				}

				SetChildrenLayers(child);
			}
		}
	}
}