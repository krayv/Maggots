using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace Maggots
{
    [CreateAssetMenu(fileName = "BattleStartConfig", menuName = "MaggotsAssets/BattleStartConfig", order = 1)]
    public class BattleStarter : ScriptableObject
    {
        [SerializeField] private string arenaScene;

        public string ArenaScene { get { return arenaScene; } }
        public Vector2 mapXBorders;
        public Vector2 mapYBorders;
        public CameraController cameraController;
        public BezierCurve2D[] mapCurves;
        public List<InitWeapon> weapons;

        [Serializable]
        public struct InitWeapon
        {
            public Weapon Weapon;
            public int Count;
        }
    }

    
}
