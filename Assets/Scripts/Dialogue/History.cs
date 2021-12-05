using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void onStatus�hanged(string companion, int stat);
public static class History 
{
    public static event onStatus�hanged OnStatus�hanged;
    public static string NameCompanion { get; set; }
     private static int Status_=0 ;

    public static int Status
    {
        get
        {
            return Status_;    // ���������� �������� ��������
        }
        set
        {
            Status_ = value;   // ������������� ����� �������� ��������
            if (OnStatus�hanged != null)
            {
                OnStatus�hanged(NameCompanion, value);
            }
        }
    }

  
}
