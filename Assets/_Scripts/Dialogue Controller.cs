using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    //Variables

    [SerializeField] private string[] DialogueLines1;
    [SerializeField] private string[] DialogueLines2;
    [SerializeField] private string[] DialogueLines3;
    [SerializeField] private GameObject DialogueTrigger1;
    [SerializeField] private GameObject DialogueTrigger2;
    [SerializeField] private GameObject DialogueTrigger3;
    [SerializeField] private TMP_Text Dialogue1;
    [SerializeField] private TMP_Text Dialogue2;
    [SerializeField] private TMP_Text Dialogue3;
    [SerializeField] private GameObject DialogueContainer1;
    [SerializeField] private GameObject DialogueContainer2;
    [SerializeField] private GameObject DialogueContainer3;
    [SerializeField] private float delayBetweenItems;
    [SerializeField] private bool useUnscaledTime = false; // false => Time.deltaTime, true => Time.unscaledDeltaTime
    [SerializeField] private bool Dialogue1Active;
    private bool isProcessing = false;

    //Code
    void Start()
    {
        if (DialogueContainer1 != null) DialogueContainer1.SetActive(false);
        if (DialogueContainer2 != null) DialogueContainer2.SetActive(false);
        if (DialogueContainer3 != null) DialogueContainer3.SetActive(false);

        if (DialogueTrigger1 != null) DialogueTrigger1.SetActive(true);

        Debug.Log($"DialogueController: delayBetweenItems (Inspector) = {delayBetweenItems} seconds, useUnscaledTime = {useUnscaledTime}");

        // initialize defaults only if not set in Inspector
        if (DialogueLines1 == null || DialogueLines1.Length == 0)
        {
            DialogueLines1 = new string[12];
            DialogueLines1[0] = "Hey you!";
            DialogueLines1[1] = "Come here!";
            DialogueLines1[2] = "I found this injured baby hippo..I am a busy ferryman";
            DialogueLines1[3] = "I do not have time to care for babies!";
            DialogueLines1[4] = "I need to make a delivery at a village nearby";
            DialogueLines1[5] = "There might be a farmer there who would be willing";
            DialogueLines1[6] = "to give you some medicine for this little guy.";
            DialogueLines1[7] = "Perhaps you could come with me to get them";
            DialogueLines1[8] = "and we can try to get this one back to it's family!";
            DialogueLines1[9] = "So! Whaddya say?";
            DialogueLines1[10] = "Whenever you are ready, go get on the boat!";
            DialogueLines1[11] = "Let's go!";
        }

        if (DialogueLines2 == null || DialogueLines2.Length == 0)
        {
            DialogueLines2 = new string[12];
            DialogueLines2[0] = "You look like you haven't been to the Nile before";
            DialogueLines2[1] = "Did you know? As pretty as the Nile looks,";
            DialogueLines2[2] = "It can be just as dangerous.";
            DialogueLines2[3] = "The Nile is home to many creatures.";
            DialogueLines2[4] = "Such as, the ferocious Nile crocodile";
            DialogueLines2[5] = "Woah!! Watch out!! Crocs ahead! They seem hungry..";
            DialogueLines2[6] = "They're attacking my boat!! This is not good!";
            DialogueLines2[7] = "Quick! Grab a paddle and hit them!";
            DialogueLines2[8] = "Are they gone yet?!";
            DialogueLines2[9] = "That was terrifying! you really knocked them out!";
            DialogueLines2[10] = "Hey look! We reached the village!";
            DialogueLines2[11] = "I've got some errands to run so, you go on ahead!";
        }

        if (DialogueLines3 == null || DialogueLines3.Length == 0)
        {
            DialogueLines3 = new string[6];
            DialogueLines3[0] = "Hello there, traveler.";
            DialogueLines3[1] = "Welcome to our market.";
            DialogueLines3[2] = "We trade goods from faraway lands.";
            DialogueLines3[3] = "If you need supplies, I can help.";
            DialogueLines3[4] = "Be careful at night — the paths can be rough.";
            DialogueLines3[5] = "Safe travels.";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProcessing && other.CompareTag("Player"))
        {
            isProcessing = true;

            // Determine which trigger this script instance is running on.
            if (gameObject == DialogueTrigger1)
            {   
                if (DialogueContainer1 != null) DialogueContainer1.SetActive(true);
                StartCoroutine(ProcessDialogueOverTime1());
            }
            else if (gameObject == DialogueTrigger2)
            {   
                if (DialogueContainer2 != null) DialogueContainer2.SetActive(true);
                StartCoroutine(ProcessDialogueOverTime2());
            }
            else if (gameObject == DialogueTrigger3)
            {   
                if (DialogueContainer3 != null) DialogueContainer3.SetActive(true);
                StartCoroutine(ProcessDialogueOverTime3());
            }
            else
            {
                // fallback: choose by the Dialogue1Active flag (keeps previous behavior)
                if (Dialogue1Active)
                    StartCoroutine(ProcessDialogueOverTime1());
                else
                    StartCoroutine(ProcessDialogueOverTime2());
            }
        }
    }

    IEnumerator ProcessDialogueOverTime1()
    {
        if (DialogueLines1 == null) yield break;

        for (int i = 0; i < DialogueLines1.Length; i++)
        {
            Debug.Log($"[{Time.realtimeSinceStartup:F2}s realtime] Processing item: {DialogueLines1[i]} at index {i}");
            if (Dialogue1 != null) Dialogue1.text = DialogueLines1[i];

            float elapsed = 0f;
            float waitFor = Mathf.Max(0f, delayBetweenItems);
            while (elapsed < waitFor)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        if (DialogueTrigger1 != null) DialogueTrigger1.SetActive(false);
        if (DialogueContainer1 != null) DialogueContainer1.SetActive(false);
        isProcessing = false;
    }

    IEnumerator ProcessDialogueOverTime2()
    {
        if (DialogueLines2 == null) yield break;

        for (int i = 0; i < DialogueLines2.Length; i++)
        {
            Debug.Log($"[{Time.realtimeSinceStartup:F2}s realtime] Processing item: {DialogueLines2[i]} at index {i}");
            if (Dialogue2 != null) Dialogue2.text = DialogueLines2[i];

            float elapsed = 0f;
            float waitFor = Mathf.Max(0f, delayBetweenItems);
            while (elapsed < waitFor)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        if (DialogueTrigger2 != null) DialogueTrigger2.SetActive(false);
        if (DialogueContainer2 != null) DialogueContainer2.SetActive(false);
        isProcessing = false;
    }

    IEnumerator ProcessDialogueOverTime3()
    {
        if (DialogueLines3 == null) yield break;

        for (int i = 0; i < DialogueLines3.Length; i++)
        {
            Debug.Log($"[{Time.realtimeSinceStartup:F2}s realtime] Processing item: {DialogueLines3[i]} at index {i}");
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
