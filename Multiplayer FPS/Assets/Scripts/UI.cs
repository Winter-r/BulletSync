using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI healthText = default;
	[SerializeField] private TextMeshProUGUI staminaText = default;

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

	private void Start()
	{
		UpdateHealth(100);
		UpdateStamina(100);
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
