using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Maggots
{
    public class InventoryUI : UIPanel
    {
        [SerializeField] private WeaponIcon weaponIconPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private int itemsCount = 30;
        [SerializeField] private ArenaData arenaData;

        private readonly List<WeaponIcon> icons = new();
        private Inventory inventory;

        public void Awake()
        {
            for (int i = 0; i < itemsCount; i++)
            {
                WeaponIcon icon = Instantiate(weaponIconPrefab, container);
                icons.Add(icon);
                icon.Init(OnSelect);
            }
        }

        public override void Open()
        {
            base.Open();
            OpenInventory(arenaData.CurrentTeam.Inventory);
        }

        public void OpenInventory(Inventory inventory)
        {
            int i = 0;
            foreach (var weapon in inventory.AvaibleWeapons)
            {               
                icons[i].gameObject.SetActive(true);
                icons[i].Set(weapon, inventory.GetCount(weapon));
                i++;
            }
            while (i < icons.Count)
            {
                icons[i].gameObject.SetActive(false);
                i++;
            }
            this.inventory = inventory;
        }

        private void OnEnable()
        {
            arenaData.InputSystem.InventoryEvent.AddListener(Close);
        }

        private void OnDisable()
        {
            arenaData.InputSystem.InventoryEvent.RemoveListener(Close);
        }

        private void OnSelect(Weapon weapon)
        {
            inventory.SelectedWeapon = weapon;
            Close();
        }
    }
}

