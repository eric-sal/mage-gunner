using UnityEngine;
using System.Collections;

[System.Serializable]

public class InventoryItem {
    public string name;
    public string path;
    public int quantity;

    private GameObject _instance;

    public GameObject instance {
        get { return _instance; }
    }

    // We need to instantiate the actual GameObjects from the prefabs here.
    public GameObject Instantiate(Transform parent) {
        GameObject itemPrefab = (GameObject)Resources.Load("Prefabs/" + path + "/" + name);
        _instance = (GameObject)Object.Instantiate(itemPrefab, parent.transform.position, itemPrefab.transform.rotation);
        _instance.name = name;
        _instance.transform.parent = parent;

        return _instance;
    }
}
