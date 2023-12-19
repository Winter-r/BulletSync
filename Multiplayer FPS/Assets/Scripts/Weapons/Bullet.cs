using UnityEngine;

public class Bullet : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			print("Hit Player");
			Destroy(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
