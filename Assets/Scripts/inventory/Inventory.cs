using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject objectCanvasInventory { get; set; }
    private Slot[] slots;
    private bool[] hasItems;  //Наличие предметов

    void Awake()
    {
        objectCanvasInventory = GameObject.Find("CanvasInventory");
        slots = objectCanvasInventory.GetComponentsInChildren<Slot>(); //Получение всех ячеек
        hasItems = new bool[slots.Length]; 
        objectCanvasInventory.SetActive(false);
    }

    public void Activate()
    {
        UpdateUI(); //Обновление интерфейса
        objectCanvasInventory.SetActive(!objectCanvasInventory.activeSelf);
    }

    public void AddItem(int index, InventoryObject objects)//активируем ячейки с предметами
    {
        slots[index].sprite = objects.iconItem[index];
        objects.iconItem[index] = null;
        slots[index].tipObject= objects.tipItem[index];
        objects.tipItem[index]="";
        slots[index].numberObject= slots[index].numberObject+ objects.numberItem[index];
        objects.numberItem[index]=0;
        hasItems[index] = true; //Добавление предмета при передаче
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++) //Проверка всех предметов
        {
            bool active = false;
            if (hasItems[i]) //Если такой предмет есть у пользователя, то он будет отображаться в слоте
            {
                active = true;
            }

            slots[i].UpdateSlot(active);
        }
    }
}

