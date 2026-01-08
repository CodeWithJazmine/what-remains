using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockedShelterStation", menuName = "Scriptable Objects/UnlockedShelterStation")]
public class UnlockedShelterStation : ScriptableObject
{
    public List<StationUnlockStatus> unlockedStations = new();
}
