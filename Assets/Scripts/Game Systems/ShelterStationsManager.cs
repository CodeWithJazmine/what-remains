using System.Collections.Generic;
using UnityEngine;


public class ShelterStationsManager : MonoBehaviour
{
    [SerializeField] private UnlockedShelterStation unlockedStations;
    [SerializeField] private List<ShelterStation> shelterStations;

    void Start()
    {
        RefreshStations();
    }

    public void RefreshStations()
    {
        foreach (var station in shelterStations)
        {
            StationUnlockStatus status = unlockedStations.unlockedStations.Find(s => s.stationType == station.stationType);
            bool isUnlocked = status != null && status.isUnlocked;
            station.gameObject.SetActive(isUnlocked);
            station.stationFree = true;
        }
    }
}
