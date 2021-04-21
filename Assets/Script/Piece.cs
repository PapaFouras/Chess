using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    public int id;
    public bool isHeld = false;
    public string pieceName;

    public int team;

    public bool isDead = false;

    public int column;
    public int line;

    public int move = 0;

    public bool hasAlreadyMoved = false;

        public int tempLine= 0;
    public int tempColumn = 0;

    public List<Cell> cellWherePieceCanMove = new List<Cell>();

public void PieceClicked(){

    if(!GameManager.instance.isPlayerAI[GameManager.instance.teamToPlay]){
        if(!isDead && !GameManager.instance.gameOver){
        if(!isHeld){
        if(GameManager.instance.teamToPlay == team){
            
            GameManager.instance.SetCellsNonPlayable();
               // Debug.Log("clicked "+pieceName+team);
            GameManager.instance.SetAllAttackedCells();
            GameManager.instance.CalculatePlayableOrAttackedCells(this,true,false);
            gameObject.transform.SetSiblingIndex(-1);
            tempLine = line;
            tempColumn = column;
            isHeld = true;

            GameManager.instance.UpdatePiecePositionUI();

        }
        else{
            return;
        }
     
        
    }

    GameManager.instance.SetPlayerTurn();
    GameManager.instance.UpdateCellUI();
    
        }
    }
    
    }

    
 public void SetUp(int _team, string _name, int _index){
    pieceName = _name;
    
    gameObject.name = pieceName;

    id = _index;

    team = _team;

    gameObject.GetComponent<Image>().sprite = 
            pieceName =="Tower"? Util.instance.towerSprite[team] :
            pieceName =="Bishop"? Util.instance.bishopSprite[team] :
            pieceName =="King"? Util.instance.kingSprite[team] :
            pieceName =="Queen"? Util.instance.queenSprite[team] :
            pieceName =="Knight"? Util.instance.knightSprite[team] :
            pieceName =="Pawn"? Util.instance.pawnSprite[team] : null;

        



 }

 private void Update() {
     if(Input.GetMouseButtonDown(0)){

     
     if(isHeld){
        isHeld = false;
        //Debug.Log("hello0 :"+gameObject.transform.position);

        gameObject.transform.position = Input.mousePosition;

        GameManager.instance.DoMove(this);

        //GameManager.instance.DefineAllPossibleMoves();
        
        
     }
     }
     
     
 }


}
