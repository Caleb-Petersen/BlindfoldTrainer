using ilf.pgn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlindfoldTrainer
{
    public class BlindfoldMove : Move
    {
        /// <summary>
        /// Hack constructor to create a blindfold move from a move
        /// </summary>
        /// <remarks>Check to see if reflection works to copy properties</remarks>
        /// <remarks>Check to see if cast is possible</remarks>
        /// <param name="m"></param>
        public BlindfoldMove(Move m)
        {
            Piece = m.Piece;
            IsDoubleCheck = m.IsDoubleCheck;
            IsCheck = m.IsCheck;
            PromotedPiece = m.PromotedPiece;
            OriginRank = m.OriginRank;
            OriginFile = m.OriginFile;
            OriginSquare = m.OriginSquare;
            Annotation = m.Annotation;
            TargetFile = m.TargetFile;
            TargetSquare = m.TargetSquare;
            TargetPiece = m.TargetPiece;
            Type = m.Type;
            IsCheckMate = m.IsCheckMate;
        }

        public string SpeechText
        {
            get
            {
                bool isError = false;
                string errorMsg = "";
                string movingPieceText = "";
                string startingSquareText = "";
                string captureText = "";
                string destinationSquareText = "";
                string enpassantText = "";
                string promotionText = "";
                string checkText = "";

                if(Type == MoveType.CastleKingSide)
                {
                    return "Castled Kingside";
                }
                else if (Type == MoveType.CastleQueenSide)
                {
                    return "Castled Queenside";
                }

                if(Piece != null && Piece.HasValue)
                {
                    movingPieceText = Piece.Value.ToString() + " ";
                }
                else
                {
                    isError = true;
                    errorMsg = "Error. No piece recorded in move.";
                }

                if(OriginSquare != null)
                {
                    startingSquareText = " on " + OriginSquare.File.ToString() + OriginSquare.Rank.ToString() + " ";
                }
                else if (OriginFile != null)
                {
                    startingSquareText = " on the " + OriginFile.Value.ToString() + " file ";
                }
                else if (OriginRank != null)
                {
                    startingSquareText = " on rank " + OriginRank.Value.ToString();
                }
                
                if (Type == MoveType.Capture)
                {
                    captureText = "takes on ";
                }
                else if (Type == MoveType.CaptureEnPassant)
                {
                    captureText = "takes enpassant on ";
                }
                else
                {
                    //Don't say "to" if a capture has taken place
                    startingSquareText += " to ";
                }

                if(TargetSquare != null)
                {
                    destinationSquareText = TargetSquare.File.ToString() + TargetSquare.Rank.ToString() + " ";
                }
                else if(!isError)
                {
                    isError = true;
                    errorMsg = "Error: No Target Square Found for " + movingPieceText + startingSquareText;
                }

                
                if(PromotedPiece != null && PromotedPiece.HasValue)
                {
                    promotionText = "equals " + PromotedPiece.Value.ToString() + " ";
                }

                if(IsCheckMate != null && IsCheckMate.Value)
                {
                    checkText = "checkmate";
                }
                else if(IsCheck != null && IsCheck.Value )
                {
                    checkText = "check";
                }
                
                //Return results
                if (isError)
                {
                    return errorMsg;
                }

                return movingPieceText + startingSquareText + captureText + destinationSquareText + 
                    enpassantText + promotionText + checkText;
            }
        }
    }
}
