using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	public static UI Instance { get; set; }
	private static FPSController player;

	[Header("Health & Stamina")]
	[SerializeField] private TextMeshProUGUI healthText = default;
	[SerializeField] private TextMeshProUGUI staminaText = default;

	[Header("Ammo")]
	public TextMeshProUGUI currentAmmoUI;
	public TextMeshProUGUI magazineAmmoUI;
	public Image ammoTypeUI;

	[Header("Weapon")]
	public Weapon activeWeapon;
	public Weapon[] inactiveWeapons;
	public Image activeWeaponUI;
	public Image[] inactiveWeaponsUI;

	[Header("Throwables")]
	[Header("Lethal")]
	public Image lethalUI;
	public TextMeshProUGUI lethalAmountUI;

	[Header("Tactical")]
	public Image tacticalUI;
	public TextMeshProUGUI tacticalAmountUI;

	// Weapon Sprites
	private GameObject M1911WeaponSprite;
	private GameObject AK74WeaponSprite;
	private GameObject M107WeaponSprite;
	private GameObject benelliM4WeaponSprite;
	private GameObject knifeWeaponSprite;

	// Ammo Sprites
	private GameObject pistolAmmoSprite;
	private GameObject rifleAmmoSprite;
	private GameObject sniperAmmoSprite;
	private GameObject shotgunAmmoSprite;
	private GameObject noAmmoSprite;
	public Sprite emptySlot;

	private void OnEnable()
	{
		FPSController.OnDamage += UpdateHealth;
		FPSController.OnHeal += UpdateHealth;
		FPSController.OnStaminaChange += UpdateStamina;
	}

	private void OnDisable()
	{
		FPSController.OnDamage -= UpdateHealth;
		FPSController.OnHeal -= UpdateHealth;
		FPSController.OnStaminaChange -= UpdateStamina;
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

		player = FindObjectOfType<FPSController>();
		M1911WeaponSprite = Resources.Load<GameObject>("M1911_Weapon");
		AK74WeaponSprite = Resources.Load<GameObject>("AK74_Weapon");
		M107WeaponSprite = Resources.Load<GameObject>("M107_Weapon");
		benelliM4WeaponSprite = Resources.Load<GameObject>("Benelli_M4_Weapon");
		knifeWeaponSprite = Resources.Load<GameObject>("Knife_Weapon");

		pistolAmmoSprite = Resources.Load<GameObject>("Pistol_Ammo");
		rifleAmmoSprite = Resources.Load<GameObject>("Rifle_Ammo");
		sniperAmmoSprite = Resources.Load<GameObject>("Sniper_Ammo");
		shotgunAmmoSprite = Resources.Load<GameObject>("Shotgun_Ammo");
		noAmmoSprite = Resources.Load<GameObject>("No_Ammo");
	}

	private void Start()
	{
		UpdateHealth(100);
		UpdateStamina(100);
	}

	private void Update()
	{
		UpdateWeaponUI();
	}

	public void UpdateWeaponUI()
	{
		activeWeapon = player.equippedWeaponsTransforms[player.currentWeaponIndex].GetComponentInChildren<Weapon>();
		inactiveWeapons = new Weapon[player.equippedWeaponsTransforms.Count - 1];
		
		int index = 0;
		for (int i = 0; i < player.equippedWeaponsTransforms.Count; i++)
		{
			if (i != player.currentWeaponIndex)
			{
				inactiveWeapons[index] = player.equippedWeaponsTransforms[i].GetComponentInChildren<Weapon>();
				index++;
			}
		}

		if (activeWeapon)
		{
			currentAmmoUI.text = $"{activeWeapon.bulletsLeft}";
			magazineAmmoUI.text = $"{activeWeapon.magazineSize}";

			Weapon.WeaponType model = activeWeapon.thisWeaponModel;
			ammoTypeUI.sprite = GetAmmoSprite(model);
			activeWeaponUI.sprite = GetWeaponSprite(model);

			for (int i = 0; i < inactiveWeaponsUI.Length; i++)
			{
				if (i < inactiveWeapons.Length && inactiveWeapons[i] != null)
				{
					inactiveWeaponsUI[i].sprite = GetWeaponSprite(inactiveWeapons[i].thisWeaponModel);
				}
				else
				{
					inactiveWeaponsUI[i].sprite = emptySlot;
				}
			}
		}
		else
		{
			currentAmmoUI.text = "";
			magazineAmmoUI.text = "";

			ammoTypeUI.sprite = emptySlot;
			for (int i = 0; i < inactiveWeaponsUI.Length; i++)
			{
				inactiveWeaponsUI[i].sprite = emptySlot;
			}
		}
	}

	private Sprite GetWeaponSprite(Weapon.WeaponType type)
	{
		return type switch
		{
			Weapon.WeaponType.M1911 => M1911WeaponSprite.GetComponent<SpriteRenderer>().sprite,
			Weapon.WeaponType.AK74 => AK74WeaponSprite.GetComponent<SpriteRenderer>().sprite,
			Weapon.WeaponType.M107 => M107WeaponSprite.GetComponent<SpriteRenderer>().sprite,
			Weapon.WeaponType.Benelli_M4 => benelliM4WeaponSprite.GetComponent<SpriteRenderer>().sprite,
			Weapon.WeaponType.Knife => knifeWeaponSprite.GetComponent<SpriteRenderer>().sprite,
			_ => null
		};
	}

	private Sprite GetAmmoSprite(Weapon.WeaponType type)
	{
		return type switch
		{
			Weapon.WeaponType.M1911 => pistolAmmoSprite.GetComponent<SpriteRenderer>().sprite,
			Weapon.WeaponType.AK74 => rifleAmmoSprite.GetComponent<SpriteRenderer>().sprite,
			Weapon.WeaponType.M107 => sniperAmmoSprite.GetComponent<SpriteRenderer>().sprite,
			Weapon.WeaponType.Benelli_M4 => shotgunAmmoSprite.GetComponent<SpriteRenderer>().sprite,
			Weapon.WeaponType.Knife => noAmmoSprite.GetComponent<SpriteRenderer>().sprite,
			_ => null,
		};
	}

	private void UpdateHealth(float currentHealth)
	{
		healthText.text = currentHealth.ToString("00 HP");
	}

	private void UpdateStamina(float currentStamina)
	{
		staminaText.text = currentStamina.ToString("00 Stamina");
	}
}
