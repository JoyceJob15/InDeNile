using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// DialogueController4 - renamed and fully using '4' suffix for fields/methods.
public class Dialoguetrigger6 : MonoBehaviour
{
    // Variables (controller 4)
    [SerializeField] private string[] DialogueLines6;
    [SerializeField] private GameObject DialogueTrigger6;
    [SerializeField] private TMPro.TMP_Text Task;
    [SerializeField] private TMP_Text Dialogue6;
    [SerializeField] private GameObject DialogueContainer6;
   
    
    [SerializeField] private float delayBetweenItems = 2f;
    [SerializeField] private bool useUnscaledTime = false; // false => Time.deltaTime, true => Time.unscaledDeltaTime

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource DialogueAudioSource;
    [SerializeField] private AudioClip[] DialogueAudio6;
    [SerializeField] private bool waitForAudioToFinish = false;

    private bool isProcessing = false;

    void Start()
    {
        if (DialogueContainer6 != null) DialogueContainer6.SetActive(false);
        if (DialogueTrigger6 != null) DialogueTrigger6.SetActive(true);
        
        // Ensure we have an AudioSource if audio clips were assigned but source wasn't
        if ((DialogueAudio6 != null && DialogueAudio6.Length > 0) && DialogueAudioSource == null)
        {
            DialogueAudioSource = GetComponent<AudioSource>();
            if (DialogueAudioSource == null)
                DialogueAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (DialogueLines6 == null || DialogueLines6.Length == 0)
        {
            DialogueLines6 = new string[3];
            DialogueLines6[0] = "Thank you for helping with the farming!";
            DialogueLines6[1] = "Here is the medicine you need!";
            DialogueLines6[2] = "Good luck and safe travels!";
     
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProcessing && other.CompareTag("Player"))
        {
            if (gameObject == DialogueTrigger6 || DialogueTrigger6 == null)
            {
                isProcessing = true;
                if (DialogueContainer6 != null) DialogueContainer6.SetActive(true);
                StartCoroutine(ProcessDialogueOverTime4());
                
            }
        }
    }

    IEnumerator ProcessDialogueOverTime4()
    {
        if (DialogueLines6 == null) yield break;

        for (int i = 0; i < DialogueLines6.Length; i++)
        {
            Debug.Log($"[{Time.realtimeSinceStartup:F2}s realtime] (C4) {DialogueLines6[i]}");
            if (Dialogue6 != null) Dialogue6.text = DialogueLines6[i];

            // Play audio for this line if available
            AudioClip clipToPlay = null;
            if (DialogueAudio6 != null && i < DialogueAudio6.Length)
                clipToPlay = DialogueAudio6[i];

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

        if (DialogueTrigger6 != null) DialogueTrigger6.SetActive(false);
        if (DialogueContainer6 != null) DialogueContainer6.SetActive(false);
        
        isProcessing = false;
        Task.text = "Go to the boat";
    }
}