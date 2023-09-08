using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Maggots
{
    public class MainMenu : UIPanel
    {

        [SerializeField] private Button startButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        private void OnStartButtonClick()
        {
            ui.OpenPanel(UIPanelType.MapPanel);
        }

        private void OnExitButtonClick()
        {

        }
    }
}

