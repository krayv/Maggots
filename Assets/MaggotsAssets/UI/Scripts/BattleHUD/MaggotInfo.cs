using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Maggots
{
    public class MaggotInfo : MonoBehaviour
    {
        [SerializeField] private Fillbar fillbar;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI nameText;
        public void UpdateInfo(Maggot maggot)
        {
            nameText.text = maggot.MaggotName;
            hpText.text = maggot.Health.ToString();
            hpText.color = maggot.Team.TeamColor;
            fillbar.SetValue(maggot.HealthPercent);
        }

        public void UpdateCharge(Maggot maggot, float progress)
        {
            if (progress <= 0f)
            {
                fillbar.gameObject.SetActive(false);
            }
            else
            {
                fillbar.gameObject.SetActive(true);
            }
            fillbar.SetValue(progress);
        }
    }
}
