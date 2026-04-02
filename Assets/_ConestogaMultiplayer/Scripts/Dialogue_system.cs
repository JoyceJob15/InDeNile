using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public UnityEvent onDialogueFinished;

    public void EndDialogue()
    {
        Debug.Log("Dialogue finished");

        // Fire event
        onDialogueFinished.Invoke();
    }
}
