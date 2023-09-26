using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace Maggots
{
    public class CountSettings : MonoBehaviour
    {
        [SerializeField] private Button leftArrow;
        [SerializeField] private Button rightArrow;
        [SerializeField] private int step;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] Vector2Int valueRange;
        [SerializeField] private int startValue;

        public Action<int> OnChangeValue;
        public int Value
        {
            get
            {
                return currentValue;
            }
        }
        private int currentValue;


        private void Awake()
        {
            leftArrow.onClick.AddListener(DecreaseValue);
            rightArrow.onClick.AddListener(IncreaseValue);
            currentValue = startValue;
            UpdateText();
        }

        private void DecreaseValue()
        {
            if (currentValue == valueRange.x)
            {
                return;
            }
            currentValue -= step;
            if (currentValue < valueRange.x)
            {
                currentValue = valueRange.x;
            }
            OnChangeValue.Invoke(currentValue);
            UpdateText();
        }

        private void IncreaseValue()
        {
            if (currentValue == valueRange.y)
            {
                return;
            }
            currentValue += step;
            if (currentValue > valueRange.y)
            {
                currentValue = valueRange.y;
            }
            OnChangeValue.Invoke(currentValue);
            UpdateText();
        }

        private void UpdateText()
        {
            countText.text = currentValue.ToString();
        }

    }
}

