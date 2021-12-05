using System.Collections;
using System.Collections.Generic;
//using TouchControlsKit;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
	//[Header("������ ��� �����")]
	public Sprite sprite; //������  ��� ������������ ����� 
	public string tipObject; //��� �������
	public Sprite nullSprite; //������  ��� ������� �����
	public int numberObject; //����������� ��������

	public Image icon; //������, ���� ����� ������������� ������
	public Text number; //����, ����������� ��������
	public Text tip; //��� �������

	public void UpdateSlot(bool active) //���������� �����
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
