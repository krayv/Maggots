using UnityEngine;
using UnityEngine.UI;

namespace Maggots
{
    public class Fillbar : MonoBehaviour
    {
        [SerializeField] Image fillbar;

        public void SetValue(float value)
        {
            fillbar.fillAmount = value;
        }
    }
}

