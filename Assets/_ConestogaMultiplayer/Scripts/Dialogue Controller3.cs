using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// DialogueController3 - same format as DialogueController1/2, wired via Inspector for scene-specific use.
public class DialogueController3 : MonoBehaviour
{
    [SerializeField] private string[] DialogueLines3;
    [SerializeField] private GameObject DialogueTrigger3;
    [SerializeField] private TMP_Text Dialogue3;
    [SerializeField] private GameObject DialogueContainer3;
    [SerializeField] private float delayBetweenItems = 2f;
    [SerializeField] private bool useUnscaledTime = false; // false => Time.deltaTime, true => Time.unscaledDeltaTime

    private bool isProcessing = false;

    void Start()
    {
        if (DialogueContainer3 != null) DialogueContainer3.SetActive(false);
        if (DialogueTrigger3 != null) DialogueTrigger3.SetActive(true);

        // Default/sample lines only if nothing assigned in Inspector
        if (DialogueLines3 == null || DialogueLines3.Length == 0)
        {
            DialogueLines3 = new string[9];
            DialogueLines3[0] = "Oh thank you for taking care of them for me! That was really scary!";
            DialogueLines3[1] = "And you.. were scary too!";
            DialogueLines3[2] = "Goodness! How strong you are...!";
            DialogueLines3[3] = "Look! We have reached the Nubia village.";
            DialogueLines3[4] = "There is a farmer by the entrance who can help you get medicine for this hippo.";
            DialogueLines3[5] = "Go talk to him!";
            DialogueLines3[6] = "I have other errands to run so,";
            DialogueLines3[7] = "Come back once you have the medicine okay?";
            DialogueLines3[8] = "Go on now.";
        }
    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProcessing && other.CompareTag("Player"))
        {
            // Only start if this component instance is on the assigned trigger or trigger reference is unset
            if (gameObject == DialogueTrigger3 || DialogueTrigger3 == null)
            {
                isProcessing = true;
                if (DialogueContainer3 != null) DialogueContainer3.SetActive(true);
                StartCoroutine(ProcessDialogueOverTime3());
            }
        }
    }

    IEnumerator ProcessDialogueOverTime3()
    {
        if (DialogueLines3 == null) yield break;

        for (int i = 0; i < DialogueLines3.Length; i++)
        {
            Debug.Log($"[{Time.realtimeSinceStartup:F2}s realtime] (C3) {DialogueLines3[i]}");
            if (Dialogue3 != null) Dialogue3.text = DialogueLines3[i];

            float elapsed = 0f;
            float waitFor = Mathf.Max(0f, delayBetweenItems);
            while (elapsed < waitFor)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        if (DialogueTrigger3 != null) DialogueTrigger3.SetActive(false);
        if (DialogueContainer3 != null) DialogueContainer3.SetActive(false);
        isProcessing = false;
    }
}