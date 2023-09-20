using System;
using UnityEngine;

namespace Maggots
{
    [Serializable]
    public class InputEvent
    {
        [SerializeField]
        private InputEventType inputType;
        [SerializeField]
        private InputKeyPressType keyPressType;
        [SerializeField]
        private KeyCode[] relatedKeys;

        public InputEventType InputType { get => inputType; }

        private delegate bool CheckInput(KeyCode key);

        private CheckInput checkInput;

        public void Init()
        {
            checkInput = keyPressType switch
            {
                InputKeyPressType.Default => delegate (KeyCode key) { return Input.GetKey(key); },
                InputKeyPressType.KeyUp => delegate (KeyCode key) { return Input.GetKeyUp(key); },
                InputKeyPressType.KeyDown => delegate (KeyCode key) { return Input.GetKeyDown(key); },
                _ => delegate (KeyCode key) { return Input.GetKey(key); }
            };
        }

        public bool UpdateInput()
        {
            if (relatedKeys.Length == 0)
            {
                return false;
            }
            
            for (int i = 0; i < relatedKeys.Length; i++)
            {
                if (checkInput(relatedKeys[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public enum InputEventType
    {
        Left, Right, Up, Down, Jump, FireStart, FireRelease
    }

    public enum InputKeyPressType 
    {
        Default, KeyUp, KeyDown
    }
}
