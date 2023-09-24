using System.Collections.Generic;
using System.Linq;
using System;

namespace Maggots
{
    public class Inventory
    {
        private readonly Dictionary<Weapon, int> weapons = new();

        private Weapon _currentSelection;

        public Action<Weapon> OnChangeWeapon;

        public Weapon SelectedWeapon
        {
            get
            {
                return _currentSelection;
            }
            set
            {
                if (weapons.ContainsKey(value))
                {
                    OnChangeWeapon.Invoke(value);
                    _currentSelection = value;
                }             
            }
        }

        public List<Weapon> AvaibleWeapons
        {
            get
            {
                List<Weapon> value = new(weapons.Count);
                foreach (var weapon in weapons)
                {
                    if (weapon.Value > 0)
                    {
                        value.Add(weapon.Key);
                    }                  
                }
                return value;
            }
        }
        public int Count => weapons.Count;

        public Inventory(Dictionary<Weapon, int> weapons)
        {
            this.weapons = weapons;
            _currentSelection = null;
        }

        public int GetCount(Weapon weapon)
        {
            return weapons[weapon];
        }

        public void AddWeapon(Weapon weapon, int count)
        {
            weapons[weapon] += count;
        }

        public void RemoveWeapon(Weapon weapon, int count)
        {
            weapons[weapon] -= count;
            if (weapons[weapon] <= 0)
            {
                if (weapon == SelectedWeapon)
                {
                    _currentSelection = null;
                }
                weapons.Remove(weapon);
            }           
        }
    }
}
