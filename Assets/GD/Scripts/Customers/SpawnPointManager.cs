using UnityEngine;
using System.Collections.Generic;

public class SpawnPointManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private bool[] occupiedSpawnPoints;

    public int SpawnPointCount => spawnPoints.Length;

    private void Awake()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Aucun SpawnPoint assigné !");
            return;
        }

        occupiedSpawnPoints = new bool[spawnPoints.Length];
    }

    public int GetFreeSpawnPoint()//quel SP est libre ?
    {
        if (occupiedSpawnPoints == null)
            return -1;

        List<int> freePoints = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!occupiedSpawnPoints[i])
                freePoints.Add(i);
        }

        if (freePoints.Count == 0)
            return -1;

        return freePoints[Random.Range(0, freePoints.Count)];// alea sp choisit
    }

    public Transform GetSpawnPoint(int index)
    {
        if (index < 0 || index >= spawnPoints.Length)
            return null;

        return spawnPoints[index];
    }

    public bool OccupySpawnPoint(int index)//en gros on réserve une place sans connaitre fonctionnement gestionnaire de slot 
    {
        if (occupiedSpawnPoints == null)
            return false;

        if (index < 0 || index >= occupiedSpawnPoints.Length)
            return false;

        if (occupiedSpawnPoints[index])
            return false;

        occupiedSpawnPoints[index] = true;
        return true;
    }

    public void ReleaseSpawnPoint(int index)// sp libéré quand un client part 
    {
        if (occupiedSpawnPoints == null)
            return;

        if (index >= 0 && index < occupiedSpawnPoints.Length)
        {
            occupiedSpawnPoints[index] = false;
        }
    }
}