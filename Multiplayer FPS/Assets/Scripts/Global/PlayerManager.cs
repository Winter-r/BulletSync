using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static event System.Action<GameObject> OnPlayerInstantiated;

    private GameObject playerPrefab;
    private GameObject playerInstance;

    public void InstantiatePlayer(Transform spawnPoint)
    {
        if (playerPrefab != null)
        {
            playerInstance = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            OnPlayerInstantiated?.Invoke(playerInstance);
        }
        else
        {
            Debug.LogError("Player prefab is not set in PlayerManager.");
        }
    }

    // You can add other player-related functions here

    private void Awake()
    {
        // Set playerPrefab in the Unity Editor or through code
        playerPrefab = Resources.Load<GameObject>("PlayerPrefab"); // Replace with your player prefab name
    }
}