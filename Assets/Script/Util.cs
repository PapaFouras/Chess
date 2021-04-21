using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Util : MonoBehaviour
{
    public static Util instance;
     private void Awake() {
        if(instance != null){
            Debug.LogWarning("Il y a plus d'une instance de GameManager dans la scène");
            return;
        }
        instance = this;
        
    }
   
    [SerializeField]
    public Sprite[] kingSprite = new Sprite[2];
    public Sprite[] queenSprite = new Sprite[2];

    public Sprite[] bishopSprite = new Sprite[2];
    public Sprite[] knightSprite = new Sprite[2];
    public Sprite[] towerSprite = new Sprite[2];

    public Sprite[] pawnSprite = new Sprite[2];

    //https://www.chessprogramming.org/Perft_Results
    public static string position2 ="TnnnKnnTPPPFFPPPnnCnnQnpnpnnPnnnnnnPCnnnfcnnpcpnpnppqpfntnnnknnt";
    public static string position3 ="nnnnnnnnnnnnPnPnnnnnnnnnnTnnnpnkKPnnnnntnnnpnnnnnnpnnnnnnnnnnnnn";
    public static string position4 ="TnnQnTKnPpnPnnPPqnnnnCnnFFPnPnnncPnnnnnnnfnnncfCPpppnppptnnnknnt";
    public static string position5 ="TCFQKnnTPPPnCcPPnnnnnnnnnnFnnnnnnnnnnnnnnnpnnnnnppnPfppptcfqnknt";
    public static string position6 ="TnnnnTKnnPPnQPPPPnCPnCnnnnFnPnfnnnfnpnFnpncpncnnnppnqppptnnnntkn";


}
