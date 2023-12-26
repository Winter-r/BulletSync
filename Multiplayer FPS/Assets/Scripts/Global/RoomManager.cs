using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public GameObject playerGameObject;
	public GameObject player;
	[Space]
	public Transform spawnPoint;
	
	private void Start()
	{
		Debug.Log("Connecting");
		
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		base.OnConnectedToMaster();
		
		Debug.Log("Connected to Server");
		
		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();
		
		PhotonNetwork.JoinOrCreateRoom("Prototype", null, null);
		
		Debug.Log("Joined Lobby");
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		
		Debug.Log("Joined Room");
		
		player = PhotonNetwork.Instantiate(playerGameObject.name, spawnPoint.position, Quaternion.identity);
	}
}
