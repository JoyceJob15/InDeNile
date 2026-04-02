using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerTimeline : MonoBehaviour
{
    public PlayableDirector director;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            director.Play();
        }
    }
}
