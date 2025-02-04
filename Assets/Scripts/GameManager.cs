using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    [Header("Fruits Management")]
    public bool fruitsHaveRandomLook;
    public int fruitsCollected;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // If there is more than one GameManager it will delete one of them.
            Destroy(gameObject);
        }
    }
    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;

    public void RespawnPlayer() => StartCoroutine(RespawnCoroutine());

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Since respawnPoint is a "Transform" we write it as a "respawnPoint.position"
        // Alternative way: player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity).GetComponent<Player>();
        // Since the player is not an game object in here, we write ".GetComponent<Player>()" at the end 
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

        player = newPlayer.GetComponent<Player>();
    }

    public void AddFruit() => fruitsCollected++;

    public bool FruitsHaveRandomLook() => fruitsHaveRandomLook;

}
