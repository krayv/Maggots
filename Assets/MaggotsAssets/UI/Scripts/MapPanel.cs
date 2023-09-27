using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Maggots
{
    public class MapPanel : UIPanel
    {
        [SerializeField] private Button generateNextMapButton;
        [SerializeField] private Button startBattleButton;
        [SerializeField] private Vector2Int textureResolution;
        [SerializeField] private Image mapImage;
        [SerializeField] private BezierCurvesGenerator curvesGenerator;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private BattleStarter battleStarter;

        [SerializeField] private Button addTeamButton;
        [SerializeField] private Button removeTeamButton;

        [SerializeField] private TeamSettings teamSettingsPrefab;
        [SerializeField] private Transform teamSettingsContainer;
        [SerializeField] private int maxTeams = 4;

        private BezierCurve2D[] curves;

        private Dictionary<Team, TeamSettings> teams = new(); 

        private void Awake()
        {
            generateNextMapButton.onClick.AddListener(GenerateMap);
            startBattleButton.onClick.AddListener(StartBattle);
            addTeamButton.onClick.AddListener(AddTeam);
            removeTeamButton.onClick.AddListener(RemoveTeam);
        }      

        private void OnEnable()
        {         
            GenerateMap();
        }

        private void OnDisable()
        {
            foreach (var team in teams)
            {
                Destroy(team.Value.gameObject);       
            }
            teams.Clear();
        }

        public override void Open()
        {
            base.Open();
            for (int i = 0; i < 2; i++)
            {
                AddTeam(i);
            }
        }

        public void GenerateMap()
        {
            int seed = Random.Range(0, 10000000);
            curves = curvesGenerator.Generate(seed);
            battleStarter.mapCurves = curves;
            battleStarter.mapXBorders = curvesGenerator.XBorders;
            battleStarter.mapYBorders = curvesGenerator.YBorders;
            ShowMap();
        }

        private void AddTeam()
        {
            if(teams.Count < maxTeams)
                AddTeam(teams.Count);
        }

        private void AddTeam(int i)
        {
            TeamSettings teamSettings = Instantiate(teamSettingsPrefab, teamSettingsContainer);

            string teamName = "Player " + (i + 1).ToString();
            Dictionary<Weapon, int> startWeapons = new();
            foreach (var weapon in battleStarter.weapons)
            {
                if (weapon.Count > 0)
                {
                    if (startWeapons.ContainsKey(weapon.Weapon))
                    {
                        startWeapons[weapon.Weapon] += weapon.Count;
                    }
                    else
                    {
                        startWeapons.Add(weapon.Weapon, weapon.Count);
                    }

                }
            }
            Team team = new(teamName, startWeapons);

            teams[team] = teamSettings;
            teamSettings.Init(team);
        }

        private void RemoveTeam()
        {
            var team = teams.Last();
            Destroy(team.Value);
            teams.Remove(team.Key);
        }

        private void ShowMap()
        {
            Texture2D texture = new(textureResolution.x, textureResolution.y);
            texture.alphaIsTransparency = true;
            texture.filterMode = FilterMode.Point;

            foreach (BezierCurve2D curve in curves)
            {
                for (int i = 0; i < 1000; i++)
                {
                    Vector2 point = curve.GetPointUV((float)i / 1000f);
                    texture.SetPixel((int)(point.x * texture.width), (int)(point.y * texture.height), Color.black);
                }
            }
            texture.Apply();
            mapImage.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));            
        }

        private void StartBattle()
        {
            battleStarter.cameraController = Camera.main.GetComponent<CameraController>();
            battleStarter.Teams = teams.Select(t => t.Key).ToList();
            ui.OpenPanel(UIPanelType.BattleHUD);
            SceneManager.LoadScene(battleStarter.ArenaScene, LoadSceneMode.Additive);
        }
    }

}
