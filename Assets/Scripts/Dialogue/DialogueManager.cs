using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;
using System.IO;
using System;
using System.Xml.Linq;

public class DialogueManager : MonoBehaviour {
	
	public ScrollRect scrollRect;
	public GameObject companion;
	public GameObject player;

	public string folder = "Russian"; // подпапка в Resources, для чтения
	public string fileNameDialogueTriggerTramp; // указываем имя файла диалога
	
	public int valueStatus;
	public int offset = 20;

	private ButtonComponent[] buttonText; // первый элемент списка, всегда будет использоваться для вывода текста NPC, остальные элементы для ответов, соответственно, общее их количество должно быть достаточным
	private Inventory inventoryPlayer;
	private GameObject inventoryConvas;
	private InventoryObject inventorTramp;
	private string fileName;
	private string lastName;
	private List<Dialogue> node;
	private Dialogue dialogue;
	private Answer answer;
	private float curY;
	private float height;
	private int id;
	private Text buyText;

	void Start()
	{
		QuestReset();
		valueStatus = 0; // начальное значение статуса
		enabled = false;
		buttonText = scrollRect.GetComponentsInChildren<ButtonComponent>();
		inventoryPlayer = player.GetComponent<Inventory>();
		inventorTramp = companion.GetComponent<InventoryObject>();
		History.OnStatusСhanged += OnStatusСhanged;//событие изменения статуса в других диалогах
		inventoryConvas = inventoryPlayer.objectCanvasInventory;
		CloseWindow();
	}
	 void OnStatusСhanged(string companionName, int stat)
    {
		if(companionName!= companion.name)
        {
			switch (companionName)
			{//здесь меняем статус диалога в зависимости с кем общались companionName
				case "Tramp":
					if (valueStatus == 1 && companion.name == "TwinVagrant" && stat==1)
                    {
						valueStatus = 3;//Tramp и TwinVagrant отдал деньги
					}
                    else
                    {
						//valueStatus = 4;//иначе только Tramp
					}
					break;
				
			}
		}
	}
	void OnTriggerEnter(Collider col)
	{
		if (col.tag.Contains("Player"))//  вхождения в тригер разговора с бродягой
		{
			DialogueStart(fileNameDialogueTriggerTramp);
			inventoryConvas.SetActive(true);
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag.Contains("Player"))//выхода из тригера разговора с бродягой
		{
			inventoryConvas.SetActive(false);
			CloseWindow();
		}
	}

	void QuestReset()
	{
		enabled = false;
		valueStatus = 0;
	}

	void DialogueStart(string name)
	{
		if (name == string.Empty)
		{ 
			return;
		}
		fileName = name;
		Load();
	}

