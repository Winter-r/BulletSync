using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
	[SerializeField] private List<Rigidbody> allParts = new List<Rigidbody>();

	public void Shatter()
	{
		foreach (Rigidbody part in allParts)
		{
			part.isKinematic = false;
		}
	}
}
