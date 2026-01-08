using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<GameObject> items = new();

    public void AddItem(GameObject item) => items.Add(item);
    public bool RemoveItem(GameObject item) => items.Remove(item);
    public bool HasItem(GameObject item) => items.Contains(item);
    public List<GameObject> GetAllItems() => items;
    public void Clear() => items.Clear();

}
