using System.Collections;
using System.Collections.Generic;
//using TouchControlsKit;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
	//[Header("Иконка для слота")]
	public Sprite sprite; //Спрайт  для заполненного слота 
	public string tipObject; //тип объекта
	public Sprite nullSprite; //Спрайт  для пустого слота
	public int numberObject; //колличества обьектов

	public Image icon; //Иконка, куда будет прикрепляться спрайт
	public Text number; //счет, колличества обьектов
	public Text tip; //тип обьекта

	public void UpdateSlot(bool active) //Обновление слота
	{
		if (active)
		{
			icon.sprite = sprite;
			number.text = numberObject.ToString() ;
			tip.text = tipObject;
		}
		else
		{
			icon.sprite = nullSprite;
			number.text = "0";
			tip.text = "";
		}
	}
}
