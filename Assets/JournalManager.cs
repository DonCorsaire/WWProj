using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    bool journalOpened = false;

    public GameObject journalCanvas;

    public List<GameObject> pages;
    public List<UnityEngine.UI.Button> buttons;

    void Update()
    {
        //TO-Do вынести в отдельное меню управления
        if (Input.GetKeyDown(KeyCode.J))
        {
            JournalPressed();
        }
    }
    public void JournalPressed()
    {
        journalOpened = !journalOpened;
        Time.timeScale = journalOpened? 0: 1;
        journalCanvas.SetActive(journalOpened);
    }

    public void HidePagesEnableButtons()
    {
        foreach (GameObject p in pages)
        {
            p.SetActive(false);
        }
        foreach(UnityEngine.UI.Button b in buttons)
        {
            b.interactable = true;
        }
    }
}
