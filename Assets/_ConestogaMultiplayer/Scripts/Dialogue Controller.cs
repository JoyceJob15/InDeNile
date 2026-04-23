using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// DialogueController1 - only handles NPC/trigger 1.
// Everything for controllers 2 & 3 is intentionally commented out.

public class DialogueController : MonoBehaviour
{

    // Variables (controller 1 active)
    [SerializeField] private string[] DialogueLines1;
    // [SerializeField] private string[] DialogueLines2; // commented out
    // [SerializeField] private string[] DialogueLines3; // commented out
    [SerializeField] private TMPro.TMP_Text Task;
    [SerializeField] private GameObject DialogueTrigger1;
    // [SerializeField] private GameObject DialogueTrigger2; // commented out
    // [SerializeField] private GameObject DialogueTrigger3; // commented out

    [SerializeField] private TMP_Text Dialogue1;
    // [SerializeField] private TMP_Text Dialogue2; // commented out
    // [SerializeField] private TMP_Text Dialogue3; // commented out

    [SerializeField] private GameObject DialogueContainer1;
    // [SerializeField] private GameObject DialogueContainer2; // commented out
    // [SerializeField] private GameObject DialogueContainer3; // commented out

    // NEW: arrow GameObjects to behave like the container
    [SerializeField] private GameObject Arrow;
    [SerializeField] private GameObject Arrow2;

    [Header("Timing")]
    [SerializeField] private float delayBetweenItems;
    [SerializeField] private bool useUnscaledTime = false; // false => Time.deltaTime, true => Time.unscaledDeltaTime

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource DialogueAudioSource;
    [SerializeField] private AudioClip[] DialogueAudio1;
    [SerializeField] private bool waitForAudioToFinish = false; // if true, each step waits until clip length or delayBetweenItems whichever is longer

    [Header("Scene Progression")]
    [SerializeField] private GameObject SceneTrigger1; // NEW: assign the GameObject to enable after dialogue finishes

    private bool isProcessing = false;

    void Start()
    {
        if (DialogueContainer1 != null) DialogueContainer1.SetActive(false);
        if (DialogueTrigger1 != null) DialogueTrigger1.SetActive(true);

        // Ensure Arrow and Arrow2 are disabled at start
        //if (Arrow != null) Arrow.SetActive(false);
        if (Arrow2 != null) Arrow2.SetActive(false);

        // Ensure the SceneTrigger1 object is disabled at start (will be enabled after dialogue finishes)
        if (SceneTrigger1 != null) SceneTrigger1.SetActive(false);

        // Ensure we have an AudioSource if audio clips were assigned but source wasn't
        if ((DialogueAudio1 != null && DialogueAudio1.Length > 0) && DialogueAudioSource == null)
        {
            DialogueAudioSource = GetComponent<AudioSource>();
            if (DialogueAudioSource == null)
                DialogueAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // initialize defaults only if not set in Inspector
        if (DialogueLines1 == null || DialogueLines1.Length == 0)
        {
            DialogueLines1 = new string[8];
            DialogueLines1[0] = "Hey you! Yeah, you!!";
            DialogueLines1[1] = "Come over here!";
            DialogueLines1[2] = "I found this injured baby hippo here but I am a busy ferryman";
            DialogueLines1[3] = "I cannot take care of him";
            DialogueLines1[4] = "But there is a village up the river";
            DialogueLines1[5] = "where you can find some medicine for this little fella";
            DialogueLines1[6] = "So come on";
            DialogueLines1[7] = "Let's hurry up and go!";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProcessing && other.CompareTag("Player"))
        {
            // Only run this controller if this component instance belongs to the DialogueTrigger1 GameObject
            if (gameObject == DialogueTrigger1)
            {
                isProcessing = true;
                if (DialogueContainer1 != null) DialogueContainer1.SetActive(true);
                
                StartCoroutine(ProcessDialogueOverTime1());
            }
        }
    }

    IEnumerator ProcessDialogueOverTime1()
    {
        if (DialogueLines1 == null) yield break;

        for (int i = 0; i < DialogueLines1.Length; i++)
        {
            Debug.Log($"[{Time.realtimeSinceStartup:F2}s realtime] (C1) {DialogueLines1[i]}");
            if (Dialogue1 != null) Dialogue1.text = DialogueLines1[i];

            // Play audio for this line if available
            AudioClip clipToPlay = null;
            if (DialogueAudio1 != null && i < DialogueAudio1.Length)
                clipToPlay = DialogueAudio1[i];

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

        if (DialogueTrigger1 != null) DialogueTrigger1.SetActive(false);
        if (DialogueContainer1 != null) DialogueContainer1.SetActive(false);
        if (Arrow != null) Arrow.SetActive(false); // keep existing behavior
        if (Arrow2 != null) Arrow2.SetActive(true); // enable Arrow2 when dialogue ends
        Task.text = "Enter the boat";
        isProcessing = false;

        // NEW: enable a GameObject named "SceneTrigger1" when all dialogues are finished
        if (SceneTrigger1 != null) SceneTrigger1.SetActive(true);
    }

    // Controller 2 & 3 logic/commented intentionally:
    // /* DialogueLines2, DialogueTrigger2, Dialogue2, DialogueContainer2, ProcessDialogueOverTime2() */
    // /* DialogueLines3, DialogueTrigger3, Dialogue3, DialogueContainer3, ProcessDialogueOverTime3() */
}
