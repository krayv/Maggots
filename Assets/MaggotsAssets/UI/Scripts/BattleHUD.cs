using UnityEngine;
using UnityEngine.UI;

namespace Maggots
{
    public class BattleHUD : UIPanel
    {
        [SerializeField] private ArenaData arenaData;
        [SerializeField] private Button leaveButton;
        private void Awake()
        {
            leaveButton.onClick.AddListener(LeaveBattle);
        }
        public void LeaveBattle()
        {
            arenaData.arenaController.LeaveBattle();
            ui.OpenPanel(UIPanelType.MainMenu);
        }
    }
}

