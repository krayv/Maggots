using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Maggots
{
    public class BattleHUD : UIPanel
    {
        [SerializeField] private ArenaData arenaData;
        [SerializeField] private Button leaveButton;        
        [SerializeField] private MaggotInfo infoPrefab;
        [SerializeField] private Vector2 infoOffset = Vector2.up;
        [SerializeField] private Transform MaggotsInfoLayer;

        private List<Team> Teams => arenaData.Teams;
        private readonly Dictionary<Maggot, MaggotInfo> infos = new();

        private Camera MainCamera => arenaData.CameraController.LocalCamera;

        private void Awake()
        {
            leaveButton.onClick.AddListener(LeaveBattle);
            arenaData.OnNewBattle += SpawnInfo;
        }

        private void LateUpdate()
        {
            UpdateInfo();
        }

        private void OnDestroy()
        {
            foreach (var info in infos)
            {
                info.Key.OnDestroyGO -= OnMaggotDestroy;
                info.Key.OnChangeLife -= info.Value.UpdateInfo;
                info.Key.OnChargeWeapon -= info.Value.UpdateCharge;
            }
            infos.Clear();
        }

        public void LeaveBattle()
        {
            arenaData.ArenaController.LeaveBattle();
            ui.OpenPanel(UIPanelType.MainMenu);
        }

        private void SpawnInfo()
        {
            foreach (var info in infos)
            {
                DeleteInfo(info.Key);
            }

            foreach (Team team in Teams)
            {
                foreach (Maggot maggot in team.Maggots)
                {
                    MaggotInfo info = Instantiate(infoPrefab, MaggotsInfoLayer);
                    info.UpdateInfo(maggot);
                    maggot.OnDestroyGO += OnMaggotDestroy;
                    maggot.OnChangeLife += info.UpdateInfo;
                    maggot.OnChargeWeapon += info.UpdateCharge;
                    infos[maggot] = info;
                }
            }
        }

        private void DeleteInfo(Maggot maggot)
        {
            if (infos[maggot].gameObject != null)
            {
                Destroy(infos[maggot].gameObject);
            }               
            infos.Remove(maggot);          
        }

        private void OnMaggotDestroy(Maggot maggot)
        {
            maggot.OnDestroyGO -= OnMaggotDestroy;
            DeleteInfo(maggot);
        }

        private void UpdateInfo()
        {
            SetUIPosition();
        }     

        private void SetUIPosition()
        {
            foreach (var info in infos)
            {
                MaggotInfo infoGO = info.Value;
                Maggot maggot = info.Key;
                Vector2 scale = Vector2.one / MainCamera.orthographicSize * 5;
                infoGO.transform.localScale = scale;
                infoGO.transform.position = MainCamera.WorldToScreenPoint(maggot.transform.position + (Vector3)infoOffset);
            }
        }      
    }
}

