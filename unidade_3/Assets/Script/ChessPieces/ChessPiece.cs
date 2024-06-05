using System.Collections.Generic;
using UnityEngine;

namespace Script.ChessPieces
{
    public enum ChessPieceType
    {
        None = 0,
        Pawn = 1,
        Rook = 2,
        Knight = 3,
        Bishop = 4,
        Queen = 5,
        King = 6
    }

    public class ChessPiece : MonoBehaviour
    {
        public int team;
        public int currentX;
        public int currentY;
        public ChessPieceType type;

        private Vector3 desiredPosition;
        private Vector3 desiredScale = Vector3.one;
        
        public virtual List<Vector2Int> GetAvailableMoves(List<ChessPiece> pieces, int tileCountX, int tileCountY, ChessPiece piece)
        {
            List<Vector2Int> r = new List<Vector2Int>();

            return r;
        }
    }
}