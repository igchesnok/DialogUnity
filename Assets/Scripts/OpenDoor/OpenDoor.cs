using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class OpenDoor : MonoBehaviour {

	public float smooth = 1;//скорость открытия двери
	public float doorOpenAngle = 45.0f;//угол открытия двери положительное число открытие двери на себя  если петли справа иначе от себя
	public GameObject buttonOpenClous;//кнопка интерфейса открытия закрытия двери
	public GameObject buttonOpenClousText;//Text кнопки открытия закрытия двери
	
	private Text textButtonOpenClous;//текстовое поле кнопки открытия закрытия двери
	private bool audioEnd;//флаг завершения проигрования звука
	private bool doorOpen;//флаг завершения движения двери
	private bool open;// флаг взаимодействия игрока с дверью (нажата кнопка закрыть открыть дверь)
	private bool enter;//флаг  вхождения игрока в тригер срабатывания двери

	private bool jump360 = false;//флаг превышения абсолютного угла 360 градусов
	private bool equals360 = false;//флаг завершения подхода к 360 град

	private Vector3 defaultRot;//абсолютный исходный угол  в движке
	private Vector3 openRot;//абсолютный угол поворота в движке
	private Vector3 vectorNull;//вектор = 0
	private Vector3 vector359;//вектор = 359
	private Vector3 vector360;//вектор = 360
	
	//неоходимы для приближения к  углу перехода 0-360 при открытии и закрытии
	private Vector3 priblKzOpen;
	private Vector3 kzClous;
	private Vector3 kzOpen;
	private Vector3 inKzClous;

	private float rotOpen;//значение для приближения к  углу перехода 0-360 при открытии двери
	private float rotClous;//значение для приближения к  углу перехода 0-360 при закрытии двери
	
	private AudioClip[] audioOpenClous;

	void Start () {
		defaultRot = transform.eulerAngles;
		vectorNull = new Vector3(defaultRot.x, 0, defaultRot.z);
		vector359 = new Vector3(defaultRot.x, 359.9f, defaultRot.z);
		vector360 = new Vector3(transform.eulerAngles.x, 360, transform.eulerAngles.z);
		audioOpenClous = Resources.LoadAll<AudioClip>("FootSteps/door");
		textButtonOpenClous = buttonOpenClousText.GetComponents<Text>()[0];
		float povorot;
		if (doorOpenAngle != Math.Abs(doorOpenAngle))
		{
			povorot = defaultRot.y + Math.Abs(doorOpenAngle);
			if (povorot > 360)
			{
				jump360 = true;
				povorot -= 360;
			}
			rotOpen = 359.9f;
			rotClous = 0;
			priblKzOpen = vectorNull;
			kzClous = vector360;
			kzOpen = vectorNull;
			inKzClous = vector359;
		}
		else
        {
			if (defaultRot.y == 0) {
				defaultRot.y = 359.9f;
				transform.eulerAngles = vector359;
			}
			povorot = defaultRot.y - doorOpenAngle;
			if (povorot < 0)
			{
				jump360 = true;
				povorot =360+ povorot;
			}
			rotOpen = 0;
			rotClous = 359.9f ;
			priblKzOpen = vector359;
			kzClous = vectorNull;
			kzOpen = vector360;
			inKzClous = vectorNull;
		}

		openRot = new Vector3 (defaultRot.x, povorot, defaultRot.z);
	}

	void Update()
	{
		if (doorOpen == true) //проверка на завершение действия  открытия / закрытия двери
		{
			if (open)
			{
				DoorOpen360();// дверь открывается
			}
			else
			{
				if (transform.eulerAngles.y != defaultRot.y)
				{
					if (open == false)
					{
						DoorClous360();//дверь закрывается  
					}
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.F) && enter)
		{
			if (doorOpen == false)
			{
				open = !open;
				equals360 = false;
				doorOpen = true;//устанавливаем флаг начато действие с дверью
			}
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Player")
		{
			enter = true; //устанавливаем флаг  вхождения в тригер
			buttonOpenClous.gameObject.SetActive(true);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Player")
		{
			enter = false;//устанавливаем флаг  выхода из тригера
			buttonOpenClous.gameObject.SetActive(false);
		}
	}

	public void DoorOpenClous()
	{
		if (doorOpen == false)
		{
			open = !open;
			equals360 = false;
			doorOpen = true;//устанавливаем флаг начато действие с дверью
		}
	}

	void DoorOpen360()
	{
		if (audioEnd == false)
		{
			GetComponent<AudioSource>().PlayOneShot(audioOpenClous[1]);//проигрываем звук открытия двери
			audioEnd = true;

		}

		Text[] texst_ = buttonOpenClousText.GetComponents<Text>();
		texst_[0].text = "ЗАКРЫТЬ ДВЕРЬ";

		#region есть перехода через 360
		if (jump360 == true)
		{
			#region переход к конечному значению 
			if (equals360 == true)
			{
				transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
				if ((transform.eulerAngles.y >= openRot.y - 0.05) & (transform.eulerAngles.y <= openRot.y + 0.05))
				{
					transform.eulerAngles = openRot;
					equals360 = false;
					doorOpen = false;
				}
			}
			#endregion
			else
			{
				#region переход к переходному значению 0-360
				if (transform.eulerAngles.y != openRot.y)
				{
					if ((transform.eulerAngles.y >= rotOpen) & (transform.eulerAngles.y <= rotOpen + 0.1))
					{
						transform.eulerAngles = priblKzOpen;
						equals360 = true;
					}
					else
					{
						transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, kzClous, 0.3f);
					}
				}
				#endregion
			}
		}
		#endregion

		#region нет перехода через 360
		else
		{
			OpenDoorNo360(openRot, false);
		}
		#endregion

	}

	void DoorClous360()
	{
		if (audioEnd == true)
		{
			GetComponent<AudioSource>().PlayOneShot(audioOpenClous[0]);//проигрываем звук закрытия двери
			audioEnd = false;
		}

		#region есть перехода через 360
		Text[] texst_ = buttonOpenClousText.GetComponents<Text>();
		texst_[0].text = "ОТКРЫТЬ ДВЕРЬ";
		if (jump360 == true)
		{
			#region переход к конечному значению 
			if (equals360 == true)
			{
				if ((transform.eulerAngles.y >= defaultRot.y - 0.05) & (transform.eulerAngles.y <= defaultRot.y + 0.05))
				{
					transform.eulerAngles = defaultRot;
					equals360 = false;
					doorOpen = false;
				}
				transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaultRot, Time.deltaTime * smooth);
			}
			#endregion
			else
			{
				#region переход к переходному значению 0-360
				if (transform.eulerAngles.y != defaultRot.y)
				{
					if ((transform.eulerAngles.y >= rotClous) & (transform.eulerAngles.y < rotClous + 0.1))
					{
						transform.eulerAngles = inKzClous;
						equals360 = true;
						if (transform.eulerAngles.y == defaultRot.y)
						{
							equals360 = false;
							doorOpen = false;
						}
					}
					else
					{
						transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, kzOpen, 0.3f);
					}
				}
				#endregion
			}
		}
		#endregion

		#region нет перехода через 360
		else
		{
			OpenDoorNo360(defaultRot, true);

		}
		#endregion

	}
	
	void OpenDoorNo360(Vector3 GradRot,bool oop)//осуществляет поворот обьекта если нет перехода 0-360
	{
		transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, GradRot, Time.deltaTime * smooth);
		if ((transform.eulerAngles.y >= GradRot.y - 0.05) & (transform.eulerAngles.y <= GradRot.y + 0.05))
		{
			transform.eulerAngles = GradRot;
			doorOpen = false;
		}
	}

}