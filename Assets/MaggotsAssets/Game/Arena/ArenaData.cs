using UnityEngine;
using System.Collections.Generic;
using System;

namespace Maggots
{
    [CreateAssetMenu(fileName = "ArenaData", menuName = "MaggotsAssets/ArenaData", order = 2)]
    public class ArenaData : ScriptableObject
    {
        public List<Team> Teams;
        public ArenaController ArenaController;
        public CameraController CameraController;

        public Action OnNewBattle;
    }
}

