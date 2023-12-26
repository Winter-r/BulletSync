using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
	// Rotation
	private Vector3 currentRotation;
	private Vector3 targetRotation;

	private bool isAiming;

	// References
	private RoomManager roomManager;
	private FPSController player;
	private Weapon currentWeapon;

	private void Awake()
	{
		roomManager = FindObjectOfType<RoomManager>();
		player = roomManager.player.GetComponent<FPSController>();
	}

	private void Update()
	{
		currentWeapon = player.equippedWeaponsTransforms[player.currentWeaponIndex].gameObject.GetComponent<Weapon>();
		isAiming = player.isAiming;

		targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, currentWeapon.returnSpeed * Time.deltaTime);
		currentRotation = Vector3.Slerp(currentRotation, targetRotation, currentWeapon.snappiness * Time.fixedDeltaTime);
		transform.localRotation = Quaternion.Euler(currentRotation);
	}

	public void RecoilFire()
	{
		if (isAiming) targetRotation += new Vector3(currentWeapon.aimRecoilX, Random.Range(-currentWeapon.aimRecoilY, currentWeapon.aimRecoilY), Random.Range(-currentWeapon.aimRecoilZ, currentWeapon.aimRecoilZ));
		else targetRotation += new Vector3(currentWeapon.recoilX, Random.Range(-currentWeapon.recoilY, currentWeapon.recoilY), Random.Range(-currentWeapon.recoilZ, currentWeapon.recoilZ));
	}
}
