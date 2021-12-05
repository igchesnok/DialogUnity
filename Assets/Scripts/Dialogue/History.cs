using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void onStatusÑhanged(string companion, int stat);
public static class History 
{
    public static event onStatusÑhanged OnStatusÑhanged;
    public static string NameCompanion { get; set; }
     private static int Status_=0 ;

    public static int Status
    {
        get
        {
            return Status_;    // âîçâğàùàåì çíà÷åíèå ñâîéñòâà
        }
        set
        {
            Status_ = value;   // óñòàíàâëèâàåì íîâîå çíà÷åíèå ñâîéñòâà
            if (OnStatusÑhanged != null)
            {
                OnStatusÑhanged(NameCompanion, value);
            }
        }
    }

  
}
