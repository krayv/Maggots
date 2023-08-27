using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maggots
{
    public class WeaponSprite : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(projectileStartPoint.transform.position, 0.05f);
        }

        public Transform projectileStartPoint;
    }

}
