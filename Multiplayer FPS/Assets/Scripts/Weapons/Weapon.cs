using System;
using System.Collections;
using UnityEngine;

public class WeaponInteraction : Interactable
{
	public enum ShootingMode
	{
		Single,
		Burst,
		Automatic
	}

	[Header("Bullet Properties")]
	[SerializeField] private bool isGun;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private Transform bulletSpawn;
	[SerializeField] private float bulletVelocity = 30;
	private float bulletPrefabLifeTime = 3f;

	[Header("Shooting Properties")]
	[SerializeField] private ShootingMode currentShootingMode;
	[SerializeField] private float shootingDelay = 2f;
	[SerializeField] private int bulletsPerBurst = 3;
	[SerializeField] private float spreadIntensity;
	private bool isShooting, readyToShoot;
	private bool allowReset = true;
	private int burstBulletsLeft;

	[Header("Graphics")]
	[SerializeField] private GameObject muzzleEffect;
	[SerializeField] private Animator animator;
	
	[Header("Sounds")]
	[SerializeField] private AudioSource gunAudioSource = default;
	[SerializeField] private AudioClip gunSound = default;

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

		animator = GetComponent<Animator>();
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

			if (readyToShoot && isShooting)
			{
				burstBulletsLeft = bulletsPerBurst;
				FireGun();
			}
		}
		else
		{
			return;
		}
	}

	private void FireGun()
	{
		muzzleEffect.GetComponent<ParticleSystem>().Play();
		if (animator != null)
			animator.SetTrigger("Recoil");
		
		if (gunSound != null && gunAudioSource != null)
			gunAudioSource.PlayOneShot(gunSound);

		// Prevents shooting while already shooting, causing player to shoot twice before the first shot ends.
		readyToShoot = false;

		Vector3 shootingDirection = CalculateSpreadAndDirection().normalized;

		GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

		bullet.transform.forward = shootingDirection;

		bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

		StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));

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

		float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
		float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

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
