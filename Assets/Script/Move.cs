using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Move
{
    public Piece pieceMoved;
    public Piece pieceKilled;

    public Cell cellWherePieceMoved;
    public Cell originalCell;

    public bool hasBeenPromoted = false;

    public Piece towerRocked = null;
    public Cell cellFromWhereTowerCastled;

    public int turn;
}
