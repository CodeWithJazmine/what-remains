using UnityEngine;

[CreateAssetMenu(fileName = "SurvivorData", menuName = "Scriptable Objects/SurvivorData")]
public class SurvivorData : ScriptableObject
{
    public string survivorName;
    public Texture2D survivorIcon;
}
