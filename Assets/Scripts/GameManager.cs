using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    [Header("Fruits Management")]
    public bool fruitsAreRandom;
    public int fruitsCollected;
    public int totalFruits;

    [Header("Checkpoints")]
    public bool canReactivate;

    [Header("Traps")]
    public GameObject arrowPrefab;

    [SerializeField] private GameObject fruitPrefab;
    private List<Vector3> fruitOriginalPositions = new List<Vector3>();
    private List<Quaternion> fruitOriginalRotations = new List<Quaternion>();

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
    private void Start()
    {
        CollectFruitsInfo();
    }

    private void CollectFruitsInfo()
    {
        Fruit[] allFruits = Object.FindObjectsByType<Fruit>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        // Old Version: Fruit[] allFruits = FindObjectsOfType<Fruit>(); 
        totalFruits = allFruits.Length;

        fruitOriginalPositions.Clear();
        fruitOriginalRotations.Clear();

        foreach (Fruit fruit in allFruits)
        {
            fruitOriginalPositions.Add(fruit.transform.position);
            fruitOriginalRotations.Add(fruit.transform.rotation);
        }
    }

    private void RespawnFruits()
    {
        Fruit[] allFruits = Object.FindObjectsByType<Fruit>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Fruit fruit in allFruits)
        {
            Destroy(fruit.gameObject);
        }

        fruitsCollected = 0;

        for (int i = 0; i < fruitOriginalPositions.Count; i++)
        {
            Instantiate(fruitPrefab, fruitOriginalPositions[i], fruitOriginalRotations[i]);
        }
    }


    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;

    public void RespawnPlayer() => StartCoroutine(RespawnCoroutine());

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        RespawnFruits();

        // Since respawnPoint is a "Transform" we write it as a "respawnPoint.position"
        // Alternative way: player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity).GetComponent<Player>();
        // Since the player is not an game object in here, we write ".GetComponent<Player>()" at the end 
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

        player = newPlayer.GetComponent<Player>();
    }

    public void AddFruit() => fruitsCollected++;

    public bool FruitsHaveRandomLook() => fruitsAreRandom;

    public void CreateObject(GameObject prefab, Transform target, float delay = 0)
    {
        StartCoroutine(CreateObjectCoroutine(prefab, target, delay));
    }

    private IEnumerator CreateObjectCoroutine(GameObject prefab, Transform target, float delay)
    {
        Vector3 newPosition = target.position;

        yield return new WaitForSeconds(delay);

        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);
    }

    private void LoadTheEndScene()
    {
        SceneManager.LoadScene("TheEnd");
    }

    public void LevelFinished()
    {
        UI_InGame.instance.fadeEffect.ScreenFade(1, 1.5f, LoadTheEndScene);
    }
}
