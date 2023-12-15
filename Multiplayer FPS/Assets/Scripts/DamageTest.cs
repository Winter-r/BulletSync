using UnityEngine;

public class DamageTest : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            FPSController.OnTakeDamage(15);
    }
}