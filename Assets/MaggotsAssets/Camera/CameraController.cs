using UnityEngine;

namespace Maggots
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private GameObject trackObject;

        [SerializeField] private Vector2 maxSizeRange = new(4f, 25f);

        private Camera localCamera;

        private void Awake()
        {
            localCamera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (trackObject != null)
            {
                transform.position = new Vector3(trackObject.transform.position.x, trackObject.transform.position.y, transform.position.z);
            }
            
            if (Input.mouseScrollDelta.y != 0f)
            {
                localCamera.orthographicSize = Mathf.Clamp((localCamera.orthographicSize + Input.mouseScrollDelta.y), maxSizeRange.x, maxSizeRange.y);
            }
        }

        public void TrackNewObject(GameObject track)
        {
            trackObject = track;
        }
    }
}

