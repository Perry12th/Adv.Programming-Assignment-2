using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum LockpickLevel
{ 
    HARD,
    MEDIUM,
    EASY
}

public enum PlayerSkillLevel
{ 
    ROOKIE,
    ADAPT,
    EXPERT
}


public enum GameState
{ 
    SETTINGUP,
    MOVINGPIN,
    UNLOCKING,
    FINISHED
}


public class LockPickMinigame : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField]
    GameObject Lock;
    [SerializeField]
    GameObject OuterRing;
    [SerializeField]
    GameObject RotatingParts;
    [SerializeField]
    GameObject BobbyPin;

    [Header("Game Stats")]
    [SerializeField]
    GameState state;
    [SerializeField]
    LockpickLevel lockpickLevel;
    [SerializeField]
    PlayerSkillLevel playerSkillLevel;
    [SerializeField]
    [Range(-90, 90)]
    float pickAngle;
    [SerializeField]
    [Range(-90, 90)]
    int sweetSpot;
    [SerializeField]
    int sweetSpotRange;
    [SerializeField]
    float timeLeft;
    [SerializeField]
    float startingTime;
    [SerializeField]
    float turnRate;
    [SerializeField]
    [Range(0, 1)]
    float unlockRatio;
    [SerializeField]
    float timer;
    [SerializeField]
    float rotateTime;
    [SerializeField]
    bool movingLock;

    [Header("Game UI")]
    [SerializeField]
    TextMeshProUGUI TimerText;
    [SerializeField]
    TextMeshProUGUI PlayerSkillText;
    [SerializeField]
    TextMeshProUGUI LockpickLevelText;
    [SerializeField]
    TMP_Dropdown PlayerSkillDropDown;
    [SerializeField]
    TMP_Dropdown LockpickLevelDropDown;
    [SerializeField]
    TextMeshProUGUI MessageField;
    [SerializeField]
    Button StartGameButton;
    [SerializeField]
    Button RestartGameButton;

    [Header("Game Effects")]
    [SerializeField]
    Material easyMaterial;
    [SerializeField]
    Material mediumMaterial;
    [SerializeField]
    Material hardMaterial;


    // Start is called before the first frame update
    void Start()
    {
        ChangeLockLevel((int)lockpickLevel);
        ChangePlayerSkillLevel((int)playerSkillLevel);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.MOVINGPIN || state == GameState.UNLOCKING)
        {
            timeLeft -= Time.deltaTime;
            TimerText.text = ((int)timeLeft).ToString();
            if (timeLeft <= 0)
            {
                state = GameState.FINISHED;
                MessageField.text = "Game Over you failed to lock pick the lock in time, please reset the game to try again";
            }

            if (state == GameState.MOVINGPIN)
            {

                if (Input.GetKey(KeyCode.A))
                {
                    pickAngle -= turnRate * Time.deltaTime;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    pickAngle += turnRate * Time.deltaTime;
                }

                pickAngle = Mathf.Clamp(pickAngle, -90, 90);
                BobbyPin.transform.eulerAngles = new Vector3(pickAngle, BobbyPin.transform.eulerAngles.y, BobbyPin.transform.eulerAngles.z);

                if (Input.GetKey(KeyCode.W))
                {
                    float sweetspotDiff = Mathf.Abs(sweetSpot - pickAngle);
                    if (sweetspotDiff <= sweetSpotRange)
                    {
                        unlockRatio = 1.0f;
                    }
                    else if (sweetspotDiff <= sweetSpotRange * 2f)
                    {
                        unlockRatio = 0.7f;
                    }
                    else if (sweetspotDiff <= sweetSpotRange * 3f)
                    {
                        unlockRatio = 0.4f;
                    }
                    else
                    {
                        unlockRatio = 0.2f;
                    }
                    StartCoroutine(TurnLock(90 * unlockRatio));
                    state = GameState.UNLOCKING;
                }
            }

            else if (state == GameState.UNLOCKING)
            {
                if (unlockRatio != 0 && !Input.GetKey(KeyCode.W))
                {
                    StopAllCoroutines();
                    unlockRatio = 0;
                    StartCoroutine(TurnLock(0));
                }

                if (!movingLock)
                {
                    if (unlockRatio == 1)
                    {
                        state = GameState.FINISHED;
                        MessageField.text = "Nice you opened the lock!! You can restart the game to try again or pick different settings";
                    }
                    else
                    {
                        state = GameState.MOVINGPIN;
                        if (unlockRatio == 0.7f)
                        {
                            MessageField.text = "You're really close!!";
                        }
                        else if (unlockRatio == 0.4f)
                        {
                            MessageField.text = "Getting warmer";
                        }
                        else if (unlockRatio == 0.2f)
                        {
                            MessageField.text = "Not even close";
                        }
                    }
                }
            }
        }
    }

    public void ChangeLockLevel(int newLevel)
    {
        
        lockpickLevel = (LockpickLevel)newLevel;

        MeshRenderer renderer = OuterRing.GetComponent<MeshRenderer>();

        // Change the outer rings color
        switch (lockpickLevel)
        {
            case (LockpickLevel.EASY):
                renderer.material = easyMaterial;
                break;

            case (LockpickLevel.MEDIUM):
                renderer.material = mediumMaterial;
                break;

            case (LockpickLevel.HARD):
                renderer.material = hardMaterial;
                break;
        }

        startingTime = 40 + 20 * (int)lockpickLevel;
        timeLeft = startingTime;
        TimerText.text = ((int)timeLeft).ToString();
    }

    public void ChangePlayerSkillLevel(int newLevel)
    {
        playerSkillLevel = (PlayerSkillLevel)newLevel;

        sweetSpotRange = 5 + 5 * (int)playerSkillLevel;
    }

    public void StartGame()
    {
        sweetSpot = Random.Range(-90, 90);
        state = GameState.MOVINGPIN;
        LockpickLevelDropDown.interactable = false;
        PlayerSkillDropDown.interactable = false;
        StartGameButton.gameObject.SetActive(false);
        RestartGameButton.gameObject.SetActive(true);
        MessageField.text = "Use the A and D to move the boddy pin, press and hold the W key to try to unlock the lock, release W to reset the lock to move the pin again";
    }

    public void ResetGame()
    {
        StopAllCoroutines();
        timeLeft = startingTime;
        TimerText.text = ((int)timeLeft).ToString();
        pickAngle = 0;
        state = GameState.SETTINGUP;
        LockpickLevelDropDown.interactable = true;
        PlayerSkillDropDown.interactable = true;
        StartGameButton.gameObject.SetActive(true);
        RestartGameButton.gameObject.SetActive(false);
        RotatingParts.transform.localRotation = Quaternion.Euler(0, 0, 0);
        BobbyPin.transform.eulerAngles = new Vector3(0, BobbyPin.transform.eulerAngles.y, BobbyPin.transform.eulerAngles.z);
        MessageField.text = "Welcome to my little lockpick minigame here in the setup you can pick the player's skill level and lock pick level. Click the start game button to start.";
    }

    private IEnumerator TurnLock(float newYAngle)
    {
        timer = 0;
        float startingAngle = RotatingParts.transform.localRotation.eulerAngles.y;
        float endAngle = newYAngle;
        movingLock = true;

        do
        {
            float currentAngle = Mathf.Lerp(startingAngle, endAngle, timer / rotateTime);
            RotatingParts.transform.localRotation = Quaternion.Euler(0, currentAngle, 0);
            timer += Time.deltaTime;
            yield return null;
        } while (timer < rotateTime);

        movingLock = false;
        RotatingParts.transform.localRotation = Quaternion.Euler(0, endAngle, 0);
        timer = 0;

    }
}