	void Load()
	{
		if(lastName == fileName) // проверка, чтобы не загружать уже загруженный файл
		{
			BuildDialogue(0);
			return;
		}

		node = new List<Dialogue>();

		try // чтение элементов XML и загрузка значений атрибутов в массивы
		{
			XDocument xdoc = XDocument.Load(Application.dataPath+ "/Resources/" + folder + "/" + fileName+".xml");
			foreach (XElement node_ in xdoc.Element("dialogue").Elements("node"))
			{
				dialogue = new Dialogue();
				dialogue.answer = new List<Answer>();
				dialogue.questValue = new List<int>();
				dialogue.npcText = new List<string>();
				dialogue.id = GetINT(node_.Attribute("id").Value);
				dialogue.tipDialoga = node_.Attribute("tipDialoga").Value;// 1- вопросы к персонажу, 2- диалог с персонажом, 3- описание с действиями,
				
				int index = 0;
				foreach (XElement npcs in node_.Elements("npc"))
                {
					XAttribute elm = npcs.Attribute("npcText");
					if (elm != null)
					{
						dialogue.npcText.Add(elm.Value);
					}
					elm = npcs.Attribute("questValue");
					if (elm != null)
					{
						dialogue.questValue.Add(GetINT(elm.Value));
					}
					

				}
				
				foreach (XElement answers in node_.Elements("answer"))
				{
					answer = new Answer();
					XAttribute elm = answers.Attribute("text");
					if (elm != null)
					{
						answer.text = elm.Value;
					}
					elm = answers.Attribute("toNode");
					if (elm != null)
					{
						answer.toNode = GetINT(elm.Value);
					}
					elm = answers.Attribute("exit");
					if (elm != null)
                    {
						answer.exit = GetBOOL(elm.Value);
					}
					elm = answers.Attribute("questStatus");
					if (elm != null)
					{
						answer.questStatus = GetINT(elm.Value);
                    }
                   
					elm = answers.Attribute("questValue");
					if (elm != null)
					{
						answer.questValue = GetINT(elm.Value);
					}
					elm = answers.Attribute("questValueGreater");
					if (elm != null)
					{
						answer.questValueGreater = GetINT(elm.Value);
					}
					elm = answers.Attribute("questName");
					if (elm != null)
					{
						answer.questName = elm.Value;
					}

					dialogue.answer.Add(answer);
				}

				node.Add(dialogue);
				index++;

			}
			lastName = fileName;
		}
		catch(System.Exception error)
		{
			Debug.Log(this + " ошибка чтения файла диалога: " + fileName + ".xml | Error: " + error.Message);
			CloseWindow();
			lastName = string.Empty;
		}

		BuildDialogue(0);
	}

	void CreatCorol(Color col)
    {
		ColorBlock cb = buttonText[0].button.colors;
		cb.disabledColor = col;
		buttonText[0].button.colors = cb;
		buyText = buttonText[0].button.GetComponentInChildren<Text>();
		buyText.color = Convert(col);//выодим текст в чорно-белой тональности с учетом фона
	}

	private Color Convert(Color value)
	{
		var c = value;
		var l = 0.2126 * c.r + 0.7152 * c.g + 0.0722 * c.b;
		return l < 0.5 ? Color.white : Color.black;
	}

	void AddToList(bool exit, int toNode, string text, int questStatus, string questName, bool isActive)
	{
        buttonText[id].text.text = text;
		buttonText[id].rect.sizeDelta = new Vector2(buttonText[id].rect.sizeDelta.x, buttonText[id].text.preferredHeight + offset);
		buttonText[id].button.interactable = isActive;
		height = buttonText[id].rect.sizeDelta.y;
		buttonText[id].rect.anchoredPosition = new Vector2(0, -height/2 - curY);

		if(exit)
		{
			SetExitDialogue(buttonText[id].button);
			 SetQuestStatus(buttonText[id].button, questStatus, questName);
		}
		else
		{
			SetNextNode(buttonText[id].button, toNode);
			SetQuestStatus(buttonText[id].button, questStatus, questName);
		}

		id++;

		curY += height + offset;
		RectContent();
	}

