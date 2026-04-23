using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class StartLock : MonoBehaviour
{
    public GameObject Task;
    public GameObject TaskHeader;
    DynamicMoveProvider player;
    public GameObject Canvas;
    // Start is called before the first frame update
    void Start()
    {
        
        Task.SetActive(false);
        TaskHeader.SetActive(false);
    }

   
   public void Update()
    {
        player.moveSpeed = 0;
        
    }
    public void StartGame()
    {
          player.moveSpeed = 1.5f;
            Task.SetActive(true);
            TaskHeader.SetActive(true);
        Canvas.SetActive(false);

    }
    public void Quit
        ()
    {
        Application.Quit();
    }
}
