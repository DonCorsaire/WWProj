using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{

    public string fileName; // указываем имя файла диалога

    public List<Dialogue> node = new List<Dialogue>();


    //TO-DO Добавить общий Интерфейс для сбора и предзагрузки для всех объектов на стадии loading.
    void Start()
    {
        LoadDialogueFromXML();
    }

    public void LoadDialogueFromXML()
    {
        node = DialogueManager.Internal.PreLoad(fileName);
        print("Preloaded with first node: " + node[0].npcText);
    }
}
