using UnityEngine;
using System;

namespace Maggots
{
    public abstract class UIPanel: MonoBehaviour
    {
        public UIPanelType Type;

        protected UIController ui;

        public void Init(UIController ui)
        {
            this.ui = ui;
        }
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }
        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }

    public enum UIPanelType
    {
        MainMenu, MapPanel, BattleHUD, Inventory
    }
}
