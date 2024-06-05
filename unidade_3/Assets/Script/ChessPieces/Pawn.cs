using System.Collections.Generic;
using UnityEngine;

namespace Script.ChessPieces
{
    public class Pawn : ChessPiece
    {
        public override List<Vector2Int> GetAvailableMoves(List<ChessPiece> pieces, int tileCountX, int tileCountY,
            ChessPiece piece)
        {
            List<Vector2Int> r = new List<Vector2Int>();

            int direction = piece.team == 0 ? 1 : -1;

            // Verifica se a posição está vazia
            if (GetPieceAt(pieces, piece.currentX, piece.currentY + direction) == null)
                r.Add(new Vector2Int(piece.currentX, piece.currentY + direction));

            if (GetPieceAt(pieces, piece.currentX, piece.currentY + direction) == null)
            {
                switch (piece.team)
                {
                    case 0 when piece.currentY == 1 &&
                                GetPieceAt(pieces, piece.currentX, piece.currentY + direction * 2) == null:
                    case 1 when piece.currentY == 6 &&
                                GetPieceAt(pieces, piece.currentX, piece.currentY + direction * 2) == null:
                        r.Add(new Vector2Int(piece.currentX, piece.currentY + direction * 2));
                        break;
                }
            }

            // Captura diagonal à direita
            if (piece.currentX != tileCountX - 1)
            {
                ChessPiece rightDiagonalPiece = GetPieceAt(pieces, piece.currentX + 1, piece.currentY + direction);
                if (rightDiagonalPiece != null && rightDiagonalPiece.team != piece.team)
                    r.Add(new Vector2Int(piece.currentX + 1, piece.currentY + direction));
            }

            // Captura diagonal à esquerda
            if (piece.currentX != 0)
            {
                ChessPiece leftDiagonalPiece = GetPieceAt(pieces, piece.currentX - 1, piece.currentY + direction);
                if (leftDiagonalPiece != null && leftDiagonalPiece.team != piece.team)
                    r.Add(new Vector2Int(piece.currentX - 1, piece.currentY + direction));
            }

            return r;
        }

        private static ChessPiece GetPieceAt(List<ChessPiece> board, int x, int y)
        {
            return board.Find(piece => piece.currentX == x && piece.currentY == y);
        }
    }
}