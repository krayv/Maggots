using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Maggots
{
    public class InputSystem : MonoBehaviour
    {
        [SerializeField] private InputsConfiguration config;

        public AxisUnityEvent AxisEvent = new();
        public AxisUnityEvent HorizontalAxisEvent = new();
        public AxisUnityEvent VerticalAxisEvent = new();

        public UnityEvent JumpEvent;
        public UnityEvent FireStartEvent;
        public UnityEvent FireReleaseEvent;
        public UnityEvent InventoryEvent;

        private readonly Dictionary<InputEventType, InputEvent> inputEvents = new();
        public void Init()
        {
            foreach (InputEvent inputEvent in config.inputEvents)
            {
                inputEvent.Init();
                inputEvents[inputEvent.InputType] = inputEvent;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            CheckAxis();

            if (inputEvents[InputEventType.Jump].UpdateInput())
            {
                JumpEvent.Invoke();
            }

            if (inputEvents[InputEventType.FireStart].UpdateInput())
            {
                FireStartEvent.Invoke();
            }

            if (inputEvents[InputEventType.FireRelease].UpdateInput())
            {
                FireReleaseEvent.Invoke();
            }

            if (inputEvents[InputEventType.Inventory].UpdateInput())
            {
                InventoryEvent.Invoke();
            }
        }

        private void CheckAxis()
        {
            bool right = inputEvents[InputEventType.Right].UpdateInput();
            bool left = inputEvents[InputEventType.Left].UpdateInput();
            bool up = inputEvents[InputEventType.Up].UpdateInput();
            bool down = inputEvents[InputEventType.Down].UpdateInput();

            float x = right.ToFloat() - left.ToFloat();
            float y = up.ToFloat() - down.ToFloat();

            if(x != 0)
            {
                HorizontalAxisEvent.Invoke(new AxisInputEventArgs(x, 0));
                AxisEvent.Invoke(new AxisInputEventArgs(x, y));
            }
            else if (y != 0)
            {
                VerticalAxisEvent.Invoke(new AxisInputEventArgs(0, y));
                AxisEvent.Invoke(new AxisInputEventArgs(x, y));
            }
        }

        public class AxisUnityEvent : UnityEvent<AxisInputEventArgs>
        {

        }

    } 

    public class AxisInputEventArgs
    {
        private Vector2 _value;
        public Vector2 Value => _value;

        public AxisInputEventArgs(float xValue, float yValue)
        {
            _value = new Vector2(xValue, yValue).normalized;
        }
    }
}

