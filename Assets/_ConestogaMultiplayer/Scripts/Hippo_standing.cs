using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    public GameObject hippo2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hippo"))
        {
            other.gameObject.SetActive(false);
            hippo2.SetActive(true);
        }
    }
}