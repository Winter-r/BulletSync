using TMPro;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
	public static AmmoManager Instance { get; set; }

	public TextMeshProUGUI ammoText;

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
	}
}