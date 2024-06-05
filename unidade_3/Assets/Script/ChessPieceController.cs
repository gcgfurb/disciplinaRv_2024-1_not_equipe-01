using UnityEngine;
using Leap.Unity;
using Leap.Unity.PhysicalHands;
using Script.ChessPieces;
using UnityEngine.Serialization;

public class ChessPieceController : MonoBehaviour
{
    public LeapServiceProvider leapServiceProvider;
    [FormerlySerializedAs("Chessboard")] public ChessboardV2 chessboard;
    private Rigidbody pieceRigidbody;
    
    public void OnBeginGrab(ContactHand contactHand)
    {
        var hand = leapServiceProvider.CurrentFrame.Hands[0];
        
        Ray ray = new Ray(hand.PalmPosition, hand.PalmNormal);
        if (Physics.Raycast(ray, out var hit))
        {
            if (pieceRigidbody is null)
            {
                pieceRigidbody = hit.collider.gameObject.GetComponent<Rigidbody>();
                pieceRigidbody.constraints = RigidbodyConstraints.None;
                pieceRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            
            chessboard.OnBeginGrab(hit.collider.gameObject.GetComponent<ChessPiece>());
        }
    }
    
    public void OnEndGrab(ContactHand contactHand)
    {
        pieceRigidbody.constraints = RigidbodyConstraints.FreezeAll; // Congela tudo de novo
        
        chessboard.EndBeginGrab(pieceRigidbody.GetComponent<ChessPiece>());
        
        pieceRigidbody = null;
    }
}