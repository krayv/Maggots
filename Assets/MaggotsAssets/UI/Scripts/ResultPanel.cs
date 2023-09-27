using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

namespace Maggots
{
    public class ResultPanel : UIPanel
    {
        [SerializeField] ArenaData arenaData;
        [SerializeField] private TextMeshProUGUI winnerText;
        [SerializeField] Button mainMenuButton;

        private void Awake()
        {
            mainMenuButton.onClick.AddListener(OpenMainMenu);
        }

        public override void Open()
        {
            base.Open();
            winnerText.text  = arenaData.Teams.First(t => !t.TeamLost).TeamName;
        }

        private void OpenMainMenu()
        {
            arenaData.ArenaController.LeaveBattle();
            ui.OpenPanel(UIPanelType.MainMenu);
        }
    }
}
