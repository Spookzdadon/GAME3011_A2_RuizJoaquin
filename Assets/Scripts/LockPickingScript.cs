using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Difficulty
{
    EASY,
    MEDIUM,
    HARD
}

public class LockPickingScript : MonoBehaviour
{
    [Header("Lock Attributes")]
    [SerializeField]
    private float pickSpeed;
    [SerializeField]
    private float lockRotationSpeed;
    [SerializeField]
    private float lockRetentionSpeed;
    [SerializeField]
    private float lockLeniency;
    [SerializeField]
    private float tensionMultiplier;
    [SerializeField]
    private float playerSkill = 0;
    [SerializeField]
    private Difficulty difficulty = Difficulty.EASY;

    [Header("Object References")]
    public TextMeshProUGUI lockDifficultyText;
    public TextMeshProUGUI playerSkillText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI lockPickText;
    public TextMeshProUGUI textBox;

    private Animator animator;
    private float lockPosition;
    private float pickPosition;
    private bool gameDone = false;
    private float targetPosition;
    private bool shaking;
    private float tension = 0f;
    private float timeValue = 120;
    private int lockPickAmount = 3;
    

    

    private void Awake()
    {
        animator = GetComponent<Animator>();   
    }
    private void Start()
    {
        LockPosition = 0;
        PickPosition = 0;
        tension = 0f;
        targetPosition = Random.value;
        lockPickText.text = lockPickAmount.ToString();
    }

    private void Update()
    {
        if (!gameDone)
        {
            if (Input.GetAxisRaw("Vertical") == 0)
            {
                Pick();
            }
            Shaking();
            Lock();
            UpdateAnimator();
            Timer();
        }
    }

    private void Shaking()
    {
        shaking = MaxRotationDistance - LockPosition < 0.03f;
        if (shaking)
        {
            tension += Time.deltaTime * tensionMultiplier;
            if (tension > 1.5f)
            {
                BreakPick();
            }
        }
    }

    private void BreakPick()
    {
        Debug.Log("Lock pick broken");
        //textBox.text = "Lock pick broke!";
        SetTextBox("Lock pick broke!");
        shaking = false;
        LockPosition = 0;
        PickPosition = 0;
        lockPickAmount -= 1;
        if (lockPickAmount <=0)
        {
            lockPickAmount = 0;
            textBox.text = "You ran out of lock picks!";
            gameDone = true;
        }
        tension = 0f;
        lockPickText.text = lockPickAmount.ToString();

    }

    private float MaxRotationDistance
    {
        get
        {
            return 1f - Mathf.Abs(targetPosition - PickPosition) + lockLeniency + (playerSkill / 100);
        }
    }

    private float PickPosition
    {
        get { return pickPosition; }
        set 
        { 
            pickPosition = value;
            pickPosition = Mathf.Clamp(pickPosition, 0f, 1f);
        }
    }

    private float LockPosition
    {
        get { return lockPosition; }
        set
        {
            lockPosition = value;
            lockPosition = Mathf.Clamp(lockPosition, 0f, MaxRotationDistance);
        }
    }


    private void Pick()
    {
        PickPosition += Input.GetAxisRaw("Mouse X") * Time.deltaTime * pickSpeed;
    }

    private void Lock()
    {
        LockPosition -= lockRetentionSpeed * Time.deltaTime;
        LockPosition += Mathf.Abs(Input.GetAxisRaw("Vertical")) * Time.deltaTime * lockRotationSpeed;

        if(LockPosition > 0.98f)
        {
            Win();
        }
    }

    private void Win()
    {
        gameDone = true;
        textBox.text = "You opened the lock!";
        //SetTextBox("You opened the lock!");
        print("You opened the lock!");
    }

    public void ResetGame()
    {
        lockPickAmount = 3;
        timeValue = 120f;
        LockPosition = 0;
        PickPosition = 0;
        tension = 0f;
        textBox.text = "";
        lockPickText.text = lockPickAmount.ToString();
        gameDone = false;
    }

    public void SetDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.EASY:
                lockLeniency = 0.0005f;
                lockDifficultyText.text = "Medium";
                difficulty = Difficulty.MEDIUM;
                break;
            case Difficulty.MEDIUM:
                lockLeniency = 0.0001f;
                lockDifficultyText.text = "Hard";
                difficulty = Difficulty.HARD;
                break;
            case Difficulty.HARD:
                lockLeniency = 0.001f;
                lockDifficultyText.text = "Easy";
                difficulty = Difficulty.EASY;
                break;
        }

    }

    public void IncreasePlayerSkill()
    {
        if (playerSkill >= 99)
        {
            return;
        }
        playerSkill += 1f;
        int floatToInt = (int)(playerSkill);
        playerSkillText.text = (floatToInt).ToString();
    }

    public void DecreasePlayerSkill()
    {
        if (playerSkill <= 0)
        {
            return;
        }
        playerSkill -= 1f;
        int floatToInt = (int)(playerSkill);
        playerSkillText.text = (floatToInt).ToString();
    }

    private void Timer()
    {
        if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            timeValue = 0;
            gameDone = true;
            textBox.text = "You ran out of time!";
        }

        float minutes = Mathf.FloorToInt(timeValue / 60);
        float seconds = Mathf.FloorToInt(timeValue % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void SetTextBox(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(SetText(msg));
    }

    IEnumerator SetText(string msg)
    {
        textBox.text = msg;
        yield return new WaitForSeconds(2);
        textBox.text = "";
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("PickPosition", PickPosition);
        animator.SetFloat("LockOpen", LockPosition);
        animator.SetBool("Shake", shaking);
    }
}
