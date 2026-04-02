using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// DialogueController4 - renamed and fully using '4' suffix for fields/methods.

public class DialogueController4 : MonoBehaviour
{
    // Variables (controller 4)
    [SerializeField] private string[] DialogueLines4;
    [SerializeField] private GameObject DialogueTrigger4;
    [SerializeField] private TMP_Text Dialogue4;
    [SerializeField] private GameObject DialogueContainer4;
    [SerializeField] private float delayBetweenItems;
    [SerializeField] private bool useUnscaledTime = false;

    private bool isProcessing = false;

    void Start()
    {
        if (DialogueContainer4 != null) DialogueContainer4.SetActive(false);
        if (DialogueTrigger4 != null) DialogueTrigger4.SetActive(true);

        if (DialogueLines4 == null || DialogueLines4.Length == 0)
        {
            DialogueLines4 = new string[10];
            DialogueLines4[0] = "Welcome to Nubia village!";
            DialogueLines4[1] = "Woah! Is that a baby hippo you are carrying?";
            DialogueLines4[2] = "Poor thing seems to be injured.";
            DialogueLines4[3] = "I trust you want medicine for him";
            DialogueLines4[4] = "I can find you some, if you can help me with farming";
            DialogueLines4[5] = "Here, let me show you what needs to be done!";
            DialogueLines4[6] = "First, Irrigate the crops by feeding the cows.";
            DialogueLines4[7] = "Second, harvest the crops that were grown from the irrigation.";
            DialogueLines4[8] = "When you are finished, I will give you the medicine";
            DialogueLines4[9] = "Ah! You did a great job! Here is the medicine.";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProcessing && other.CompareTag("Player"))
        {
            if (gameObject == DialogueTrigger4)
            {
                isProcessing = true;
                if (DialogueContainer4 != null) DialogueContainer4.SetActive(true);
                StartCoroutine(ProcessDialogueOverTime4());
            }
        }
    }

    IEnumerator ProcessDialogueOverTime4()
    {
        if (DialogueLines4 == null) yield break;

        for (int i = 0; i < DialogueLines4.Length; i++)
        {
            Debug.Log($"[{Time.realtimeSinceStartup:F2}s realtime] (C4) {DialogueLines4[i]}");
            if (Dialogue4 != null) Dialogue4.text = DialogueLines4[i];

            float elapsed = 0f;
            float waitFor = Mathf.Max(0f, delayBetweenItems);
            while (elapsed < waitFor)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        if (DialogueTrigger4 != null) DialogueTrigger4.SetActive(false);
        if (DialogueContainer4 != null) DialogueContainer4.SetActive(false);
        isProcessing = false;
    }
}