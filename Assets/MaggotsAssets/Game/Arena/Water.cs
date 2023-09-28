using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maggots
{
    public class Water : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<IDrowable>(out IDrowable drowable))
            {
                drowable.Drow();
            }
        }
    }

}
