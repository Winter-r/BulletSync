using UnityEngine;

public class Bullet : MonoBehaviour
{
	private void OnCollisionEnter(Collision objectHit)
	{
		if (objectHit.gameObject.CompareTag("Player"))
		{
			print("Hit Player");
			CreateBulletImpactEffect(objectHit);
			Destroy(gameObject);
		}
		else if (objectHit.gameObject.CompareTag("Bottle"))
		{
			print("Hit Bottle");
			objectHit.gameObject.GetComponent<Bottle>().Shatter();
		}
		else if (
			objectHit.gameObject.CompareTag("Footsteps/TILES")
			|| objectHit.gameObject.CompareTag("Footsteps/WOOD")
			|| objectHit.gameObject.CompareTag("Footsteps/METAL")
			|| objectHit.gameObject.CompareTag("Footsteps/GRASS")
		)
		{
			CreateBulletImpactEffect(objectHit);
			Destroy(gameObject);
		}
	}

	private void CreateBulletImpactEffect(Collision objectHit)
	{
		ContactPoint contact = objectHit.contacts[0];

		GameObject hole = Instantiate(
			GlobalReferences.Instance.bulletImpactEffectPrefab,
			contact.point,
			Quaternion.LookRotation(contact.normal)
		);

		hole.transform.SetParent(objectHit.gameObject.transform);
	}
}
