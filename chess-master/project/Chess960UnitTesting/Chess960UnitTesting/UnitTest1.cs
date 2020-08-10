using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess;
using System.Linq;
using System.Collections.Generic;

namespace Chess960UnitTesting
{
    [TestClass]
    public class UnitTesting
    {
        [TestMethod]
        public void TestRooksChess960()
        {
            ChessBoard chessboard = CreateChessboard(true);

            int kingPos = 0;
            for (int i = 0; i < chessboard.Grid[0].Count() - 1; i++)
            {
                if(chessboard.Grid[0][i].piece == Piece.KING)
                {
                    kingPos = i;
                    break;
                }
            }

            int rookPos = -1;
            for(int i = 0; i < kingPos; i++)
            {
                if (chessboard.Grid[0][i].piece == Piece.ROOK)
                {
                    rookPos = i;
                    break;
                }
            }
            Assert.IsTrue(rookPos != -1);

            rookPos = -1;
            for (int i = kingPos; i < chessboard.Grid.Count(); i++)
            {
                if (chessboard.Grid[0][i].piece == Piece.ROOK)
                {
                    rookPos = 1;
                    break;
                }
                
            }

            Assert.IsTrue(rookPos != -1);
        }

        [TestMethod]
        public void TestBishopsChess960()
        {
            ChessBoard chessboard = CreateChessboard(true);

            int bishopPos = -1;
            for (int i = 0; i < chessboard.Grid[0].Count(); i++)
            {
                if (chessboard.Grid[0][i].piece == Piece.BISHOP)
                {
                    bishopPos = i;
                    break;
                }
            }

            for (int i = bishopPos + 1; i < chessboard.Grid[0].Count(); i++)
            {
                if(chessboard.Grid[0][i].piece == Piece.BISHOP)
                {
                    if(bishopPos % 2 == 0)
                    {
                        Assert.IsTrue(i % 2 == 1);
                    }
                    else
                    {
                        Assert.IsTrue(i % 2 == 0);
                    }
                }
            }
        }

        [TestMethod]
        public void TestAllPiecesChess960()
        {
            ChessBoard chessboard = CreateChessboard(true);

            int rooks = 0, bishops = 0, knights = 0, king = 0, queen = 0;

            for(int i = 0; i < chessboard.Grid[0].Count(); i++)
            {
                Piece piece = chessboard.Grid[0][i].piece;
                switch (piece)
                {
                    case Piece.ROOK:
                        rooks++;
                        break;
                    case Piece.BISHOP:
                        bishops++;
                        break;
                    case Piece.KNIGHT:
                        knights++;
                        break;
                    case Piece.QUEEN:
                        queen++;
                        break;
                    case Piece.KING:
                        king++;
                        break;
                    default:
                        break;
                }
            }
            Assert.IsTrue(rooks == 2 && bishops == 2 && knights == 2 && king == 1 && queen == 1);
        }

        [TestMethod]
        public void TestMirroredSideChess960()
        {
            ChessBoard chessboard = CreateChessboard(true);

            List<Piece> white = new List<Piece>();
            List<Piece> black = new List<Piece>();

            for(int i = 0; i < chessboard.Grid[0].Count(); i++)
            {
                Piece piece = chessboard.Grid[0][i].piece;
                white.Add(piece);
            }

            for (int i = 0; i < chessboard.Grid[7].Count(); i++)
            {
                Piece piece = chessboard.Grid[7][i].piece;
                black.Add(piece);
            }

            Assert.IsTrue(white.SequenceEqual(black));
        }

        [TestMethod]
        public void TestNormalConfig()
        {
            ChessBoard chessboard = CreateChessboard(false);

            Assert.IsTrue(chessboard.Grid[0][0].piece == Piece.ROOK &&
                          chessboard.Grid[0][1].piece == Piece.KNIGHT &&
                          chessboard.Grid[0][2].piece == Piece.BISHOP &&
                          chessboard.Grid[0][3].piece == Piece.QUEEN &&
                          chessboard.Grid[0][4].piece == Piece.KING &&
                          chessboard.Grid[0][5].piece == Piece.BISHOP &&
                          chessboard.Grid[0][6].piece == Piece.KNIGHT &&
                          chessboard.Grid[0][7].piece == Piece.ROOK);
        }

        public ChessBoard CreateChessboard(bool isChess960)
        {
            ChessBoard chessboard = new ChessBoard();
            chessboard.SetChess960(isChess960);
            chessboard.SetInitialPlacement();

            return chessboard;
        }
    }
}
