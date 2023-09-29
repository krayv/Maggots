using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maggots
{
    public class Background : MonoBehaviour
    {
        private Camera TrackCamera;
        private void Start()
        {
            TrackCamera = Camera.main;
        }

        private void Update()
        {
            transform.position = new Vector3(TrackCamera.transform.position.x, TrackCamera.transform.position.y, transform.position.z);
        }
    }
}

