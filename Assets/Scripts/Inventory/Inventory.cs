using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
    public int maxItems;

    // These are public because that's how they get serialized.
    public List<string> keys;
    public List<InventoryItem> values;

    private PlayerCharacterController _controller;
    private Dictionary<string,InventoryItem> _inventory;

    // probably not the best place for this, but this is only a proof of concept dealy
    private int _currentWeaponIndex = 0;

    public void Awake() {
        _controller = this.transform.parent.GetComponentInChildren<PlayerCharacterController>();
    }

    // Stitch the dictionary together
    public void OnEnable() {
        if (_inventory == null) {
            _inventory = new Dictionary<string, InventoryItem>();

            string key;
            InventoryItem inventoryItem;
            for (int i = 0; i < keys.Count; i++) {
                key = keys[i];
                inventoryItem = values[i];

                inventoryItem.Instantiate(transform);

                _inventory[key] = inventoryItem;
            }

            // Hacky-hacky!
            _controller.equippedFirearm = values[0].instance.GetComponent<Firearm>();

            keys.Clear();
            values.Clear();
        }
    }

    // Extract dictionary to the lists
    public void OnDisable() {
        if (!Application.isPlaying) {
            foreach (var item in _inventory) {
                keys.Add(item.Key);
                values.Add(item.Value);
            }
    
            _inventory.Clear();
        }
    }

    public void NextWeapon() {
        _currentWeaponIndex += 1;
        if (_currentWeaponIndex >= _inventory.Values.Count) {
            _currentWeaponIndex = 0;
        }

        SetWeapon(_currentWeaponIndex);
    }

    public void PreviousWeapon() {
        _currentWeaponIndex -= 1;
        if (_currentWeaponIndex < 0) {
            _currentWeaponIndex = _inventory.Values.Count - 1;
        }

        SetWeapon(_currentWeaponIndex);
    }

    public void SetWeapon(int index) {
        List<InventoryItem> weapons = new List<InventoryItem>(_inventory.Values);
        _controller.equippedFirearm = weapons[index].instance.GetComponent<Firearm>();
    }
}
