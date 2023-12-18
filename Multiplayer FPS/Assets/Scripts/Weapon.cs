using System;
using TMPro;
using UnityEngine;

public class Weapon : Interactable
{
	[Header("Gun Parameters")]
	[SerializeField] private bool isGun;
	[SerializeField] private bool allowHolding;
	[SerializeField] private int magSize, bulletsPerTap;
	[SerializeField] private float damage, timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
	[SerializeField] private Transform attackPoint = default;
	[SerializeField] private TextMeshProUGUI ammoText = default;
	[SerializeField] private GameObject muzzleFlash, bulletHoleGraphics;
	private bool shooting, readyToShoot, reloading;
	private int bulletsLeft, bulletsShot;

	public static Action<float> OnAmmoChange;

	private FPSController player;
	private Transform weaponTransform;
	private Rigidbody weaponRigidbody;
	private MeshCollider weaponCollider;

	private void Start()
	{
		// Find the player controller in the scene
		player = FindObjectOfType<FPSController>();
		weaponTransform = GetComponent<Transform>();
		weaponRigidbody = GetComponent<Rigidbody>();
		weaponCollider = GetComponent<MeshCollider>();

		if (isGun)
		{
			bulletsLeft = magSize;
			readyToShoot = true;
		}
	}

	private void Update()
	{
		HandleWeaponInput();
		
		if (isGun)
			ammoText.SetText(bulletsLeft + " / " + magSize);
	}

	private void HandleWeaponInput()
	{
		if (isGun)
		{
			shooting = allowHolding ? Input.GetKey(player.shootingKey) : Input.GetKeyDown(player.shootingKey);

			if ((Input.GetKeyDown(player.reloadKey) && bulletsLeft < magSize && !reloading) || (bulletsLeft == 0 && !reloading))
				Reload();

			if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
			{
				bulletsShot = bulletsPerTap;
				Shoot();
			}
		}
	}

	private void Shoot()
	{
		if (!isGun)
			return;
		
		readyToShoot = false;

		// Spread
		float x = UnityEngine.Random.Range(-spread, spread);
		float y = UnityEngine.Random.Range(-spread, spread);

		Vector3 direction = player.playerCamera.transform.forward + new Vector3(x, y, 0);

		if (Physics.Raycast(player.playerCamera.transform.position, direction, out RaycastHit hit, range, player.gameObject.layer))
		{
			Debug.Log(hit.collider.name);

			if (hit.collider.CompareTag("Player"))
				hit.collider.GetComponent<FPSController>().ApplyDamage(damage);
		}

		Instantiate(bulletHoleGraphics, hit.point, Quaternion.Euler(0, 180, 0));
		Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

		bulletsLeft--;
		bulletsShot--;

		Invoke("ResetShot", timeBetweenShooting);

		if (bulletsShot > 0 && bulletsLeft > 0)
			Invoke("Shoot", timeBetweenShots);
	}

	private void ResetShot()
	{
		readyToShoot = true;
	}

	private void Reload()
	{
		reloading = true;
		Invoke("ReloadFinished", reloadTime);
	}

	private void ReloadFinished()
	{
		bulletsLeft = magSize;
		reloading = false;
		Debug.Log("Finished Reloading " + bulletsLeft);
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
