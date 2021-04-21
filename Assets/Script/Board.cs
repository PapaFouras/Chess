using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public Sprite whiteColor;
    public Sprite blackColor;

    public Color selectedCellColor;
    public Color whiteAttackCellColor;
    public Color blackAttackCellColor;
    public Color pieceOnIt;

    public GameObject whiteGraveyard;
    public GameObject blackGraveyard;

    public GameObject cellPrefab;
    public GameObject piecePrefab;

    public List<Piece> pieces = new List<Piece>();
    public Cell[,] cells = new Cell[8,8] ;

    private void Awake() {
        CreateCells();
        //PlacePieces("nnnnKnnnPPPPPPPPnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnppppppppnnnnknnn");
        PlacePieces(Util.position5);
        //GameManager.instance.SetAllAttackedCells();
        //GameManager.instance.UpdateCellUI();
    }

    private void CreateCells(){
        for (int col = 0; col < 8; col++)
        {
            for (int line = 0; line < 8; line++)
            {
                //Instantiate cells
                GameObject cellGO =  Instantiate(cellPrefab, transform);
                cells[col,line] = cellGO.GetComponent<Cell>();


                //CellSetup

                string column = "error";
          column = col == 0 ? column = "a": 
                   col == 1 ? column = "b":
                   col == 2 ? column = "c":
                   col == 3 ? column = "d":
                   col == 4 ? column = "e":
                   col == 5 ? column = "f":
                   col == 6 ? column = "g":
                   col == 7 ? column = "h":
                   "error";
                string cellName = column+(line+1).ToString();
                cells[col,line].name = cellName;

                cells[col,line].GetComponent<Image>().sprite =  (col%2 ==0 && line%2 == 0) ? blackColor:
                                                                (col%2 == 1 && line%2 == 1) ? blackColor: whiteColor; 

                cells[col,line].columnNumber = col;
                cells[col,line].lineNumber = line;
                cells[col,line].cellName= cellName;
                cells[col,line].id = col*8+line;
                cells[col,line].isAttacked[0] = false;
                cells[col,line].isAttacked[1] = false;


                Vector3 newPosition = new Vector3(col*100f,line*100f,0f);
                cells[col,line].transform.position += newPosition;

          


                
            }

            
        }

       

        }
    private void PlacePieces(string _placement ="TCFQKFCTPPPPPPPPnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnpppppppptcfqkfct"){
  
    if(_placement.Length != 64){
        Debug.LogError("string du placement des pieces != de 64");
    }

    for (var i = 0; i < _placement.Length; i++)
    {

    if(_placement[i] != 'n'){

       PieceSetUp(_placement,i);
    }
    }
    GameManager.instance.positions.Add(GameManager.instance.GetPosition());

  }




    
  private void PieceSetUp(string placement, int index){
        GameObject pieceGO = Instantiate(piecePrefab,transform);
        Piece piece = pieceGO.GetComponent<Piece>();
        int team;
        string pieceName;
        // Debug.Log("Placement length : "+placement.Length);

        (team,pieceName) =  placement[index] =='T'? (0,"Tower") :
                            placement[index] =='t'? (1,"Tower") :
                            placement[index] =='Q'? (0,"Queen") :
                            placement[index] =='q'? (1,"Queen") :
                            placement[index] =='K'? (0,"King") :
                            placement[index] =='k'? (1,"King") :
                            placement[index] =='F'? (0,"Bishop") :
                            placement[index] =='f'? (1,"Bishop") :
                            placement[index] =='C'? (0,"Knight") :
                            placement[index] =='c'? (1,"Knight") :
                            placement[index] =='P'? (0,"Pawn") :
                            placement[index] =='p'? (1,"Pawn") :
                            (0,"");//Default value;
        piece.SetUp(team,pieceName,index);
        int line = index/8;
        int col = index%8;

      
        Vector3 newPosition = new Vector3(col*100f,-100f-line*100f,0f);
        piece.gameObject.transform.position = cells[col,line].transform.position ;
        GameManager.instance.PlaceInCenterOfNearestCell(piece);
        //Debug.Log("pas ok");

        cells[col,line].SetPieceOnIt(true);
        pieces.Add(piece);
  }

}
