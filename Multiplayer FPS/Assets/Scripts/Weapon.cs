using UnityEngine;

public class Weapon : Interactable
{
	private FPSController player;
	private Transform weaponTransform;
	private Rigidbody weaponRigidbody;
	private BoxCollider weaponBoxCollider;

	private void Start()
	{
		// Find the player controller in the scene
		player = FindObjectOfType<FPSController>();
		weaponTransform = GetComponent<Transform>();
		weaponRigidbody = GetComponent<Rigidbody>();
		weaponBoxCollider = GetComponent<BoxCollider>();
	}

	public override void OnInteract()
	{
		player.EquipWeapon(weaponTransform, weaponRigidbody, weaponBoxCollider);
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
