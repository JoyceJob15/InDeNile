using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class StartLock : MonoBehaviour
{
    public GameObject Task;
    public GameObject TaskHeader;
    [SerializeField] private DynamicMoveProvider player;
    public GameObject Canvas;
    public GameObject uibuttons;
    public GameObject playerobject;
    public GameObject StartMenu;
    // Start is called before the first frame update
    void Start()
    {
        if (Task != null) Task.SetActive(false);
        if (TaskHeader != null) TaskHeader.SetActive(false);

        // Try to resolve the player MoveProvider if it wasn't assigned in the Inspector
        if (player == null)
        {
            if (playerobject != null)
                player = playerobject.GetComponent<DynamicMoveProvider>() ?? playerobject.GetComponentInChildren<DynamicMoveProvider>();

            if (player == null)
                player = FindObjectOfType<DynamicMoveProvider>();
        }

        if (player != null)
            player.moveSpeed = 0;
        else
            Debug.LogWarning("StartLock: DynamicMoveProvider 'player' is null. Movement calls will be skipped.", this);

        if (playerobject != null)
            playerobject.SetActive(false);
    }

    void Update()
    {
       
    }

    public void StartGame()
    {
        if (playerobject != null)
            playerobject.SetActive(true);

        
            player.moveSpeed = 1.5f;
        StartMenu.SetActive(false);


        if (Task != null) Task.SetActive(true);
        if (TaskHeader != null) TaskHeader.SetActive(true);
        if (uibuttons != null) uibuttons.SetActive(false);

    }

    public void Quit()
    {
        Application.Quit();
    }
}
