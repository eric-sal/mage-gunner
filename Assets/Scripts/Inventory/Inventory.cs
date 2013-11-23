using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class represents a character's inventory.
/// </summary>
public class Inventory : MonoBehaviour {

    /* *** Member Variables *** */

    public int maxItems;    // The max number of items this inventory can hold.

    private BaseCharacterController _controller;
    private int _currentFirearmIndex = 0;
    private List<InventoryItem> _firearms;  // All inventory items classified as Firearms

    // The _inventory variable is a Dictionary where the keys are the names of prefabs in the inventory.
    // Dictionary types aren't serialized by Unity, so we extract them into two separate lists - one
    // for the keys and one for the values.
    public List<string> keys;
    public List<InventoryItem> values;
    private Dictionary<string,InventoryItem> _inventory;

    /* *** Constructors *** */

    void Awake() {
        if (keys.Count != values.Count) {
            throw new InvalidOperationException("The number of elements in the list of keys must match the number of elements in the list of values.");
        }

        _controller = this.transform.parent.GetComponent<BaseCharacterController>();
    }

    /* *** MonoBehaviour Methods *** */

    /// <summary>
    /// Stitch the inventory Dictionary together.
    /// </summary>
    void OnEnable() {
        if (_inventory == null) {
            _inventory = new Dictionary<string, InventoryItem>();
            _firearms = new List<InventoryItem>();

            string key;
            InventoryItem inventoryItem;
            for (int i = 0; i < keys.Count; i++) {
                key = keys[i];
                inventoryItem = values[i];
                inventoryItem.Instantiate(transform);
                _inventory[key] = inventoryItem;

                if (inventoryItem.classification == InventoryItem.Classification.Firearm) {
                    _firearms.Add(inventoryItem);
                }
            }

            if (_firearms.Count > 0) {
                SetWeapon(_currentFirearmIndex);
            }

            keys.Clear();
            values.Clear();
        }
    }

    /// <summary>
    /// Extract the inventory Dictionary to the lists.
    /// </summary>
    void OnDisable() {
        if (!Application.isPlaying) {
            foreach (var item in _inventory) {
                keys.Add(item.Key);
                values.Add(item.Value);
            }

            _inventory.Clear();
        }
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Cycle to the next weapon.
    /// </summary>
    public void NextWeapon() {
        _currentFirearmIndex += 1;
        if (_currentFirearmIndex >= _firearms.Count) {
            _currentFirearmIndex = 0;
        }

        SetWeapon(_currentFirearmIndex);
    }

    /// <summary>
    /// Cycle to the previous weapon.
    /// </summary>
    public void PreviousWeapon() {
        _currentFirearmIndex -= 1;
        if (_currentFirearmIndex < 0) {
            _currentFirearmIndex = _firearms.Count - 1;
        }

        SetWeapon(_currentFirearmIndex);
    }

    /// <summary>
    /// Cycle to the weapon at the specified index.
    /// </summary>
    /// <param name='index'>
    /// The index of the weapon to use.
    /// </param>
    public void SetWeapon(int index) {
        _controller.equippedFirearm = _firearms[index].instance.GetComponent<Firearm>();
    }
}