	void RectContent()
	{
		scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, curY);
		scrollRect.content.anchoredPosition = Vector2.zero;
	}

	void ClearDialogue()
	{
		id = 0;
		curY = offset;
		foreach(ButtonComponent b in buttonText)
		{
			b.text.text = string.Empty;
			b.rect.sizeDelta = new Vector2(b.rect.sizeDelta.x, 0);
			b.rect.anchoredPosition = new Vector2(b.rect.anchoredPosition.x, 0);
			b.button.onClick.RemoveAllListeners();
		}
		RectContent();
	}

	void SetQuestStatus(Button button, int i, string name) // событие, для управлением статуса, текущего квеста
	{
		string t = name + "|" + i; // склейка имени квеста и значения, которое ему назначено
		button.onClick.AddListener(() => QuestStatus(t));
	}

	void SetNextNode(Button button, int i) // событие, для перенаправления на другой узел диалога
	{
		button.onClick.AddListener(() => BuildDialogue(i));//указываем функцию обратного вызова при клике по кнопке
	}

	void SetExitDialogue(Button button) // событие, для выхода из диалога
	{
		button.onClick.AddListener(() => CloseWindow());
	}

	void QuestStatus(string s) // меняем статус квеста
	{
		string[] t = s.Split(new char[]{'|'});
		switch (t[1])
		{
			case "0":
				//SetActiveQuest();
				break;
			case "1":
				SetActiveQuest(); 
				break;
			case "2":
				QuestReset();
				break;
			case "3":
				SetCompleteQuest();
				break;
			case "4":
				valueStatus =4;
				break;
			case "5":
				valueStatus = 5;
				break;
		}
	}

	void SetActiveQuest()
	{
		History.NameCompanion = companion.name;
		History.Status = 1;//приведет к вызову события в классе History и все подписанные диалоги получят сообщение
		valueStatus = 1; // квест активен
		enabled = true;
		inventoryConvas.SetActive(true);
		inventoryPlayer.AddItem(0, inventorTramp);
		inventoryPlayer.AddItem(1, inventorTramp);
		inventoryPlayer.UpdateUI();
	}

	void SetCompleteQuest()
	{
		enabled = false;
		valueStatus = -1; // квест сдан
		Application.Quit();    // закрыть приложение
	}

	void CloseWindow() // закрываем окно диалога
	{
		scrollRect.gameObject.SetActive(false);
	}

	void ShowWindow() // показываем окно диалога
	{
		scrollRect.gameObject.SetActive(true);
	}

	int FindNodeByID(int i)
	{
		int j = 0;
		foreach(Dialogue d in node)
		{
			if(d.id == i) return j;
			j++;
		}
		return -1;
	}

	void BuildDialogue(int current)
	{
		ClearDialogue();

		int j = FindNodeByID(current);

		if(j < 0)
		{
			Debug.LogError(this + " в диалоге [" + fileName + ".xml] отсутствует или указан неверно идентификатор узла.");
			return;
		}
		for (int i = 0; i < node[j].npcText.Count; i++)
		{
            if (node[j].questValue.Count==0 || node[j].questValue[i]== valueStatus)
            {
				AddToList(false, 0, node[j].npcText[i], valueStatus, string.Empty, false); // добавление текста NPC
			}
		}


		switch (node[j].tipDialoga)//подсвечиваем в зависимости от типа диалога 
        {
			case "1":
				CreatCorol(new Color32(3, 72, 135, 255));
				break;
			case "2":
				CreatCorol(new Color32(20, 186, 181, 125));
				break;
			case "3":
				CreatCorol(new Color32(121, 140, 137, 255));
				break;
			default:
				CreatCorol(new Color(0, 0, 0));
				break;
		}
       
		for(int i = 0; i < node[j].answer.Count; i++)
		{
			int value = valueStatus;

			// фильтр ответов, относительно текущего статуса квеста
			if(value >= node[j].answer[i].questValueGreater && node[j].answer[i].questValueGreater != 0 || 
				node[j].answer[i].questValue == value && node[j].answer[i].questValueGreater == 0 || 
				node[j].answer[i].questName == null)
			{
				AddToList(node[j].answer[i].exit, node[j].answer[i].toNode, node[j].answer[i].text, node[j].answer[i].questStatus, node[j].answer[i].questName, true); // текст игрока
			}
		}

		EventSystem.current.SetSelectedGameObject(scrollRect.gameObject); // выбор окна диалога как активного, чтобы снять выделение с кнопок диалога
		ShowWindow();
	}

	

	int GetINT(string text)
	{
		int value;
		if(int.TryParse(text, out value))
		{
			return value;
		}
		return 0;
	}

	bool GetBOOL(string text)
	{
		bool value;
		if(bool.TryParse(text, out value))
		{
			return value;
		}
		return false;
	}
}
	
class Dialogue
{
	public int id;
	public List<string>  npcText;
	public List <int> questValue;
	public string tipDialoga;
	public List<Answer> answer;
}

class Answer
{
	public string text;
	public string questName;
	public int toNode;
	public int questValue;
	public int questValueGreater;
	public int questStatus;
	public bool exit;
}