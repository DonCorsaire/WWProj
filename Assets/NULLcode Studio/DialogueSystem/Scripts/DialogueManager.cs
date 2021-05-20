using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;
using System.IO;
using System.Linq;

public class DialogueManager : MonoBehaviour
{

    public ScrollRect scrollRect;
    public ButtonComponent button;

    private readonly List<ButtonComponent> buttonsObjectPool = new List<ButtonComponent>();

    public string folder = "Russian"; // подпапка в Resources, для чтения
    public int offset = 20;

    private List<Dialogue> node;
    private readonly List<RectTransform> buttons = new List<RectTransform>();
    private float curY, height;
    private static DialogueManager _internal;

    //TO-DO вынести PreLoad и его переменные в загрузчик из менеджера.
    public List<Dialogue> resultNode;
    private Dialogue resultDialogue;
    private Answer resultAnswer;

    public static DialogueManager Internal
    {
        get { return _internal; }
    }

    void Awake()
    {
        _internal = this;
        button.gameObject.SetActive(false);
        scrollRect.gameObject.SetActive(false);
        InitiateObjectPool();
    }

    void InitiateObjectPool()
    {
        for (int i = 0; i < 10; i++)
        {
            ButtonComponent clone = Instantiate(button) as ButtonComponent;
            clone.gameObject.SetActive(false);
            clone.rect.SetParent(scrollRect.content);
            clone.rect.localScale = Vector3.one;

            buttonsObjectPool.Add(clone);
            buttons.Add(clone.rect);
        }
    }

    public List<Dialogue> PreLoad(string fileName)
    {
        resultNode = new List<Dialogue>();

        TextAsset binary = Resources.Load<TextAsset>(folder + "/" + fileName);
        using (XmlTextReader reader = new XmlTextReader(new StringReader(binary.text)))
        {
            int index = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement("node"))
                {
                    resultDialogue = new Dialogue
                    {
                        answer = new List<Answer>(),
                        npcText = reader.GetAttribute("npcText")
                    };
                    int.TryParse(reader.GetAttribute("nextNode"), out resultDialogue.nextNode);

                    resultNode.Add(resultDialogue);

                    using (XmlReader inner = reader.ReadSubtree())
                    {
                        while (inner.ReadToFollowing("answer"))
                        {
                            resultAnswer = new Answer
                            {
                                text = reader.GetAttribute("text")
                            };

                            if (int.TryParse(reader.GetAttribute("toNode"), out int number)) resultAnswer.toNode = number;
                            else resultAnswer.toNode = 0;

                            if (System.Enum.TryParse(reader.GetAttribute("dialogueInteract"), out DialogueInteract dialogueType)) resultAnswer.dialogueInteract = dialogueType;
                            else resultAnswer.dialogueInteract = DialogueInteract.None;

                            resultNode[index].answer.Add(resultAnswer);
                        }
                    }

                    index++;
                }
            }
        }
        return resultNode;
    }

    public void DialogueStart(List<Dialogue> dialogueNode)
    {
        node = dialogueNode;

        scrollRect.gameObject.SetActive(true);
        BuildDialogue(0);
    }

    void AddToList(int toNode, string text, bool isActive, DialogueInteract dialogueInteract)
    {
        BuildElement(toNode, text, isActive, dialogueInteract);
        curY += height + offset;
        RectContent();
    }

    void BuildElement(int toNode, string text, bool isActiveButton, DialogueInteract dialogueInteract)
    {
        ButtonComponent clone = buttonsObjectPool.First(b => !b.gameObject.activeSelf);

        clone.gameObject.SetActive(true);
        clone.text.text = text;
        clone.rect.sizeDelta = new Vector2(clone.rect.sizeDelta.x, clone.text.preferredHeight + offset);
        clone.button.interactable = isActiveButton;
        height = clone.rect.sizeDelta.y;
        clone.rect.anchoredPosition = new Vector2(0, -height / 2 - curY);

        if (toNode > 0) clone.button.onClick.AddListener(() => BuildDialogue(toNode));

        switch (dialogueInteract)
        {
            case DialogueInteract.None:
                break;
            case DialogueInteract.Exit:
                clone.button.onClick.AddListener(() => CloseDialogue());
                break;
            case DialogueInteract.Trade:
                clone.button.onClick.AddListener(() => StartTrade());
                break;
            case DialogueInteract.StartCutScene:
                clone.button.onClick.AddListener(() => StartCutScene());
                break;
            case DialogueInteract.Affect:
                break;
            case DialogueInteract.Required:
                break;
        }

        buttons.Add(clone.rect);
    }

    void RectContent()
    {
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, curY);
        scrollRect.content.anchoredPosition = Vector2.zero;
    }

    void ClearDialogue()
    {
        curY = offset;
        foreach (RectTransform b in buttons)
        {
            b.gameObject.SetActive(false);
            b.gameObject.GetComponent<ButtonComponent>().button.onClick.RemoveAllListeners();
        }
        RectContent();
    }

    #region ButtonEvents
    void StartTrade()
    {
        CloseDialogue();
        print("Начался трейд");
        //TO-DO Открыть окно трейда

    }

    void CloseDialogue()
    {
        scrollRect.gameObject.SetActive(false);
        ClearDialogue();
    }

    void StartCutScene()
    {
        //TO-DO Cutscenes
    }

    #endregion

    void BuildDialogue(int current)
    {
        ClearDialogue();
        AddToList(0, node[current].npcText, false, DialogueInteract.None); //реплика нпс создается как неактивная кнопка диалога - херня какая-то, лучше перебиндить на отдельную реплику, которая будет появляться как субтитр.
        if (node[current].answer.Count == 0)
        {
            AddToList(node[current].nextNode, "...Дальше...", true, DialogueInteract.None); //Временная кнопка "дальше" для перехода к следующей реплике. В дальнейшем можно заменить на отдельный триггер в виде пробела или конца реплики.
            return;
        }

        for (int i = 0; i < node[current].answer.Count; i++)
        {
            AddToList(node[current].answer[i].toNode, node[current].answer[i].text, true, node[current].answer[i].dialogueInteract);
        }
    }
}

public class Dialogue
{
    public string npcText;
    public int nextNode;
    public List<Answer> answer;
}


public class Answer
{
    public string text;
    public int toNode;
    public DialogueInteract dialogueInteract;
}

public enum DialogueInteract
{
    None,
    Exit,
    Trade,
    StartCutScene,
    Required,
    Affect
}