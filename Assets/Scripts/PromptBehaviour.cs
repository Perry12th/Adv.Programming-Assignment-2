using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PromptBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject minigame;
    [SerializeField]
    LockPickMinigame pickMinigame;
    [SerializeField]
    Image chest;
    [SerializeField]
    TextMeshProUGUI promptText;
    [SerializeField]
    bool gameRunning;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (gameRunning)
            {
                pickMinigame.ResetGame();
                minigame.SetActive(false);
                chest.gameObject.SetActive(true);
                promptText.gameObject.SetActive(true);
                gameRunning = false;
            }
            else
            {
                pickMinigame.ResetGame();
                minigame.SetActive(true);
                chest.gameObject.SetActive(false);
                promptText.gameObject.SetActive(false);
                gameRunning = true;
            }
        }
    }
}
