using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graveyard : MonoBehaviour
{
   public void UpdateGraveyardUI(){
    //    int multiplier = 0;
       for (int i = 0;i < transform.childCount; i++)
       {
           transform.GetChild(i).GetComponent<RectTransform>().localPosition = new Vector3(0,-i*40,0);

       }
   }
}
