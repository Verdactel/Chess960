using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess;
using System.Linq;

namespace Chess960UnitTesting
{
    [TestClass]
    public class UnitTestingChess960
    {
        [TestMethod]
        public void TestRooks()
        {
            ChessBoard chessboard = CreateChessboard(true);
            
            
        }

        [TestMethod]
        public void TestBishops()
        {
            ChessBoard chessboard = CreateChessboard(true);
        }

        [TestMethod]
        public void TestAllPieces()
        {
            ChessBoard chessboard = CreateChessboard(true);
        }

        [TestMethod]
        public void TestMirroredSide()
        {
            ChessBoard chessboard = CreateChessboard(true);
        }

        [TestMethod]
        public void TestNormalConfig()
        {
            ChessBoard chessboard = CreateChessboard(false);
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
