using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

        private BezierCurve2D[] curves;

        private void Awake()
        {
            generateNextMapButton.onClick.AddListener(GenerateMap);
            startBattleButton.onClick.AddListener(StartBattle);
        }

        private void OnEnable()
        {         
            GenerateMap();
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
            ui.OpenPanel(UIPanelType.BattleHUD);
            SceneManager.LoadScene(battleStarter.ArenaScene, LoadSceneMode.Additive);
        }
    }

}
