using UnityEngine;

namespace Maggots
{
    [CreateAssetMenu(fileName = "ArenaData", menuName = "MaggotsAssets/ArenaData", order = 2)]
    public class ArenaData : ScriptableObject
    {
        public ArenaController arenaController;
    }
}

