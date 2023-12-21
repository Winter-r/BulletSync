using System.Collections;
using UnityEngine;

public class Weapon : Interactable
{
	public enum ShootingMode
	{
		Single,
		Burst,
		Automatic
	}
	public enum WeaponType
	{
		AK74,
		M107,
		M1911,
		Benelli_M4,
		Knife
	}

	[Header("General Properties")]
	[SerializeField] private bool isGun;
	[HideInInspector] public bool isEquipped = false;
	public WeaponType thisWeaponModel;
	private CameraRecoil Recoil_Script;

	[Header("Shooting Properties")]
	[SerializeField] private ShootingMode currentShootingMode;
	[SerializeField] private float shootingDelay = 2f;
	[SerializeField] private float spreadIntensity;
	[HideInInspector] public int bulletsLeft;
	[HideInInspector] public int burstBulletsLeft;
	[HideInInspector] public bool isShooting;
	[HideInInspector] public bool readyToShoot;
	private bool allowReset = true;
	private bool isReloading;
	public int magazineSize;
	public float reloadTime;

	[Header("Bullet Properties")]
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private Transform bulletSpawn;
	[SerializeField] private float bulletVelocity = 30;
	public int bulletsPerBurst = 3;
	private float bulletPrefabLifeTime = 3f;

	[Header("Graphics")]
	[SerializeField] private GameObject muzzleEffect;
	private Animator animator;

	[Header("Sounds")]
	[SerializeField] private AudioSource gunAudioSource = default;
	[SerializeField] private AudioClip gunFiringSound = default;
	[SerializeField] private AudioClip gunReloadSound = default;

	[Header("Recoil")]
	// Hipfire Recoil
	public float recoilX;
	public float recoilY;
	public float recoilZ;

	// ADS Recoil
	public float aimRecoilX;
	public float aimRecoilY;
	public float aimRecoilZ;

	// Settings
	public float snappiness;
	public float returnSpeed;

	private FPSController player;
	private Transform weaponTransform;
	private Rigidbody weaponRigidbody;
	private MeshCollider weaponCollider;

	private void Awake()
	{
		// Find the player controller in the scene
		player = FindObjectOfType<FPSController>();

		weaponTransform = GetComponent<Transform>();
		weaponRigidbody = GetComponent<Rigidbody>();
		weaponCollider = GetComponent<MeshCollider>();

		readyToShoot = true;
		burstBulletsLeft = bulletsPerBurst;
		bulletsLeft = magazineSize;

		animator = GetComponent<Animator>();
		Recoil_Script = GameObject.Find("CameraRotation/CameraRecoil").GetComponent<CameraRecoil>();
	}

	private void Update()
	{
		HandleFiringInput();
	}

	private void HandleFiringInput()
	{
		if (isGun)
		{
			isShooting = currentShootingMode == ShootingMode.Automatic
						 ? Input.GetKey(player.shootingKey)
						 : Input.GetKeyDown(player.shootingKey);

			if (isEquipped && readyToShoot && isShooting && bulletsLeft > 0)
			{
				burstBulletsLeft = bulletsPerBurst;
				FireGun();
			}

			if (Input.GetKeyDown(player.reloadKey) && bulletsLeft < magazineSize && !isReloading)
			{
				Reload();
			}

			if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0)
			{
				Reload();
			}
		}
		else
		{
			return;
		}
	}

	private void FireGun()
	{
		bulletsLeft--;

		Recoil_Script.RecoilFire();

		muzzleEffect.GetComponent<ParticleSystem>().Play();
		if (animator != null)
			animator.SetTrigger("Recoil");

		if (gunFiringSound != null && gunAudioSource != null)
			gunAudioSource.PlayOneShot(gunFiringSound);

		// Prevents shooting while already shooting, causing player to shoot twice before the first shot ends.
		readyToShoot = false;

		for (int i = 0; i < bulletsPerBurst; i++)
		{
			Vector3 shootingDirection = CalculateSpreadAndDirection().normalized;

			GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

			bullet.transform.forward = shootingDirection;

			bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

			StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));
		}

		// Check if we are done shooting
		if (allowReset)
		{
			Invoke("ResetShot", shootingDelay);
			allowReset = false;
		}

		// Burst
		if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
		{
			burstBulletsLeft--;
			Invoke("FireWeapon", shootingDelay);
		}
	}

	private void Reload()
	{
		if (gunReloadSound != null)
			gunAudioSource.PlayOneShot(gunReloadSound);

		isReloading = true;
		// Play Reload Animation
		Invoke("ReloadComplete", reloadTime);
	}

	private void ReloadComplete()
	{
		bulletsLeft = magazineSize;
		isReloading = false;
	}

	private void ResetShot()
	{
		readyToShoot = true;
		allowReset = true;
	}

	private Vector3 CalculateSpreadAndDirection()
	{
		Ray ray = player.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

		Vector3 targetPoint;
		if (Physics.Raycast(ray, out hit))
			targetPoint = hit.point;
		else
			targetPoint = ray.GetPoint(100);

		Vector3 direction = targetPoint - bulletSpawn.position;

		float x = Random.Range(-spreadIntensity, spreadIntensity);
		float y = Random.Range(-spreadIntensity, spreadIntensity);

		return direction + new Vector3(x, y, 0);
	}

	private IEnumerator DestroyBullet(GameObject bullet, float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(bullet);
	}

	public override void OnInteract()
	{
		player.EquipWeapon(weaponTransform, weaponRigidbody, weaponCollider);
	}

	public override void OnFocus()
	{
		Debug.Log("LOOKING AT " + gameObject.name);
	}

	public override void OnLoseFocus()
	{
		Debug.Log("STOPPED LOOKING AT " + gameObject.name);
	}
}
