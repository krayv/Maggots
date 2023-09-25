using UnityEngine;

namespace Maggots
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private GameObject trackObject;

        [SerializeField] private Vector2 maxSizeRange = new(4f, 25f);
        [SerializeField] private float animationTime;
        [SerializeField] AnimationCurve curve;

        public Camera LocalCamera;

        private float currentTime = 0f;
        private Vector2 startPosition;

        private Vector3 lastTrackObjectPos;

        private void Awake()
        {
            LocalCamera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (trackObject != null)
            {
                lastTrackObjectPos = trackObject.transform.position;
            }

            Vector3 targetPos = new(lastTrackObjectPos.x, lastTrackObjectPos.y, transform.position.z);
            if (currentTime <= animationTime)
            {
                Vector3 newPos = Vector3.Lerp(startPosition, targetPos, curve.Evaluate(currentTime / animationTime));
                transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
                currentTime += Time.deltaTime;
            }
            else
            {
                transform.position = targetPos;
            }

            if (Input.mouseScrollDelta.y != 0f)
            {
                LocalCamera.orthographicSize = Mathf.Clamp((LocalCamera.orthographicSize + Input.mouseScrollDelta.y), maxSizeRange.x, maxSizeRange.y);
            }
        }

        public void TrackNewObject(GameObject track)
        {
            currentTime = 0f;
            startPosition = transform.position;
            trackObject = track;
        }
    }
}

