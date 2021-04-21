using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
public class AI : MonoBehaviour
{
   public Piece piecetoMove;
   public Cell cellToMovePiece;
    public static void PlayRandomMove(){
        //Debug.Log("Play Random");
        //Definir l'ensemble des coups possibles
        GameManager.instance.DefineAllPossibleMoves();
        //On choisit au hazard un move
        int moveToPlayIndex = GameManager.instance.rand.Next(GameManager.instance.possibleMoves.Count);
       // Debug.Log("Move choisit : "+moveToPlayIndex);
        PossibleMove moveToPlay = GameManager.instance.possibleMoves[moveToPlayIndex];
        moveToPlay.cellWherePieceCanMove.SetIsPlayable(true);


        //on le joue
        GameManager.instance.MovePiece(moveToPlay.piece,moveToPlay.cellWherePieceCanMove,true,moveToPlay);

        GameManager.instance.CheckEndOfGame(moveToPlay.piece);

        
    }
    [SerializeField]
    private int depthToGo = 2;


    public void OnClickCountAllPossibleMovesButton(){

        StartCoroutine(CalculateAllPossibleMoves());
    }

    IEnumerator CalculateAllPossibleMoves(){
        possibleMoves1.Clear();
        Stopwatch stopwatch = new Stopwatch();
        UnityEngine.Debug.Log("Début du calcul");
        stopwatch.Start();
        
        int movesdepth = CountAllPossibleMoves(depthToGo);
        stopwatch.Stop();
        UnityEngine.Debug.Log("Timer time (ms): "+stopwatch.ElapsedMilliseconds);
        UnityEngine.Debug.Log("le nombre de coup possible en depth "+ depthToGo  +" est: "+movesdepth);
        GameManager.instance.UpdatePiecePositionUI();
        yield return new WaitForSeconds(1f);
    }
    private void Update() {
       
    }

    public void movepiecetoacell(){
        GameManager.instance.MovePiece(piecetoMove,cellToMovePiece,true);
    }
    
    public List<PossibleMove> possibleMoves1;
    
    public int CountAllPossibleMoves(int depth){
        if(depth == 0){
            return 1;
        }
         GameManager.instance.SetPlayerTurn();
         //Debug.Log("turn : "+GameManager.instance.turn);
        List<PossibleMove> moves = DefineAllPossibleMoves();


        int numPosition =0;

        foreach (PossibleMove move in moves)
        {
       
        //possibleMoves1.Add(move);
            
        move.cellWherePieceCanMove.SetIsPlayable(true);
        //on le joue
        GameManager.instance.MovePiece(move.piece,move.cellWherePieceCanMove,true,move);
        GameManager.instance.SetPlayerTurn();

        numPosition += CountAllPossibleMoves(depth -1);
        

        GameManager.instance.UndoMove(false);



// 
        }

        return numPosition;
    }
    // private void Update() {
        
    //      for (int i = 0; i < GameManager.instance.board.pieces.Count; i++)
    //      {
    //          if(GameManager.instance.board.pieces[i].isDead){
    //              continue;
    //          }
    //          GameManager.instance.PutAPieceToItsCell(GameManager.instance.board.pieces[i]);
         

    //      //UpdateCellUI();
    //  }
    // }

    public IEnumerator PlaySlownLyAllPossibleMoves(){
 
        for(int i = 0; i<possibleMoves1.Count;i++)
        {
            PossibleMove move = possibleMoves1[i];

            int tempcol = move.piece.column;
            int templine = move.piece.line;
            GameManager.instance.MoveAPieceOnASpecificCell(move.piece,move.cellWherePieceCanMove);
            //GameManager.instance.MovePiece(move.piece,move.cellWherePieceCanMove,true,move);
            GameManager.instance.UpdatePiecePositionUI();
            UnityEngine.Debug.Log("une seconde :");
            yield return new WaitForSeconds(0.2f);
            GameManager.instance.MoveAPieceOnASpecificCell(move.piece,GameManager.instance.GetCell(tempcol,templine));
            yield return new WaitForSeconds(0.5f);

            
        }
        
    }

    public void OnClickPlaySlowlyAllPossibleMovesButton(){
        StartCoroutine(PlaySlownLyAllPossibleMoves());
    }

    public List<PossibleMove> DefineAllPossibleMoves(){
        List<PossibleMove> possibleMoves = new List<PossibleMove>();

        for(int i =0 ; i<GameManager.instance.board.pieces.Count; i++){
           
                Piece piece = GameManager.instance.board.pieces[i];
                if(piece.isDead){
                    continue;
                }
                // Debug.Log("Piece team : "+piece.team);
                // Debug.Log("GameManager team : "+GameManager.instance.teamToPlay);
                if(piece.team == GameManager.instance.teamToPlay){ 
                    GameManager.instance.SetCellsNonPlayable();
                    
                    GameManager.instance.CalculatePlayableOrAttackedCells(piece,true,false);
                    // GameManager.instance.CalculatePlayableOrAttackedCells(piece,true,false);
                    for (int col = 0; col < 8; col++)
                    {
                        for (int line = 0; line < 8; line++)
                        {
                            if(GameManager.instance.GetCell(col,line).GetIsPlayable()){

                                if(piece.name == "Pawn" && ((piece.line == 0) || (piece.line == 7))){//Si promotion du pion
                                    PossibleMove possibleMoveQueen = new PossibleMove();
                                    possibleMoveQueen.cellWherePieceCanMove = GameManager.instance.GetCell(col,line);
                                    possibleMoveQueen.piece = piece;
                                     possibleMoveQueen.nameOfPieceToBecome = "Queen";
                                     possibleMoves.Add(possibleMoveQueen);
                                     
                                     PossibleMove possibleMoveTower = new PossibleMove();
                                    possibleMoveTower.cellWherePieceCanMove = GameManager.instance.GetCell(col,line);
                                    possibleMoveTower.piece = piece;
                                     possibleMoveTower.nameOfPieceToBecome = "Tower";
                                     possibleMoves.Add(possibleMoveTower);
                                     
                                     PossibleMove possibleMoveBishop = new PossibleMove();
                                possibleMoveBishop.cellWherePieceCanMove = GameManager.instance.GetCell(col,line);
                                possibleMoveBishop.piece = piece;
                                     possibleMoveBishop.nameOfPieceToBecome = "Bishop";
                                     possibleMoves.Add(possibleMoveBishop);

                                     PossibleMove possibleMoveKnight = new PossibleMove();
                                possibleMoveKnight.cellWherePieceCanMove = GameManager.instance.GetCell(col,line);
                                possibleMoveKnight.piece = piece;
                                     possibleMoveKnight.nameOfPieceToBecome = "Knight";
                                     possibleMoves.Add(possibleMoveKnight);
                                }
                                else{
                                    PossibleMove possibleMove = new PossibleMove();
                                possibleMove.cellWherePieceCanMove = GameManager.instance.GetCell(col,line);
                                possibleMove.piece = piece;
                                possibleMoves.Add(possibleMove); 
                                    
                                }

                            }
                        }
                    }
                        
                                                 

                                

                                
                            
                        
                    
                }
            
        }
        return possibleMoves;
    }

}
