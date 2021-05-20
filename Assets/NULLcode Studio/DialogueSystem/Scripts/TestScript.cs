using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// пример, получения имени файла диалога и запуска процесса

	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                DialogueTrigger tr = hit.transform.GetComponent<DialogueTrigger>();
                if (tr != null && tr.fileName != string.Empty)
                {
                    DialogueManager.Internal.DialogueStart(tr.node);
                }
            }
        }
	}
}
