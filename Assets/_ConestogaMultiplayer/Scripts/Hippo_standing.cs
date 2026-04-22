using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    public Animator targetAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hippo"))
        {
            targetAnimator.SetTrigger("Open");
        }
    }
}