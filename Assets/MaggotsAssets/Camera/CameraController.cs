using UnityEngine;

namespace Maggots
{
    public class CameraController : MonoBehaviour
    {
        private GameObject trackObject;

        private void LateUpdate()
        {
            if (trackObject != null)
            {
                transform.position = new Vector3(trackObject.transform.position.x, trackObject.transform.position.y, transform.position.z);
            }           
        }

        public void TrackNewObject(GameObject track)
        {
            trackObject = track;
        }
    }
}

