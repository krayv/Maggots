using UnityEngine;

namespace Maggots
{
    [CreateAssetMenu(order = 0)]
    public class InputsConfiguration : ScriptableObject
    {
        public InputEvent[] inputEvents;
    }
}

