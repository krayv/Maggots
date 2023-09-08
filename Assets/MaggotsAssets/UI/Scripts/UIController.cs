using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Maggots
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private List<UIPanel> panels;

        private readonly List<UIPanel> activePanels = new();

        public void Init()
        {
            DontDestroyOnLoad(gameObject);
            panels.ForEach(p => p.Init(this));
            
        }

        private void Start()
        {
            OpenPanel(UIPanelType.MainMenu);
        }

        public void OpenPanel(UIPanelType type, bool closeActive = true)
        {
            if (closeActive)
            {
                var closedPanels = activePanels.Where(p => p.Type != type).ToList();
                closedPanels.ForEach(p => { 
                    p.Close(); 
                    activePanels.Remove(p);
                });
            }
            
            UIPanel panel = panels.FirstOrDefault(p => p.Type == type);
            panel.Open();
            activePanels.Add(panel);
        }

        public void ClosePanel(UIPanelType type)
        {
            var closedPanels = activePanels.Where(p => p.Type == type).ToList();
            closedPanels.ForEach(p => {
                p.Close();
                activePanels.Remove(p);
            });
        }
    }
}

