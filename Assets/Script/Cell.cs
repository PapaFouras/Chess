using UnityEngine;
[System.Serializable]
public class Cell : MonoBehaviour
{

    public int id;

    public string cellName;
    public int columnNumber;
    public int lineNumber;
    private bool isPlayable = false;

    public bool[] isAttacked = new bool[2];

    [SerializeField]
    private bool pieceOnIt = false;

    public void SetPieceOnIt(bool _pieceOnIt){
        pieceOnIt = _pieceOnIt;
    }

    public bool GetPieceOnIt(){
        return pieceOnIt;
    }

    public bool GetIsPlayable(){
        return isPlayable;
    }

    public void SetIsPlayable(bool playable){
        isPlayable = playable;
    }
    
}
