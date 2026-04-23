using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// DialogueController2 - only handles NPC/trigger 2.
// Everything for controllers 1 & 3 is intentionally commented out.

public class DialogueController2 : MonoBehaviour
{
    // Variables (controller 2 active)
    // [SerializeField] private string[] DialogueLines1; // commented out
    [SerializeField] private string[] DialogueLines2;
    // [SerializeField] private string[] DialogueLines3; // commented out
    [SerializeField] private TMPro.TMP_Text Task;
    // [SerializeField] private GameObject DialogueTrigger1; // commented out
    [SerializeField] private GameObject DialogueTrigger2;
    // [SerializeField] private GameObject DialogueTrigger3; // commented out

    // [SerializeField] private TMP_Text Dialogue1; // commented out
    [SerializeField] private TMP_Text Dialogue2;
    // [SerializeField] private TMP_Text Dialogue3; // commented out

    // [SerializeField] private GameObject DialogueContainer1; // commented out
    [SerializeField] private GameObject DialogueContainer2;
    // [SerializeField] private GameObject DialogueContainer3; // commented out

    [Header("Timing")]
    [SerializeField] private float delayBetweenItems;
    [SerializeField] private bool useUnscaledTime = false;

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource DialogueAudioSource;
    [SerializeField] private AudioClip[] DialogueAudio2;
    [SerializeField] private bool waitForAudioToFinish = false;

    private bool isProcessing = false;

    void Start()
    {
        if (DialogueContainer2 != null) DialogueContainer2.SetActive(false);
        if (DialogueTrigger2 != null) DialogueTrigger2.SetActive(true);

        // Ensure we have an AudioSource if audio clips were assigned but source wasn't
        if ((DialogueAudio2 != null && DialogueAudio2.Length > 0) && DialogueAudioSource == null)
        {
            DialogueAudioSource = GetComponent<AudioSource>();
            if (DialogueAudioSource == null)
                DialogueAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (DialogueLines2 == null || DialogueLines2.Length == 0)
        {
            DialogueLines2 = new string[13];
            DialogueLines2[0] = "You look like it is your first time in the Nile river";
            DialogueLines2[1] = "Are you from a land far far away?";
            DialogueLines2[2] = "No matter! If anyone knows this river, it is me!";
            DialogueLines2[3] = "The Nile river is one long river.";
            DialogueLines2[4] = "Stretching from Uganda through Sudan, and into Egypt.";
            DialogueLines2[5] = "The Nile is essential for many tribes and creatures living in and around it";
            DialogueLines2[6] = "They can be cute and cuddly like our little hippo friend here";
            DialogueLines2[7] = "But there are really scary creatures as well!";
            DialogueLines2[8] = "Like the big scary NILE CROCODILE";
            DialogueLines2[9] = "OH NO NO NO! Look out!! Nile Crocodiles ahead!";
            DialogueLines2[10] = "These guys are MASSIVE! And massively DANGEROUS as well!";
            DialogueLines2[11] = "Quick! Grab those paddles and hit them on the head!";
            DialogueLines2[12] = "They are damaging my boat!!";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProcessing && other.CompareTag("Player"))
        {
            if (gameObject == DialogueTrigger2)
            {
                isProcessing = true;
                if (DialogueContainer2 != null) DialogueContainer2.SetActive(true);
                StartCoroutine(ProcessDialogueOverTime2());
            }
        }
    }

    IEnumerator ProcessDialogueOverTime2()
    {
        if (DialogueLines2 == null) yield break;

        for (int i = 0; i < DialogueLines2.Length; i++)
        {
            Debug.Log($"[{Time.realtimeSinceStartup:F2}s realtime] (C2) {DialogueLines2[i]}");
            if (Dialogue2 != null) Dialogue2.text = DialogueLines2[i];

            // Play audio for this line if available
            AudioClip clipToPlay = null;
            if (DialogueAudio2 != null && i < DialogueAudio2.Length)
                clipToPlay = DialogueAudio2[i];

            if (DialogueAudioSource != null && clipToPlay != null)
            {
                DialogueAudioSource.PlayOneShot(clipToPlay);
            }

            float elapsed = 0f;
            float waitFor = Mathf.Max(0f, delayBetweenItems);

            // if requested, make sure we wait at least as long as the clip length
            if (waitForAudioToFinish && clipToPlay != null)
            {
                waitFor = Mathf.Max(waitFor, clipToPlay.length);
            }

            while (elapsed < waitFor)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        if (DialogueTrigger2 != null) DialogueTrigger2.SetActive(false);
        if (DialogueContainer2 != null) DialogueContainer2.SetActive(false);
        isProcessing = false;
        Task.text = "Hit the crocodiles";
    }

    // Controller 1 & 3 logic/commented intentionally:
    // /* DialogueLines1, DialogueTrigger1, Dialogue1, DialogueContainer1, ProcessDialogueOverTime1() */
    // /* DialogueLines3, DialogueTrigger3, Dialogue3, DialogueContainer3, ProcessDialogueOverTime3() */
}