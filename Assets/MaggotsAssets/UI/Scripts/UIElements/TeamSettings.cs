using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Maggots
{
    public class TeamSettings : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI teamTitle;
        [SerializeField] private Image flag;
        [SerializeField] private CountSettings charactersCountSetting;
        [SerializeField] private CountSettings hpPerCharacterSetting;

        private Color _teamColor;

        public Color TeamColor
        {
            get
            {
                return _teamColor;
            }
            set
            {
                team.TeamColor = value;
                flag.color = value;
            }
        }
        private Team team;

        public void Init(Team team)
        {
            this.team = team;
            TeamColor = Random.ColorHSV();
            team.CharacterCounts = charactersCountSetting.Value;
            team.HealthPerCharacter = hpPerCharacterSetting.Value;
        }

        private void Awake()
        {
            charactersCountSetting.OnChangeValue += OnChangeCharactersCount;
            hpPerCharacterSetting.OnChangeValue += OnChangeHealthCount;                   
        }

        private void OnDestroy()
        {
            charactersCountSetting.OnChangeValue -= OnChangeCharactersCount;
            hpPerCharacterSetting.OnChangeValue -= OnChangeHealthCount;
        }

        private void OnChangeCharactersCount(int value)
        {
            team.CharacterCounts = value;
        }

        private void OnChangeHealthCount(int value)
        {
            team.HealthPerCharacter = value;
        }
    }
}

