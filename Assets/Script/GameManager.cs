using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Board board;

    public GameObject contour;
    public GameObject selectMode;
    public GameObject pawnPromotion;

    public GameObject victoryPanel;
    public GameObject victoryGameObject;
    public Text victoryText;


    public int turn = 1;
    public int teamToPlay = 0;

    public Piece lastMovedPiece;

     public List<Move> moves = new List<Move>();

     public int possibleMovesCount = 0;

  

     public bool gameOver = false;


     public bool[] isPlayerAI = new bool[2];

     public System.Random rand = new System.Random();

     public List<PossibleMove> possibleMoves = new List<PossibleMove>();

    public List<string> positions = new List<string>();


    

    private void Awake() {
        if(instance != null){
            Debug.LogWarning("Il y a plus d'une instance de GameManager dans la scène");
            return;
        }
        instance = this;
        
    }
    // Start is called before the first frame update
    void Start()
    {
       // StartCoroutine(PlayAtSpeed(0.0001f));
        StartCoroutine(UpdatePiecePositionUICoroutine(3f));
    }

    private void AIPlaysAsFastAsPossible(){
    bool isAIPlaying =true;
    while(isAIPlaying){
        //Debug.Log("ia plays");
        if(isPlayerAI[teamToPlay] && !gameOver){
            AI.PlayRandomMove();
            //isCalculating= false;
            // int movesdepth2 = AI.CountAllPossibleMoves(2);
            // Debug.Log("le nombre de coup possible en depth 2 est: "+movesdepth2);
        }
        else
        {
          isAIPlaying =false;  
        }
}
    }

    private IEnumerator PlayAtSpeed(float speed){
    bool isAIPlaying =true;
    while(isAIPlaying){
        // Debug.Log("ia plays");
        if(isPlayerAI[teamToPlay] && !gameOver){
            AI.PlayRandomMove();
            //isCalculating= false;
            // int movesdepth2 = AI.CountAllPossibleMoves(2);
            // Debug.Log("le nombre de coup possible en depth 2 est: "+movesdepth2);
        }else
        {
          isAIPlaying =false;  
        }
        yield return new WaitForSeconds(speed);
        }
        
    }
    private void Update() {

        //AIPlaysAsFastAsPossible();
        

    }

    public void OnClickPlayer(int _numberOfPlayers){
    
        if(_numberOfPlayers == 1){
            isPlayerAI[0]=  (rand.Next(2) == 1);
            isPlayerAI[1] = !isPlayerAI[0];
        }

        contour.SetActive(true);
        selectMode.SetActive(false);

    }

    public void OnClickSeeBoardButton(){
        if(victoryPanel.activeSelf)
        victoryPanel.SetActive(false);
        else
        victoryPanel.SetActive(true);
    }

    public void PlaySmartMove(){
        //Debug.Log("Play Random");
        //Definir l'ensemble des coups possibles
        DefineAllPossibleMoves();
        //On choisit le best move (TODO)



        int moveToPlayIndex = rand.Next(possibleMoves.Count);

// fin du TODO


       // Debug.Log("Move choisit : "+moveToPlayIndex);
        PossibleMove moveToPlay = possibleMoves[moveToPlayIndex];
        moveToPlay.cellWherePieceCanMove.SetIsPlayable(true);


        //on le joue
        MovePiece(moveToPlay.piece,moveToPlay.cellWherePieceCanMove,true,moveToPlay);

        CheckEndOfGame(moveToPlay.piece);

        
    }


    public void SetPlayerTurn(){
        if(turn%2 == 1){
            teamToPlay = 0;
        }
        else{
            teamToPlay = 1;
        }

    }

    private bool Between0and8(int number){
        if(number >= 0 && number < 8)
            return true;

        return false;

    }

    
    public void UndoMoveUntilAPlayerCanPlay(){
        UndoMove(false);
        if(isPlayerAI[teamToPlay]){
            UndoMove(false);

        }
    }
    
    public void UndoMove(bool isCheckingKingChecked){

        if(victoryGameObject.activeSelf == true){
            victoryGameObject.SetActive(false);
        }
        
        if(!(moves.Count > 0)){
            Debug.Log("Undo Impossible");
            return;
        }
        gameOver = false;
        Move lastMove = moves[moves.Count -1];
        MoveAPieceOnASpecificCell(lastMove.pieceMoved,lastMove.originalCell);

        if(lastMove.pieceKilled != null){
        lastMove.pieceKilled.transform.SetParent(board.transform,false);
        lastMove.pieceKilled.isDead = false;
        MoveAPieceOnASpecificCell(lastMove.pieceKilled,GetCell(lastMove.pieceKilled.column,lastMove.pieceKilled.line));

        }

        if(lastMove.hasBeenPromoted){
            SetNameAndSpriteOnPiece("Pawn",lastMove.pieceMoved);
        }

        if(lastMove.towerRocked != null){
            MoveAPieceOnASpecificCell(lastMove.towerRocked,lastMove.cellFromWhereTowerCastled);
        }
        

        lastMove.pieceMoved.move -=1;
        if(lastMove.pieceMoved.move == 0){
            lastMove.pieceMoved.hasAlreadyMoved = false;
        }
        turn --;

        SetPlayerTurn();
        moves.Remove(lastMove);
        if(moves.Count >0){
            lastMovedPiece = moves[moves.Count-1].pieceMoved;
        }
        if(!isCheckingKingChecked){
            positions.Remove(positions[positions.Count - 1]);
        }
        
        //TO PUT UpdateCellUI();
        

    }

    private void  CheckKingAttacked(Piece piece,Cell cellToMove){

        // // //Simuler le coup
        piece.isHeld = false;
        //PlaceInCenterOfNearestCell(piece);
        MovePiece(piece,cellToMove,false,null,true);   

       

        // // //Calculer les cases attaquées par l'adversaire
        SetPieceOnCells();

        SetAllAttackedCells();

        // TO PUT UpdateCellUI();

        // Debug.LogError("AHAHAH");
        
        

        bool kingAttacked = false;
        //Check que notre roi n'est pas attaqué
        if(isKingChecked(piece.team)){
        
        kingAttacked = true;   
        }
        else{
        kingAttacked = false;
        }  

        UndoMove(true);
        

        cellToMove.SetIsPlayable(!kingAttacked);
      
    }

    public Cell GetCell(int col, int line){
        if(!Between0and8(col))
        {
            Debug.LogError("Colonne demandée pas entre 0 et 8 : "+col);
        }
        if(!Between0and8(line))
        {
            Debug.LogError("Ligne demandée pas entre 0 et 8 : "+ line);
        }

        return board.cells[col,line];
    }


    public void CalculatePlayableOrAttackedCells(Piece piece, bool playable, bool attacked){
        int direction = piece.team == 0 ? 1 : -1; // Si blanc ou noir
        #region Pawn
        if(piece.pieceName == "Pawn"){
            
                
                if(Between0and8(piece.line + direction)){ // Avancer tout droit
                    Cell cell = GetCell(piece.column,piece.line + direction);
                    if(!cell.GetPieceOnIt()){
                        if(playable){

                            // bool isOK = CheckKingAttacked(piece,piece.column,piece.line + direction);


                            cell.SetIsPlayable(true);
                            CheckKingAttacked(piece,cell);
                            
                            // GetCell(piece.column,piece.line + direction).SetIsPlayable(true);
                           
                            
                            



                        }
                        

                    }

                    if(!piece.hasAlreadyMoved && Between0and8(piece.line + 2 * direction)){ // avancer 2 fois si premier mouvement

                    if(!GetCell(piece.column,piece.line + 2 * direction).GetPieceOnIt() && !GetCell(piece.column,piece.line + 1 * direction).GetPieceOnIt()){
                        if(playable){
                            // bool isOK = CheckKingNotAttacked(piece,piece.column,piece.line + direction);
                            // GetCell(piece.column,piece.line + 2 * direction).SetIsPlayable(!CheckKingAttacked(piece,GetCell(piece.column,piece.line + 2 * direction)));
                            // GetCell(piece.column,piece.line + 2 * direction).SetIsPlayable(true);
                            GetCell(piece.column,piece.line + 2 * direction).SetIsPlayable(true);
                            //GetCell(piece.column,piece.line + 2 * direction).SetIsPlayable(!CheckKingAttacked(piece,GetCell(piece.column, piece.line + 2 * direction)));
                            CheckKingAttacked(piece,GetCell(piece.column,piece.line + 2 * direction));


                        }
                   
                    }

                 
                    }
                }

                if(Between0and8(piece.column - 1) && Between0and8(piece.line + direction)){
                    Cell cell = GetCell(piece.column-1,piece.line +direction);
                    if(attacked)
                    cell.isAttacked[piece.team] = true;
                    if(cell.GetPieceOnIt()){ // Si case avant gauche avec piece dessus ==> true
                        if(GetPieceOnCell(cell).team != piece.team){
                            if(playable){
                                cell.SetIsPlayable(true);
                                CheckKingAttacked(piece,cell);
                            }
                            

                            

                        }
                    }
                }
                if(Between0and8(piece.column + 1) && Between0and8(piece.line + direction)){

                    if(attacked)
                    GetCell(piece.column+1,piece.line +direction).isAttacked[piece.team] = true;
                    if(GetCell(piece.column+1,piece.line +direction).GetPieceOnIt()){ // Si case avant droite avec piece dessus ==> true
                        
                        if(GetPieceOnCell(GetCell(piece.column+1,piece.line +direction)).team != piece.team){
                            if(playable){
                                GetCell(piece.column+1,piece.line +direction).SetIsPlayable(true);
                                CheckKingAttacked(piece,GetCell(piece.column+1,piece.line +direction));
                            }
                            

                            

                        }
                        
                    
                    }

                    
                }

                //prise en passant
                CheckPriseEnPassant(piece,piece.column -1,piece.line,direction,playable,attacked);
                CheckPriseEnPassant(piece,piece.column +1,piece.line,direction,playable,attacked);

                
                   
        }
        #endregion
        #region Tower
        if(piece.pieceName == "Tower"){

            CheckTowerCellAvailable(piece,"line","up",playable,attacked);
            CheckTowerCellAvailable(piece,"line","down",playable,attacked);
            CheckTowerCellAvailable(piece,"column","up",playable,attacked);
            CheckTowerCellAvailable(piece,"column","down",playable,attacked);
            
        }
        #endregion

        #region Bishop
        if(piece.pieceName == "Bishop"){

            CheckBishopCellAvailable(piece,"line","up",playable,attacked);
            CheckBishopCellAvailable(piece,"line","down",playable,attacked);
            CheckBishopCellAvailable(piece,"column","up",playable,attacked);
            CheckBishopCellAvailable(piece,"column","down",playable,attacked);
        }

        #endregion

        #region Queen
        if(piece.pieceName == "Queen"){

            CheckBishopCellAvailable(piece,"line","up",playable,attacked);
            CheckBishopCellAvailable(piece,"line","down",playable,attacked);
            CheckBishopCellAvailable(piece,"column","up",playable,attacked);
            CheckBishopCellAvailable(piece,"column","down",playable,attacked);
            CheckTowerCellAvailable(piece,"line","up",playable,attacked);
            CheckTowerCellAvailable(piece,"line","down",playable,attacked);
            CheckTowerCellAvailable(piece,"column","up",playable,attacked);
            CheckTowerCellAvailable(piece,"column","down",playable,attacked);
        }

        #endregion

        #region  King

        if(piece.pieceName == "King"){

            //deplacement simple
            
            CheckCellAvailable(piece,piece.column-1,piece.line-1,playable,attacked);
            CheckCellAvailable(piece,piece.column,piece.line-1,playable,attacked);
            CheckCellAvailable(piece,piece.column+1,piece.line-1,playable,attacked);
            CheckCellAvailable(piece,piece.column-1,piece.line,playable,attacked);
            CheckCellAvailable(piece,piece.column,piece.line,playable,attacked);
            CheckCellAvailable(piece,piece.column+1,piece.line,playable,attacked);            
            CheckCellAvailable(piece,piece.column-1,piece.line+1,playable,attacked);
            CheckCellAvailable(piece,piece.column,piece.line+1,playable,attacked);
            CheckCellAvailable(piece,piece.column+1,piece.line+1,playable,attacked);

            //Roque
            if(!piece.hasAlreadyMoved){
                //petit roque
                if(Between0and8(piece.column+3)){
                    if(GetCell(piece.column +1,piece.line).GetPieceOnIt() == false && GetCell(piece.column +1,piece.line).isAttacked[Mathf.Abs(piece.team - 1)] == false){
                        if(GetCell(piece.column +2,piece.line).GetPieceOnIt() == false && GetCell(piece.column + 2,piece.line).isAttacked[Mathf.Abs(piece.team - 1)] == false){
                            if(GetCell(piece.column +3,piece.line).GetPieceOnIt() == true){
                                Piece pieceOnCell = GetPieceOnCell(GetCell(piece.column+3,piece.line));
                                if(pieceOnCell.name == "Tower" && !pieceOnCell.hasAlreadyMoved&& pieceOnCell.team == piece.team){
                                    if(playable)
                                    GetCell(piece.column+2,piece.line).SetIsPlayable(true);
                                    if(attacked)
                                    GetCell(piece.column+2,piece.line).isAttacked[piece.team] = true;

                                }
                            }
                           
                        }
                    }

                }

                //grand roque

                if(Between0and8(piece.column-4)){
                    if(GetCell(piece.column -1,piece.line).GetPieceOnIt() == false && GetCell(piece.column -1,piece.line).isAttacked[Mathf.Abs(piece.team - 1)] == false){
                        if(GetCell(piece.column -2,piece.line).GetPieceOnIt() == false && GetCell(piece.column -2,piece.line).isAttacked[Mathf.Abs(piece.team - 1)] == false){
                            if(GetCell(piece.column -3,piece.line).GetPieceOnIt() == false && GetCell(piece.column -3,piece.line).isAttacked[Mathf.Abs(piece.team - 1)] == false){
                                if(GetCell(piece.column -4,piece.line).GetPieceOnIt() == true){
                                    Piece pieceOnCell = GetPieceOnCell(GetCell(piece.column-4,piece.line));
                                    if(pieceOnCell.name == "Tower" && !pieceOnCell.hasAlreadyMoved && pieceOnCell.team == piece.team){
                                        if(playable)
                                        GetCell(piece.column-2,piece.line).SetIsPlayable(true);
                                        if(attacked)
                                        GetCell(piece.column-2,piece.line).isAttacked[piece.team] = true;
                                    }
                                }

                            }                           
                           
                        }
                    }

                }


            }

                                
            
        }

        #endregion

        #region Knight
        if(piece.pieceName == "Knight"){

            CheckCellAvailable(piece, piece.column -1, piece.line +2,playable,attacked);
            CheckCellAvailable(piece, piece.column +1, piece.line +2,playable,attacked);
            CheckCellAvailable(piece, piece.column -1, piece.line -2,playable,attacked);
            CheckCellAvailable(piece, piece.column +1, piece.line -2,playable,attacked);
            CheckCellAvailable(piece, piece.column -2, piece.line +1,playable,attacked);
            CheckCellAvailable(piece, piece.column +2, piece.line +1,playable,attacked);
            CheckCellAvailable(piece, piece.column -2, piece.line -1,playable,attacked);
            CheckCellAvailable(piece, piece.column +2, piece.line -1,playable,attacked);



        }

        #endregion


        
        
        }

    private void CheckPriseEnPassant(Piece piece,int col,int line, int direction, bool playable, bool attacked){
        if(Between0and8(col ) && Between0and8(line + direction)){ 
                    if(GetCell(col, line).GetPieceOnIt()){

                            Piece pawn = GetPieceOnCell(GetCell(col,line));
                            if(isKillPossibleEnPassant(pawn)){
                                if(playable){
                                GetCell(col,line + direction).SetIsPlayable(true);
                                CheckKingAttacked(piece,GetCell(col,line + direction));

                                }
                                if(attacked)
                                GetCell(col,line + direction).isAttacked[piece.team] = true;
                        }
                    }
                }

    }
       public bool isKingChecked(int team){
        for(int i =0 ; i<board.pieces.Count; i++){
            
                Piece thisPiece = board.pieces[i];
                if(thisPiece.name == "King" && thisPiece.team == team){
                        //Debug.Log("Le roi "+ (team == 0 ? "blanc":"noir") );
                    if(GetCell(thisPiece.column,thisPiece.line).isAttacked[Mathf.Abs(team-1)]){
                        //Debug.Log("Le roi "+ (team == 0 ? "blanc":"noir") + " est en échec !" );
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                
            }
        }
        Debug.LogError("King pas présent dans la scène");
        return false;
    }
    
    private bool isKillPossibleEnPassant(Piece pawn){
        return pawn == lastMovedPiece && pawn.name =="Pawn" && pawn.move == 1 && (pawn.line == 3 || pawn.line == 4);
    }

    public void KillPawnEnPassant(int col,int line,Move _currentMove){
        if(!Between0and8(col)){
            return;
        }
        if(GetCell(col,line).GetPieceOnIt()){
                            Piece pawn = GetPieceOnCell(GetCell(col,line));
                            if(isKillPossibleEnPassant(pawn) && _currentMove.cellWherePieceMoved.columnNumber == pawn.column){
                                GetCell(pawn.column,pawn.line).SetPieceOnIt(false);
                                PieceDie(pawn,_currentMove);
                            }
                        }
    }

    public Piece GetPawnKilledEnPassant(int col, int line){
        if(!Between0and8(col)){
            return null;
        }
        if(GetCell(col,line).GetPieceOnIt()){
                            Piece pawn = GetPieceOnCell(GetCell(col,line));
                            if(isKillPossibleEnPassant(pawn)){
                                GetCell(pawn.column,pawn.line).SetPieceOnIt(false);
                               return pawn;
                            }
                        }
        return null;

    }
    private void CheckCellAvailable(Piece piece,int col, int line, bool playable, bool attacked){
            if(Between0and8(col)&&Between0and8(line)){
                if(!GetCell(col,line).GetPieceOnIt()){
                    if(playable){
                        GetCell(col,line).SetIsPlayable(true);
                        CheckKingAttacked(piece,GetCell(col,line));
                    }
                    if(attacked)
                    GetCell(col,line).isAttacked[piece.team] = true;
                }
                else{
                    if(GetPieceOnCell(GetCell(col,line)).team != piece.team){
                        if(playable){
                            GetCell(col,line).SetIsPlayable(true);
                            CheckKingAttacked(piece,GetCell(col,line));

                        }
                        if(attacked)
                        GetCell(col,line).isAttacked[piece.team] = true;
                        
                    }
                }


            }
        }

     
    private void CheckBishopCellAvailable(Piece piece,string direction, string sens, bool playable, bool attacked){

            int column = piece.column;
            int line = piece.line;
             if(direction == "line" && sens == "up"){
                column ++;
                line ++;
            }
            else if(direction == "line" && sens == "down"){
                column --;
                line ++;
            }
            else if(direction == "column" && sens == "up"){
                column ++;
                line --;
            }
            else if(direction == "column" && sens == "down"){
                column --;
                line --;
            }
            
            bool pieceOnCell = false;
            // if(Between0and8(column) && Between0and8(line)){

            //     if(GetCell(column,line).GetPieceOnIt()){
            //     pieceOnCell = true;
            // }
            // }
           // bool kingChecked = false;


            while(Between0and8(column) && Between0and8(line) && !pieceOnCell){

            if(playable){
                GetCell(column,line).SetIsPlayable(true);
                //CheckKingAttacked(piece,GetCell(column,line));
                //kingChecked = CheckKingAttacked(piece,column,line);

            }
            bool checkKingNeeded = true;

            if(attacked)
            GetCell(column,line).isAttacked[piece.team] = true;
            
                if(GetCell(column,line).GetPieceOnIt()){
                    if(GetPieceOnCell(GetCell(column,line)).team == piece.team){
                        if(playable){
                            checkKingNeeded = false;
                            GetCell(column,line).SetIsPlayable(false);
                        }
                        if(attacked)
                        GetCell(column,line).isAttacked[piece.team] = false;

                    }

                    pieceOnCell = true;
                    // if(GetCell(column,line).GetIsPlayable() == true){
                    //     GetCell(column,line).SetIsPlayable(!kingChecked);
                    // }

                }

                if(checkKingNeeded && playable)
                        CheckKingAttacked(piece,GetCell(column,line));

                
                if(direction == "line" && sens == "up"){
                    column ++;
                    line ++;
                }
                else if(direction == "line" && sens == "down"){
                    column --;
                    line ++;
                }
                else if(direction == "column" && sens == "up"){
                    column ++;
                    line --;
                }
                else if(direction == "column" && sens == "down"){
                    column --;
                    line --;
                }
                
            }

    }
    
    private void CheckTowerCellAvailable(Piece piece,string direction, string sens, bool playable, bool attacked){
            
            int column = piece.column;
            int line = piece.line;

            if(direction == "line" && sens == "up"){
                column +=1;
            }
            else if(direction == "line" && sens == "down"){
                column -=1;
            }
            else if(direction == "column" && sens == "up"){
                line +=1;
            }
            else if(direction == "column" && sens == "down"){
                line -=1;
            }

            bool pieceOnCell = false;
            // if(Between0and8(column) && Between0and8(line)){

            //     if(GetCell(column,line).GetPieceOnIt()){
            //     pieceOnCell = true;
            // }
            // }
            

            
            //bool kingChecked = false;

            while(Between0and8(column) && Between0and8(line) && !pieceOnCell){

            if(playable){
                GetCell(column,line).SetIsPlayable(true);
                // CheckKingAttacked(piece,GetCell(column,line));
                //kingChecked = CheckKingAttacked(piece,column,line);
            }
            bool checkKingNeeded = true;
            
            if(attacked)
            GetCell(column,line).isAttacked[piece.team] = true;
            if(GetCell(column,line).GetPieceOnIt()){
                if(GetPieceOnCell(GetCell(column,line)).team == piece.team){
                    if(playable){
                        GetCell(column,line).SetIsPlayable(false);
                        checkKingNeeded = false;
                    }
                    if(attacked)
                    GetCell(column,line).isAttacked[piece.team] = false;

                }

                        
               

                pieceOnCell = true;

                }

                 if(checkKingNeeded && playable)
                CheckKingAttacked(piece,GetCell(column,line));

            // if(GetCell(column,line).GetIsPlayable()==true){
            //     GetCell(column,line).SetIsPlayable(!kingChecked);
            // }

            if(direction == "line" && sens == "up"){
            column ++;
            }
            else if(direction == "line" && sens == "down"){
                column --;
            }
            else if(direction == "column" && sens == "up"){
                line ++;
            }
            else if(direction == "column" && sens == "down"){
                line --;
            }
            
               
            }
    }
    public Piece GetPieceOnCell(Cell cell){
        for (var i = 0; i < board.pieces.Count; i++)
        {
            Piece piece = board.pieces[i];
                if(piece.isDead){
                    continue;
                }
                if(piece.column == cell.columnNumber && piece.line == cell.lineNumber){
                    return piece;
                
            }
        }
        Debug.LogError("GetPieceOnCell pas de Piece sur la cell column: "+cell.columnNumber+" line:"+cell.lineNumber);
        return null;
    }
    private void DestroyAllUIChildren(){
        for (int col = 0; col < 8; col++)
        {
            for (int line = 0; line < 8; line++)
            { 
                for (var i = 0; i < board.cells[col,line].transform.childCount; i++)
                {
                    Destroy(board.cells[col,line].transform.GetChild(i).gameObject);
                }
            }
        }
    }

    public void SetCellsNonPlayable(){
         for (int col = 0; col < 8; col++)
        {
            for (int line = 0; line < 8; line++)
            {   
                if(board.cells[col,line].GetIsPlayable()){
                    board.cells[col,line].SetIsPlayable(false);
                    for (var i = 0; i < board.cells[col,line].transform.childCount; i++)
                    {
                        

                        Destroy(board.cells[col,line].transform.GetChild(i).gameObject);
                    }
                }
            
            }
        }
    }

    public void SetCellsNonAttackable(){
         for (int col = 0; col < 8; col++)
        {
            for (int line = 0; line < 8; line++)
            {
                for(int team = 0; team < 2 ; team++){
                    if(board.cells[col,line].isAttacked[team]){
                    board.cells[col,line].isAttacked[team] = false;
                    
                }
                }                   
            }
        }
    }

    public void SetCellsNoNPieceOnIt(){
         for (int col = 0; col < 8; col++)
        {
            for (int line = 0; line < 8; line++)
            {
                for(int team = 0; team < 2 ; team++){
                    if(board.cells[col,line].GetPieceOnIt()){
                    board.cells[col,line].SetPieceOnIt(false);      
                }
                }                   
            }
        }
    }

    public void PieceDie(Piece deadPiece,Move _currentMove){

        deadPiece.isDead = true;
        GameObject graveyardGO = null;
        if(deadPiece.team == 0){
            graveyardGO = board.whiteGraveyard;
        }
        else{
            graveyardGO = board.blackGraveyard;
        }
        _currentMove.pieceKilled = deadPiece;
        Graveyard graveyard = graveyardGO.GetComponent<Graveyard>();
        deadPiece.transform.SetParent(graveyardGO.transform,false);
        graveyard.UpdateGraveyardUI();

    }



    public void MovePiece(Piece piece,Cell cellToMove,bool isAIplaying,PossibleMove moveToDo = null, bool isCheckingKingChecked = false){
            //Cell cellToMove = GetNearestCellTransform(piece.transform).GetComponent<Cell>();
            //Cell cellToMove = GetCell(col,line);
        
            if(piece.column != cellToMove.columnNumber || piece.line != cellToMove.lineNumber){ // Si la piece a bougé
       
                if(cellToMove.GetIsPlayable()){
                           
               
                
                Move currentMove = new Move();
                currentMove.turn = turn;
                currentMove.pieceMoved = piece;
                currentMove.cellWherePieceMoved = cellToMove;
                currentMove.originalCell = GetCell(piece.column,piece.line);

               

                    //Manger une piece sur la même case

                    if(cellToMove.GetPieceOnIt()){

                        Piece deadPiece =  GetPieceOnCell(cellToMove);
                        PieceDie(deadPiece,currentMove);
                    }

                    //Prise en passant(TODO)
                    if(piece.name == "Pawn"){
                        if(cellToMove.columnNumber != piece.column){
                            //Get  et tue la piece à gauche si elle existe 
                            KillPawnEnPassant(piece.column - 1,piece.line,currentMove);

                            //Get  et tue la piece à droite si elle existe 
                            KillPawnEnPassant(piece.column + 1,piece.line,currentMove);

                        }

                        
                        
                    }

                    //Roque (
                    if(piece.name == "King"){
                        
                            if(piece.column - cellToMove.columnNumber == 2){ // grand roque
                            Piece tower = GetPieceOnCell(GetCell(piece.column -4,piece.line)); //Can only be a tower
                            //Debug.LogError(tower);

                             if(tower.name == "Tower"){
                                currentMove.towerRocked = tower;
                                currentMove.cellFromWhereTowerCastled = GetCell(tower.column,tower.line);
                                MoveAPieceOnASpecificCell(tower,GetCell(tower.column+3,tower.line));
                                  
                                }

                        }else 
                            if(piece.column - cellToMove.columnNumber == -2){ // petit roque
                            Piece tower = GetPieceOnCell(GetCell(piece.column +3,piece.line)); //Can only be a tower
                            //Debug.LogError(tower);

                             if(tower.name == "Tower"){
                                currentMove.towerRocked = tower;
                                currentMove.cellFromWhereTowerCastled = GetCell(tower.column,tower.line);
                                MoveAPieceOnASpecificCell(tower,GetCell(tower.column-2,tower.line));      
                             }       
                        }

                        
                        
                       
                    }


                    turn ++;
                    piece.move ++;
                    if(!piece.hasAlreadyMoved){
                    piece.hasAlreadyMoved = true;
                    }
                    if(!isCheckingKingChecked){
                        lastMovedPiece = piece;
                    }

                //      if(!isCheckingKingChecked){
                //     Debug.Log(" bef piece line: "+piece.line);
                //     Debug.Log("bef piece col "+piece.column);
                //     Debug.Log("bef cell line "+cellToMove.lineNumber);
                //     Debug.Log("bef cell col "+cellToMove.columnNumber);

                // }

                            
                    
                    //Realiser le déplacement
                    MoveAPieceOnASpecificCell(piece,cellToMove);



                //      if(!isCheckingKingChecked){
                //     Debug.Log("aft piece line: "+piece.line);
                //     Debug.Log("aft piece col "+piece.column);
                //     Debug.Log("aft cell line "+cellToMove.lineNumber);
                //     Debug.Log("aft cell col "+cellToMove.columnNumber);

                // }


                    

                    //Ajout du move 
                    moves.Add(currentMove);

                    if(PawnPromotionCondition(piece)){
                        if(!isCheckingKingChecked){
                            PawnPromotion(piece,isAIplaying,moveToDo);
                        }
                        
                    }

                    if(!isCheckingKingChecked){
                         positions.Add(GetPosition());
                    }
                   
                    

                    
                    
                 

                
                }
                else{
                   // TO PUT PlaceInOriginalCell(piece);
                }
            
           
            }else{
                // TO PUT PlaceInOriginalCell(piece);

                
            }

            
            
            
    }

    public void OnClickedPawnPromotionClicked(string name){
        pawnPromotion.SetActive(false);
        //Get the pawn
        int pieceIndex = 0;

            for (int i = 0 ; i < board.pieces.Count ; i++ )
            {

                Piece temppiece = board.pieces[i];
                Debug.Log(temppiece.name+" "+temppiece.column+" "+temppiece.line);

                if(PawnPromotionCondition(temppiece)){
                    pieceIndex = i;
                    i=board.transform.childCount;
                
            }

            }

        //Lui attribuer les bonnes valeurs
        Piece piece = board.pieces[pieceIndex];
        SetNameAndSpriteOnPiece(name,piece);        

        // l'enregistrer dans les move

        moves[moves.Count-1].hasBeenPromoted = true;
        CheckEndOfGame(piece);
        UpdatePiecePositionUI();

    }

    public void SetNameAndSpriteOnPiece(string _name, Piece piece){
        if(_name == "Queen"){
            piece.name = "Queen";
            piece.pieceName = "Queen";
            piece.gameObject.GetComponent<Image>().sprite = Util.instance.queenSprite[piece.team];
        }
        else if(_name == "Tower"){
            piece.name = "Tower";
            piece.pieceName = "Tower";
            piece.gameObject.GetComponent<Image>().sprite = Util.instance.towerSprite[piece.team];
        }
        else if(_name == "Bishop"){
            piece.name = "Bishop";
            piece.pieceName = "Bishop";
            piece.gameObject.GetComponent<Image>().sprite = Util.instance.bishopSprite[piece.team];
        }
        else if(_name == "Knight"){
            piece.name = "Knight";
            piece.pieceName = "Knight";
            piece.gameObject.GetComponent<Image>().sprite = Util.instance.knightSprite[piece.team];
        }
        else if(_name == "Pawn"){
            piece.name = "Pawn";
            piece.pieceName = "Pawn";
            piece.gameObject.GetComponent<Image>().sprite = Util.instance.pawnSprite[piece.team];
        }
        else{
            Debug.LogError("le nom de la promotion est erroné: "+_name);
        }
    }
    private void PawnPromotion(Piece _piece,bool isAIplaying,PossibleMove moveToDo){
        Debug.Log("le pion va être promu");
        if(isAIplaying){
            //Debug.LogError("le moveToDo est null :"+(moveToDo == null));
            Debug.Log("le pion va être promu"+moveToDo.nameOfPieceToBecome);
            SetNameAndSpriteOnPiece(moveToDo.nameOfPieceToBecome,_piece);
            moves[moves.Count-1].hasBeenPromoted = true;
        }
        else{
            pawnPromotion.SetActive(true);
        }
    }

    private bool PawnPromotionCondition(Piece _piece){
        if(_piece.name == "Pawn" && ((_piece.line == 0) || (_piece.line == 7))){
            return true;
        }
        return false;
    }


    private void PlaceInOriginalCell(Piece piece){
            piece.transform.position = GetNearestCellTransform(GetCell(piece.column,piece.line).transform).position;
    }

    public Transform GetNearestCellTransform(Transform tr){

        int indexClosest = 0;
        float dist = 1000.03f;
        Cell closestCell = board.cells[0,0];
        for (var i = 0; i < 64; i++)
        {
            Cell cell = board.cells[i/8,i%8];
    
            


            float distancefromChild = Vector3.Distance(cell.transform.position,tr.position);

            if(distancefromChild < dist){
                indexClosest = i;
                dist = distancefromChild;
                closestCell = cell;
            }

            
            
        }

        return closestCell.transform;

    }

    public void PutAPieceToItsCell(Piece piece){
        if(piece.isDead){
            return;
        }
        Cell cell = GetCell(piece.column,piece.line);
        piece.transform.position = cell.transform.position;
    }


    public void MoveAPieceOnASpecificCell(Piece piece, Cell cell){
        
        GetCell(piece.column,piece.line).SetPieceOnIt(false);
        piece.column = cell.columnNumber;
        piece.line = cell.lineNumber;
       


        // TO PUT piece.transform.position = cell.transform.position;

      
        cell.SetPieceOnIt(true);
    }
    public void PlaceInCenterOfNearestCell(Piece piece){

        if(piece.isDead){
            return;
        }
        
        int originalColumn = piece.column;
        int originalLine = piece.line;
       
        Transform closestChild = GetNearestCellTransform(piece.transform);

        piece.transform.position = closestChild.position;
        piece.column = closestChild.GetComponent<Cell>().columnNumber;
        piece.line = closestChild.GetComponent<Cell>().lineNumber;
        closestChild.GetComponent<Cell>().SetPieceOnIt(true);

    }

    public void SetAllAttackedCells(){
        GameManager.instance.SetCellsNonAttackable();
        for (int i = 0 ; i < board.pieces.Count ; i++ )
        {
    
                Piece actualPiece = board.pieces[i];
                if(actualPiece.isDead){
                    continue;
                }
               // if(actualPiece.team == teamToPlay)
                CalculatePlayableOrAttackedCells(actualPiece,false,true);
            
        }
    }


public void SetPieceOnCells(){
    SetCellsNoNPieceOnIt();
    for (int i = 0 ; i < board.pieces.Count ; i++ )
        {
            //GetCell(board.transform.GetChild(i).GetComponent<Piece>().column,board.transform.GetChild(i).GetComponent<Piece>().line).SetPieceOnIt(false);
                Piece piece = board.pieces[i];
                if(piece.isDead){
                    continue;
                }
                GetCell(piece.column,piece.line).SetPieceOnIt(true);
            
        }

}
    



    public void UpdateCellUI(){

        DestroyAllUIChildren();
         for (int col = 0; col < 8; col++)
        {
            for (int line = 0; line < 8; line++)
            {
            if(board.cells[col,line].GetIsPlayable()){
                GameObject go = new GameObject();
                go.AddComponent<Image>();
                go.GetComponent<Image>().color = board.selectedCellColor;
                go.name = col + " " + line;
                go.transform.SetParent(board.cells[col,line].transform,false);
            }
            if(board.cells[col,line].isAttacked[Mathf.Abs(teamToPlay-1)]){
                // GameObject go = new GameObject();
                // go.AddComponent<Image>();
                
                // go.GetComponent<Image>().color = board.pieceOnIt;

                // go.name = col + " " + line;
                // go.transform.SetParent(board.cells[col,line].transform,false);
                
            }
            if(board.cells[col,line].GetComponent<Cell>().GetPieceOnIt()){
                // GameObject go = new GameObject();
                // go.AddComponent<Image>();
                
                // go.GetComponent<Image>().color = board.pieceOnIt;

                // go.name = col + " " + line;
                // go.transform.SetParent(board.cells[col,line].transform,false);
            }


            }
        }
    }

    public void SetListCellWherePiecesCanMoveToEmpty(){
        for(int i =0 ; i<board.pieces.Count; i++){
               board.pieces[i].cellWherePieceCanMove.Clear();     
               
            }
        
        possibleMovesCount = 0;

    }
    public void SetAllCellWherePiecesCanMove(){
        SetListCellWherePiecesCanMoveToEmpty();
        for(int i =0 ; i<board.pieces.Count; i++){
           
                Piece piece = board.pieces[i];
                if(piece.isDead){
                    continue;
                }
                // Debug.Log("Piece team : "+piece.team);
                // Debug.Log("GameManager team : "+teamToPlay);
                if(piece.team == teamToPlay){ 
                    SetCellsNonPlayable();
                    CalculatePlayableOrAttackedCells(piece,true,false);
                    for (int col = 0; col < 8; col++){
                        for (int line = 0; line < 8; line++)
                        {  
                            if(GetCell(col,line).GetIsPlayable()){
                                piece.cellWherePieceCanMove.Add(GetCell(col,line)); 
                                possibleMovesCount+=1;
                            }
                        }
                    }
                }
            
        }
    }

    public void DefineAllPossibleMoves(){
        possibleMoves.Clear();

        for(int i =0 ; i<board.pieces.Count; i++){
                Piece piece = board.pieces[i];
                if(piece.isDead){
                    continue;
                }
                // Debug.Log("Piece team : "+piece.team);
                // Debug.Log("GameManager team : "+teamToPlay);
                if(piece.team == teamToPlay){ 
                    SetCellsNonPlayable();
                    CalculatePlayableOrAttackedCells(piece,true,false);
                    for (int col = 0; col < 8; col++){
                        for (int line = 0; line < 8; line++)
                        {  
                            if(GetCell(col,line).GetIsPlayable()){
                                

                                if(piece.name == "Pawn" && ((line == 0) || (line == 7))){//Si promotion du pion
                                    PossibleMove possibleMoveQueen = new PossibleMove();
                                    possibleMoveQueen.cellWherePieceCanMove = GetCell(col,line);
                                    possibleMoveQueen.piece = piece;
                                     possibleMoveQueen.nameOfPieceToBecome = "Queen";
                                     possibleMoves.Add(possibleMoveQueen);
                                     
                                     PossibleMove possibleMoveTower = new PossibleMove();
                                    possibleMoveTower.cellWherePieceCanMove = GetCell(col,line);
                                    possibleMoveTower.piece = piece;
                                     possibleMoveTower.nameOfPieceToBecome = "Tower";
                                     possibleMoves.Add(possibleMoveTower);
                                     
                                     PossibleMove possibleMoveBishop = new PossibleMove();
                                possibleMoveBishop.cellWherePieceCanMove = GetCell(col,line);
                                possibleMoveBishop.piece = piece;
                                     possibleMoveBishop.nameOfPieceToBecome = "Bishop";
                                     possibleMoves.Add(possibleMoveBishop);

                                     PossibleMove possibleMoveKnight = new PossibleMove();
                                possibleMoveKnight.cellWherePieceCanMove = GetCell(col,line);
                                possibleMoveKnight.piece = piece;
                                     possibleMoveKnight.nameOfPieceToBecome = "Knight";
                                     possibleMoves.Add(possibleMoveKnight);
                                }
                                else{
                                    PossibleMove possibleMove = new PossibleMove();
                                possibleMove.cellWherePieceCanMove = GetCell(col,line);
                                possibleMove.piece = piece;
                                possibleMoves.Add(possibleMove); 
                                    
                                }

                                
                            }
                        }
                    }
                }
            
        }
    }

    public void DoMove(Piece piece){
        Cell closestCell = GetNearestCellTransform(piece.transform).GetComponent<Cell>();

        MovePiece(piece,closestCell,false);

          
        CheckEndOfGame(piece);

        UpdatePiecePositionUI();
        

        
     }

     public void CheckEndOfGame(Piece piece){
        SetCellsNonPlayable();
        SetAllAttackedCells();
        SetPlayerTurn();
        

         //Check if il reste des coups possibles pour l'adversaire --> si pas de coups possibles on rentre dans la condition
        SetAllCellWherePiecesCanMove();
 
        if(possibleMovesCount == 0){


            //Check echec
            if(isKingChecked(Mathf.Abs(piece.team-1))){ // si le roi est en echec --> echec et mat
               // Debug.Log("echec et mat");
                victoryGameObject.SetActive(true);
                victoryText.text = "Echec et Mat : les "+(Mathf.Abs(teamToPlay-1) == 0 ? "Blancs":"Noirs")+" ont gagné";
                gameOver =true;

            }
            else{
                //Debug.Log("Pat"); // si le roi n'est pas en echec --> pat
                victoryGameObject.SetActive(true);
                victoryText.text = "Pat : Match nul";
                gameOver =true;



            }

        }
        else{
            //Debug.Log("Nombre de coups possibles: "+ possibleMovesCount);
        }
        SetCellsNonPlayable();
        // TO PUT UpdateCellUI();

        if(PositionRepetition()){
            victoryGameObject.SetActive(true);
            victoryText.text = "3é repetition de la position, match nul";
            gameOver =true;

        }

        
    

     }
    private bool PositionRepetition(){

    

            
        for(int i = 0; i< positions.Count; i++)
        {
        int countRepetition = positions.Where(s=>s!=null && s == positions[positions.Count -1]).Count();
                
        // Debug.Log(countRepetition+" répétitions");

        if(countRepetition > 2){
            
            return true;
        }
                
                
            
        }
        return false;
    }
     public string GetPosition(){
         string position = "";
         List<Piece> pieces =new List<Piece>();
         for(int i =0 ; i<board.pieces.Count; i++){
                Piece piece = board.pieces[i];
                pieces.Add(piece);
                // position += piece.name+piece.team+piece.column+piece.line;
                
            
        }
        List<Piece> orderedPieces = pieces.OrderBy(q => q.id).ToList();
        foreach (Piece piece in orderedPieces)
        {
            position +=" id: "+piece.id+" team :"+piece.team+" col"+piece.column+" line: "+piece.line+"\n";
        }

        return position;
     }

     public void UpdatePiecePositionUI(){
         for (int i = 0; i < board.pieces.Count; i++)
         {
             if(board.pieces[i].isDead){
                 continue;
             }
             PutAPieceToItsCell(board.pieces[i]);
         }

         //UpdateCellUI();
     }



     private IEnumerator UpdatePiecePositionUICoroutine(float speed){
         while(true){
            for (int i = 0; i < board.pieces.Count; i++)
         {
             if(board.pieces[i].isDead){
                 continue;
             }
             PutAPieceToItsCell(board.pieces[i]);
         }
         yield return new WaitForSeconds(speed);
         //Debug.Log("update UI");
         }
         
     }
    

    
}

