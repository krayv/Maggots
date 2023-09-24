using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace Maggots
{
    public class WeaponIcon : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI countText;       

        private Weapon weapon;
        private Action<Weapon> onSelect;

        public void Init(Action<Weapon> onSelect)
        {
            this.onSelect = onSelect;
        }

        public void Set(Weapon weapon, int count)
        {
            this.weapon = weapon;
            icon.sprite = weapon.icon;
            countText.text = count.ToString();
        }

        public void Select()
        {
            onSelect.Invoke(weapon);
        }
    }
}

