using UnityEngine;
using UnityEngine.InputSystem;

namespace BulletSync.Character
{
	public class PlayerManager : MonoBehaviour
	{
		[SerializeField] private GameObject playerCamera;
		[SerializeField] private CameraLook cameraLook;
		[SerializeField] private Character character;
		[SerializeField] private Movement movement;
		[SerializeField] private PlayerInput playerInput;
		[SerializeField] private AudioListener audioListener;

		public void IsLocalPlayer()
		{
			character.enabled = true;
			movement.enabled = true;
			playerInput.enabled = true;
			cameraLook.enabled = true;
			audioListener.enabled = true;
			playerCamera.SetActive(true);
		}
	}
}