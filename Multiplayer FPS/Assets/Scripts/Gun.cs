using UnityEngine;

public class Gun : Interactable
{
	[SerializeField] private FPSController player;
	private Rigidbody gunRigidbody;
	private BoxCollider gunCollider;

	private void Awake()
	{
		gunRigidbody = GetComponent<Rigidbody>();
		gunCollider = GetComponent<BoxCollider>();
	}
	
	public override void OnInteract()
	{
		player.EquipGun(gameObject.transform, gunRigidbody, gunCollider);
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
