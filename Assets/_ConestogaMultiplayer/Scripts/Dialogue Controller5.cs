using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// DialogueController5 - same format as DialogueController1/2, wired via Inspector for scene-specific use.
public class DialogueController5 : MonoBehaviour
{
    [SerializeField] private string[] DialogueLines5;
    [SerializeField] private GameObject DialogueTrigger5;
    [SerializeField] private TMP_Text Dialogue5;
    [SerializeField] private GameObject DialogueContainer5;
    [SerializeField] private float delayBetweenItems = 2f;
    [SerializeField] private bool useUnscaledTime = false; // false => Time.deltaTime, true => Time.unscaledDeltaTime

    private bool isProcessing = false;

    void Start()
    {
        if (DialogueContainer5 != null) DialogueContainer5.SetActive(false);
        if (DialogueTrigger5 != null) DialogueTrigger5.SetActive(true);

        // Default/sample lines only if nothing assigned in Inspector
        if (DialogueLines5 == null || DialogueLines5.Length == 0)
        {
            DialogueLines5 = new string[5];
            DialogueLines5[0] = "Hooray! you got the medicine. Quick! feed it to the hippo!";
            DialogueLines5[1] = "I see the hippo's family up ahead";
            DialogueLines5[2] = "Hold the hippo in your hands and let it go into the water";
            DialogueLines5[3] = "I tam glad we were able to reunite the little guy with his family.";
            DialogueLines5[4] = "Thank you for accompanying me on this journey!";
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProcessing && other.CompareTag("Player"))
        {
            // Only start if this component instance is on the assigned trigger or trigger reference is unset
            if (gameObject == DialogueTrigger5 || DialogueTrigger5 == null)
            {
                isProcessing = true;
                if (DialogueContainer5 != null) DialogueContainer5.SetActive(true);
                StartCoroutine(ProcessDialogueOverTime5());
            }
        }
    }

    IEnumerator ProcessDialogueOverTime5()
    {
        if (DialogueLines5 == null) yield break;

        for (int i = 0; i < DialogueLines5.Length; i++)
        {
            Debug.Log($"[{Time.realtimeSinceStartup:F2}s realtime] (C5) {DialogueLines5[i]}");
            if (Dialogue5 != null) Dialogue5.text = DialogueLines5[i];

            float elapsed = 0f;
            float waitFor = Mathf.Max(0f, delayBetweenItems);
            while (elapsed < waitFor)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        if (DialogueTrigger5 != null) DialogueTrigger5.SetActive(false);
        if (DialogueContainer5 != null) DialogueContainer5.SetActive(false);
        isProcessing = false;
    }
}