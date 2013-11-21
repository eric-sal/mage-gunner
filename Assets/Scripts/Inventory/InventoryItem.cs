using UnityEngine;
using System.Collections;

[System.Serializable]

/// <summary>
/// Represents an item in the character's inventory.
/// </summary>
public class InventoryItem {

    // Specifying the ItemType will help us orgranize inventory items
    // and apply general rules for interacting with them.
    // ex: cycling through firearms, equipping armor in armor slots, etc.
    public enum Classification { General, Firearm, Grenade, Armor };

    /* *** Member Variables *** */

    public string name; // The name of prefab. In the Inventory, this will also be the key that points to this InventoryItem.
    public string path; // The path to the prefab (excluding the name).
    public Classification classification;
    public int quantity;

    private GameObject _instance;

    /* *** Properties *** */

    public GameObject instance {
        get { return _instance; }
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Instantiate the actual GameObjects from the prefabs here.
    /// </summary>
    /// <param name='parent'>
    /// The GameObject for which this instance should be parented to.
    /// </param>
    public GameObject Instantiate(Transform parent) {
        GameObject itemPrefab = (GameObject)Resources.Load("Prefabs/" + this.path + "/" + this.name);
        _instance = (GameObject)Object.Instantiate(itemPrefab, parent.transform.position, itemPrefab.transform.rotation);
        _instance.name = this.name;
        _instance.transform.parent = parent;

        return _instance;
    }
}
