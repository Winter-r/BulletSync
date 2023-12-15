using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : Interactable
{
	public override void OnInteract()
	{
		Debug.Log("INTERACTED WITH " + gameObject.name);
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
