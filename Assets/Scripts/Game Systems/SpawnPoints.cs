using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;

    void Awake()
    {
        GameManager.instance.RegisterSpawnPoints(spawnPoints);
    }
}
