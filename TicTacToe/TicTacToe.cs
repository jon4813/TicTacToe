using System;

namespace TicTacToe
{
    partial class Program
    {
        public class TicTacToe
        {
            // The number of moves that have been made.
            private int _numSquaresTaken = 0;

            // The players' positions.
            private char _currentPlayer = 'X';

            private char _emptySymbol = '_';
            private char _userPlayer = 'X';
            private char _computerPlayer = 'O';
            readonly char[,] _board = new char[3, 3];           


            public void Game()
            {
                InitBoard();
                Console.WriteLine("Крестики-нолики");

                while (true)
                {
                    Print();

                    (int i, int j) coords;
                    while (true)
                    {
                        coords = GetCoords();
                        if (_board[coords.i, coords.j] != _emptySymbol)
                        {
                            Console.WriteLine("Not empty! Try again!");
                            Print();
                        }
                        else
                        {
                            break;
                        }
                    }

                    _board[coords.i, coords.j] = _currentPlayer;

                    if (IsWinner(coords.i, coords.j))
                    {
                        Console.WriteLine($"{_currentPlayer} Win!");
                        return;
                    }

                    if (_numSquaresTaken == 9)
                    {
                        // We have a cat's game.
                        Console.WriteLine("stop!");
                        return;
                    }

                    // Switch players.
                    _currentPlayer = _computerPlayer;

                    // Let the computer move.
                    MakeComputerMove();
                }
            }

            private (int i, int j) GetCoords()
            {
                var fields = Console.ReadLine()?.Split(" ");
                return (int.Parse(fields[0]), int.Parse(fields[1]) );
            }

            private void InitBoard()
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        _board[i, j] = _emptySymbol;
                    }                    
                }
            }


            private void Print()
            {
                Console.WriteLine();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Console.Write(_board[i, j] == _emptySymbol ? " _ " : $" {_board[i, j]} ");
                    }
                    Console.WriteLine();
                }
            }

            // Make the computer take a move.
            private void MakeComputerMove()
            {
                // Minimax looking 3 moves ahead.
                Minimax(out _, out var bestR, out var bestC,
                    _computerPlayer, _userPlayer, 1, 3);

                // Make the move.
                _board[bestR, bestC] = _computerPlayer;
                _numSquaresTaken++;

                // See if there is a winner.
                if (IsWinner(bestR, bestC))
                {
                    Console.WriteLine($"{_currentPlayer} Win!");
                    return;
                }
                if (_numSquaresTaken == 9)
                {
                    // We have a cat's game.

                    return;
                }

                // Switch whose move it is.
                _currentPlayer = _userPlayer;
            }

            private void Minimax(out BoardValues bestValue, out int bestR, out int bestC, char player1, char player2,
                int depth, int maxDepth)
            {
                bestValue = BoardValues.Unknown;
                bestR = -1;
                bestC = -1;

                // If we are too deep, then we don't know.
                if ((depth > maxDepth) || (_numSquaresTaken == 9)) return;

                // Track the worst move for player2.
                BoardValues player2Value = BoardValues.Win;

                // Make test moves.
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        // See if this move is taken.
                        if (_board[row, col] == _emptySymbol)
                        {
                            // Try this move.
                            _board[row, col] = player1;
                            _numSquaresTaken++;

                            // See if this gives player1 a win.
                            if (IsWinner(row, col))
                            {
                                // This gives player1 a win and therefore player2 a loss.
                                // Take this move.
                                bestR = row;
                                bestC = col;
                                player2Value = BoardValues.Loss;
                            }
                            else
                            {
                                // Recursively try moves for player2.
                                Minimax(out var testValue, out _, out _,
                                    player2, player1, depth + 1, maxDepth);

                                // See if this is an improvement for player 2.
                                if (player2Value >= testValue)
                                {
                                    bestR = row;
                                    bestC = col;
                                    player2Value = testValue;
                                }
                            }

                            // Undo the move.
                            _board[row, col] = _emptySymbol;
                            _numSquaresTaken--;
                        }
                        // If player2 will lose, stop searching.
                        if (player2Value == BoardValues.Loss) break;
                    }

                    // If player2 will lose, stop searching.
                    if (player2Value == BoardValues.Loss) break;
                }

                // We now know the worst we can force player2 to do.
                // Convert that into a board value for player1.
                if (player2Value == BoardValues.Loss)
                    bestValue = BoardValues.Win;
                else if (player2Value == BoardValues.Win)
                    bestValue = BoardValues.Loss;
                else
                    bestValue = player2Value;

                // Parameters bestValue, bestR, and bestC contain the best move we found.
            }

            // Return true if the player who just took spare [r, c] has won.
            private bool IsWinner(int r, int c)
            {
                bool isWinner = false;
                char player = _board[r, c];

                if ((player == _board[r, 0]) &&
                    (player == _board[r, 1]) &&
                    (player == _board[r, 2]))
                {
                    isWinner = true;
                }
                else if ((player == _board[0, c]) &&
                         (player == _board[1, c]) &&
                         (player == _board[2, c]))
                {
                    isWinner = true;
                }
                else if (r == c)
                {
                    if ((player == _board[0, 0]) &&
                        (player == _board[1, 1]) &&
                        (player == _board[2, 2])) isWinner = true;
                }
                else if (r + c == 2)
                {
                    if ((player == _board[0, 2]) &&
                        (player == _board[1, 1]) &&
                        (player == _board[2, 0])) isWinner = true;
                }

                return isWinner;
            }

            // Board values.
            private enum BoardValues
            {
                None = 0,
                Loss = 1,
                Draw = 2,
                Unknown = 3,
                Win = 4,
            }

            public enum Player
            {
                X = 1,
                O = 2
            }
        }
    }
}
