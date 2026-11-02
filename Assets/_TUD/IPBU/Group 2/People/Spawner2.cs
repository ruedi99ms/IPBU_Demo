using UnityEngine;
using System.Collections; // Needed for Coroutines

public class CharacterSpawner : MonoBehaviour
{
    [Header("Character Prefabs and Textures")]
    public GameObject[] characterPrefabs; // Array of 4 character prefabs
    public Material[] characterMaterials; // Array of 21 different textures/materials

    [Header("Spawn Settings")]
    public int numberOfCharacters = 10;   // Number of characters to spawn
    public Transform[] spawnPoints;       // Spawn points array
    public float spawnDelay = 0.5f;       // Time between spawns

    void Start()
    {
        StartCoroutine(SpawnCharactersStaggered());
    }

    IEnumerator SpawnCharactersStaggered()
    {
        for (int i = 0; i < numberOfCharacters; i++)
        {
            // Select a random prefab and spawn point
            GameObject selectedPrefab = characterPrefabs[Random.Range(0, characterPrefabs.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate the selected prefab at the spawn point
            GameObject spawnedCharacter = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);

            // Apply a random material to the spawned character
            ApplyRandomMaterial(spawnedCharacter);

            // Wait before spawning the next character
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void ApplyRandomMaterial(GameObject character)
    {
        // Get a random material from the array
        Material randomMaterial = characterMaterials[Random.Range(0, characterMaterials.Length)];

        // Find all renderers on the character and apply the material
        Renderer[] renderers = character.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = randomMaterial;
        }
    }
}
