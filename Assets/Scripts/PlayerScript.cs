using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    public GameObject lockGameobject;
    [SerializeField]
    public GameObject doorGameobject;
    [SerializeField]
    public GameObject doorCanvas;
    [SerializeField]
    public GameObject gameCanvas;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            doorGameobject.layer = 0;
            lockGameobject.SetActive(true);
            gameCanvas.SetActive(true);
            doorCanvas.SetActive(true);
            this.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
