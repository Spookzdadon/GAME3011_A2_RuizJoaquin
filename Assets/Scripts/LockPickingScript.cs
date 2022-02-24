using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPickingScript : MonoBehaviour
{
    [SerializeField]
    private float pickSpeed;
    [SerializeField]
    private float lockRotationSpeed;
    [SerializeField]
    private float lockRetentionSpeed;
    [SerializeField]
    private float lockleniency;
    [SerializeField]
    private float tensionMultiplier;

    private Animator animator;
    private float lockPosition;
    private float pickPosition;
    private bool gameDone = false;
    private float targetPosition;
    private bool shaking;
    private float tension = 0f;
    

    private void Start()
    {
        LockPosition = 0;
        PickPosition = 0;
        tension = 0f;

        targetPosition = Random.value;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();   
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
        }
    }

    private void Shaking()
    {
        shaking = MaxRotationDistance - LockPosition < 0.03f;
        if (shaking)
        {
            tension += Time.deltaTime * tensionMultiplier;
            if (tension > 1f)
            {
                BreakPick();
            }
        }
    }

    private void BreakPick()
    {
        Debug.Log("Lock pick broken");
        shaking = false;
        gameDone = true;
    }

    private float MaxRotationDistance
    {
        get
        {
            return 1f - Mathf.Abs(targetPosition - PickPosition) + lockleniency;
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
        PickPosition += Input.GetAxisRaw("Horizontal") * Time.deltaTime * pickSpeed;
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
        print("You opened the lock!");
    }

    public void ResetGame()
    {
        LockPosition = 0;
        PickPosition = 0;
        tension = 0f;
        gameDone = false;
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("PickPosition", PickPosition);
        animator.SetFloat("LockOpen", LockPosition);
        animator.SetBool("Shake", shaking);
    }
}
