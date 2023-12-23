using System;
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
	private CameraRecoil recoilScript;
	private Outline outline;

	[Header("Shooting Properties")]
	[SerializeField] private ShootingMode currentShootingMode;
	[SerializeField] private float shootingDelay = 2f;
	[HideInInspector] public int bulletsLeft;
	[HideInInspector] public int burstBulletsLeft;
	[HideInInspector] public bool isShooting;
	[HideInInspector] public bool readyToShoot;
	[HideInInspector] public bool isReloading;
	private bool allowReset = true;
	public int magazineSize;
	public float reloadTime;

	[Header("Bullet Properties")]
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private Transform bulletSpawn;
	[SerializeField] private float bulletVelocity = 30;
	public int bulletsPerBurst = 3;
	private float bulletPrefabLifeTime = 3f;

	[Header("Spread")]
	[SerializeField] private float hipSpreadIntensity;
	[SerializeField] private float adsSpreadIntensity;
	private float spreadIntensity;

	[Header("ADS")]
	[SerializeField] private float adsSpeed;
	private bool isADS;

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
		animator = GetComponent<Animator>();
		outline = GetComponent<Outline>();

		outline.enabled = false;
		readyToShoot = true;
		burstBulletsLeft = bulletsPerBurst;
		bulletsLeft = magazineSize;
		spreadIntensity = hipSpreadIntensity;

		recoilScript = GameObject.Find("CameraRotation/CameraRecoil").GetComponent<CameraRecoil>();
	}

	private void Update()
	{
		animator.enabled = isEquipped;
		HandleFiringInput();
		HandleADSInput();
	}

	private void HandleFiringInput()
	{
		if (isGun && isEquipped)
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

		recoilScript.RecoilFire();

		muzzleEffect.GetComponent<ParticleSystem>().Play();

		if (isADS)
		{
			animator.SetTrigger("recoilADS");
		}
		else
		{
			animator.SetTrigger("Recoil");
		}

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
		// Play Reload Animation Here
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

		float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
		float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

		return direction + new Vector3(0, y, z);
	}

	private IEnumerator DestroyBullet(GameObject bullet, float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(bullet);
	}

	private void HandleADSInput()
	{
		if (Input.GetKeyDown(player.ADSKey))
			EnterADS();


		if (Input.GetKeyUp(player.ADSKey))
			ExitADS();

	}

	private void EnterADS()
	{
		isADS = true;
		animator.SetTrigger("enterADS");
		player.playerCamera.GetComponent<Animator>().SetTrigger("zoomIn");
		UI.Instance.crossHair.SetActive(false);
		spreadIntensity = adsSpreadIntensity;
	}

	private void ExitADS()
	{
		isADS = false;
		animator.SetTrigger("exitADS");
		player.playerCamera.GetComponent<Animator>().SetTrigger("zoomOut");
		UI.Instance.crossHair.SetActive(true);
		spreadIntensity = hipSpreadIntensity;
	}

	public override void OnInteract()
	{
		player.EquipWeapon(weaponTransform, weaponRigidbody, weaponCollider, animator);
	}

	public override void OnFocus()
	{
		if (!isEquipped)
		{
			outline.enabled = true;
		}
	}

	public override void OnLoseFocus()
	{
		// You stopped here, figure out why OnLoseFocus isn't being called
		outline.enabled = false;
		Debug.Log("Lost Focus on" + gameObject.name);
	}
}
