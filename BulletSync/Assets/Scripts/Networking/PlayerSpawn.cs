using UnityEngine;
using Photon.Pun;
using BulletSync.Character;

public class PlayerSpawn : MonoBehaviour
{
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private GameObject spawnPoint;

	private void Awake()
	{
		GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.transform.position, Quaternion.identity);
		player.GetComponent<PlayerManager>().IsLocalPlayer();
	}
}
