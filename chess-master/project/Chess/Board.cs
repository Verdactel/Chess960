using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chess
{
    public class ChessBoard
    {
        private static int[] pieceWeights = { 1, 3, 4, 5, 7, 20 };

        public piece_t[][] Grid { get; private set; }
        public Dictionary<Player, position_t> Kings { get; private set; }
        public Dictionary<Player, List<position_t>> Pieces { get; private set; }
        public Dictionary<Player, position_t> LastMove { get; private set; }

        bool isChess960;

        public ChessBoard()
        {
            // init blank board grid
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                    Grid[i][j] = new piece_t(Piece.NONE, Player.WHITE);
            }

            // init last moves
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t();
            LastMove[Player.WHITE] = new position_t();

            // init king positions
            Kings = new Dictionary<Player, position_t>();

            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());
        }

        public ChessBoard(ChessBoard copy)
        {
            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());

            // init board grid to copy locations
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                {
                    Grid[i][j] = new piece_t(copy.Grid[i][j]);

                    // add piece location to list
                    if (Grid[i][j].piece != Piece.NONE)
                        Pieces[Grid[i][j].player].Add(new position_t(j, i));
                }
            }

            // copy last known move
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t(copy.LastMove[Player.BLACK]);
            LastMove[Player.WHITE] = new position_t(copy.LastMove[Player.WHITE]);

            // copy king locations
            Kings = new Dictionary<Player, position_t>();
            Kings[Player.BLACK] = new position_t(copy.Kings[Player.BLACK]);
            Kings[Player.WHITE] = new position_t(copy.Kings[Player.WHITE]);
        }

        /// <summary>
        /// Calculate and return the boards fitness value.
        /// </summary>
        /// <param name="max">Who's side are we viewing from.</param>
        /// <returns>The board fitness value, what else?</returns>
        public int fitness(Player max)
        {
            int fitness = 0;
            int[] blackPieces = { 0, 0, 0, 0, 0, 0 };
            int[] whitePieces = { 0, 0, 0, 0, 0, 0 };
            int blackMoves = 0;
            int whiteMoves = 0;

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.BLACK])
            {
                blackMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                blackPieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.WHITE])
            {
                whiteMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                whitePieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // if viewing from black side
            if (max == Player.BLACK)
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (blackPieces[i] - whitePieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (blackMoves - whiteMoves));
            }
            else
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (whitePieces[i] - blackPieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (whiteMoves - blackMoves));
            }

            return fitness;
        }

        public void SetInitialPlacement()
        {
            for (int i = 0; i < 8; i++)
            {
                SetPiece(Piece.PAWN, Player.WHITE, i, 1);
                SetPiece(Piece.PAWN, Player.BLACK, i, 6);
            }

            if (isChess960)
            {
                Random rand = new Random();

                List<int> homePositions = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };

                #region King Placement
                //Place King
                int kingPos = rand.Next(1, 7);
                SetPiece(Piece.KING, Player.WHITE, kingPos, 0);
                SetPiece(Piece.KING, Player.BLACK, kingPos, 7);
                #endregion

                #region Rook Placement
                //Rule 2: Place rooks around king
                int rookOnePos = rand.Next(homePositions[0], homePositions[kingPos]);
                int rookTwoPos = rand.Next(homePositions[kingPos] + 1, homePositions.Count);

                SetPiece(Piece.ROOK, Player.WHITE, rookOnePos, 0);
                SetPiece(Piece.ROOK, Player.BLACK, rookOnePos, 7);
                SetPiece(Piece.ROOK, Player.WHITE, rookTwoPos, 0);
                SetPiece(Piece.ROOK, Player.BLACK, rookTwoPos, 7);

                homePositions.Remove(rookOnePos);
                homePositions.Remove(rookTwoPos);
                homePositions.Remove(kingPos);
                #endregion

                #region Bishop Placement
                //Rule 1: Place Bishops
                int randomPos = rand.Next(homePositions.Count);
                int bishopOnePos = homePositions[randomPos];
                SetPiece(Piece.BISHOP, Player.WHITE, bishopOnePos, 0);
                SetPiece(Piece.BISHOP, Player.BLACK, bishopOnePos, 7);
                homePositions.Remove(bishopOnePos);

                randomPos = rand.Next(homePositions.Count);
                int bishopTwoPos = homePositions[randomPos];
                if (bishopOnePos % 2 == 0)
                {
                    if (bishopTwoPos % 2 == 1) { }
                    else
                    {
                        while(bishopTwoPos % 2 != 1)
                        {
                            randomPos = rand.Next(homePositions.Count);
                            bishopTwoPos = homePositions[randomPos];
                        }
                    }
                }
                else
                {
                    if (bishopTwoPos % 2 == 0) { }
                    else
                    {
                        while (bishopTwoPos % 2 != 0)
                        {
                            randomPos = rand.Next(homePositions.Count);
                            bishopTwoPos = homePositions[randomPos];
                        }
                    }
                }
                SetPiece(Piece.BISHOP, Player.WHITE, bishopTwoPos, 0);
                SetPiece(Piece.BISHOP, Player.BLACK, bishopTwoPos, 7);
                
                homePositions.Remove(bishopTwoPos);
                #endregion

                #region Knight Placement
                randomPos = rand.Next(homePositions.Count);
                int knightOnePos = homePositions[randomPos];
                SetPiece(Piece.KNIGHT, Player.WHITE, knightOnePos, 0);
                SetPiece(Piece.KNIGHT, Player.BLACK, knightOnePos, 7);
                homePositions.Remove(knightOnePos);

                randomPos = rand.Next(homePositions.Count);
                int knightTwoPos = homePositions[randomPos];
                SetPiece(Piece.KNIGHT, Player.WHITE, knightTwoPos, 0);
                SetPiece(Piece.KNIGHT, Player.BLACK, knightTwoPos, 7);
                homePositions.Remove(knightTwoPos);
                #endregion

                #region Queen Placement
                SetPiece(Piece.QUEEN, Player.WHITE, homePositions[0], 0);
                SetPiece(Piece.QUEEN, Player.BLACK, homePositions[0], 7);
                #endregion
            }
            else
            {
                SetPiece(Piece.ROOK, Player.WHITE, 0, 0);
                SetPiece(Piece.ROOK, Player.WHITE, 7, 0);
                SetPiece(Piece.ROOK, Player.BLACK, 0, 7);
                SetPiece(Piece.ROOK, Player.BLACK, 7, 7);

                SetPiece(Piece.KNIGHT, Player.WHITE, 1, 0);
                SetPiece(Piece.KNIGHT, Player.WHITE, 6, 0);
                SetPiece(Piece.KNIGHT, Player.BLACK, 1, 7);
                SetPiece(Piece.KNIGHT, Player.BLACK, 6, 7);

                SetPiece(Piece.BISHOP, Player.WHITE, 2, 0);
                SetPiece(Piece.BISHOP, Player.WHITE, 5, 0);
                SetPiece(Piece.BISHOP, Player.BLACK, 2, 7);
                SetPiece(Piece.BISHOP, Player.BLACK, 5, 7);

                SetPiece(Piece.KING, Player.WHITE, 4, 0);
                SetPiece(Piece.KING, Player.BLACK, 4, 7);
                Kings[Player.WHITE] = new position_t(4, 0);
                Kings[Player.BLACK] = new position_t(4, 7);
                SetPiece(Piece.QUEEN, Player.WHITE, 3, 0);
                SetPiece(Piece.QUEEN, Player.BLACK, 3, 7);
            }
        }

        public void SetPiece(Piece piece, Player player, int letter, int number)
        {
            // set grid values
            Grid[number][letter].piece = piece;
            Grid[number][letter].player = player;

            // add piece to list
            Pieces[player].Add(new position_t(letter, number));

            // update king position
            if (piece == Piece.KING)
            {
                Kings[player] = new position_t(letter, number);
            }
        }

        public void SetChess960(bool active)
        {
            isChess960 = active;
        }
    }
}
