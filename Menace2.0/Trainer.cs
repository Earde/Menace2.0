using System;
using System.Collections.Generic;
using System.Linq;

namespace Menace2._0
{
    class Trainer
    {
        int winPointsX = 1, losePointsX = -1, drawPointsX = 0;
        int winPointsO = 1, losePointsO = -1, drawPointsO = 3;
        private Dictionary<int, int> pointMap = new Dictionary<int, int>();//<index speelbord, score>

        public Installer installer;
        public Random rand = new Random();

        public Trainer(Installer installer)
        {
            this.installer = installer;
        }

        public void Init()
        {
            for (int i = 0; i < installer.boards.Count; i++)
            {
                int initialScore = 0;
                int turn = (Turn(installer.boards[i]) + 1) / 2;
                if (turn == 1)
                {
                    initialScore = 100000;
                } 
                else if (turn == 2)
                {
                    initialScore = 100000;
                }
                else if (turn == 3)
                {
                    initialScore = 100000;
                }
                else if (turn == 4)
                {
                    initialScore = 100000;
                }
                pointMap.Add(i, initialScore);
            }
        }

        public void PrintOutput()
        {
            foreach (KeyValuePair<int, int> pointsBoard in pointMap.OrderBy(x => x.Value))
            {
                Console.WriteLine(installer.boards[pointsBoard.Key] + " " + pointsBoard.Value);
            }
        }

        public string GetBestBoard(string board, bool print)
        {
            if (board.Length != 9)
            {
                return "???????????????";
            }
            if (installer.HasWinner(board) != 'N') return board;
            int winningMove = HasWinningMove(board);
            if (winningMove > -1)
            {
                char[] win = board.ToCharArray();
                win[winningMove] = Turn(board) % 2 == 0 ? 'X' : 'O';
                return new string(win);
            }
            List<int> indexes = installer.GetNextBoards(board);
            int score = 0;
            int index = -1;
            if (print && indexes.Count > 0)
            {
                Console.WriteLine("BOARDS    POINTS");
                indexes.OrderBy(i => pointMap[i]).ToList().ForEach(ind => Console.WriteLine(this.installer.boards[ind] + " " + pointMap[ind].ToString()));
                Console.WriteLine(" ");
            }
            foreach (int i in indexes)
            {
                if (pointMap[i] > score)
                {
                    score = pointMap[i];
                    index = i;
                }
            }
            if (index == -1)
            {
                char[] draw = board.ToCharArray();
                for (int i = 0; i < draw.Length; i++)
                {
                    if (draw[i] == 'N')
                    {
                        draw[i] = Turn(board) % 2 == 0 ? 'X' : 'O';
                        break;
                    }
                }
                return new string(draw);
            } else
            {
                return installer.boards[index];
            }
        }

        public int HasWinningMove(string board)
        {
            char[] b = board.ToCharArray();
            for (int i = 0; i < board.Length; i++)
            {
                if (b[i] == 'N')
                {
                    b[i] = Turn(board) % 2 == 0 ? 'X' : 'O';
                    char w = installer.HasWinner(new string(b));
                    if (w != 'N')
                    {
                        return i;
                    }
                    b[i] = 'N';
                }
            }
            return -1;
        }

        public void PlayTrainGame()
        {
            string board = "NNNNNNNNN";
            List<int> usedXBoards = new List<int>();
            List<int> usedOBoards = new List<int>();
            int turn = 0;
            while (true)
            {
                int nextBoard = -1;
                List<int> nextBoards = installer.GetNextBoards(board);//kies een random bord
                if (HasWinningMove(board) == -1 && nextBoards.Count > 0)//kijk of er een winnende move is
                {
                    nextBoard = nextBoards[rand.Next(0, nextBoards.Count)];
                }
                //Als er geen volgend speelbord gevonden is resulteert de volgende zet sowieso in winst of gelijkspel
                if (nextBoard == -1)
                {
                    char result = DoFinalMove(board);
                    UpdateScores(result, usedXBoards, usedOBoards);
                    break;
                }
                else
                {
                    board = installer.boards[nextBoard];
                    if (turn % 2 == 0)
                    {
                        usedXBoards.Add(nextBoard);
                    }
                    else
                    {
                        usedOBoards.Add(nextBoard);
                    }
                }
                turn++;
            }
        }

        private int Turn(string b)
        {
            int count = 0;
            foreach (char c in b)
            {
                if (c != 'N') count++;
            }
            return count;
        }

        private void UpdateScores(char result, List<int> xBoards, List<int> oBoards)
        {
            int xScore = 0, oScore = 0;
            //Update de punten voor de gespeelde velden
            if (result == 'N')
            {
                xScore = drawPointsX;
                oScore = drawPointsO;
            }
            else if (result == 'X')
            {
                xScore = winPointsX;
                oScore = losePointsO;
            }
            else if (result == 'O')
            {
                xScore = losePointsX;
                oScore = winPointsO;
            }
            foreach (int index in xBoards)
            {
                pointMap[index] += xScore;
                if (pointMap[index] < 1)
                {
                    pointMap[index] = 1;
                }
            }
            foreach (int index in oBoards)
            {
                pointMap[index] += oScore;
                if (pointMap[index] < 1)
                {
                    pointMap[index] = 1;
                }
            }
        }

        public char DoFinalMove(string board)
        {
            char[] b = board.ToCharArray();
            if (Turn(board) == 8) //zet de laatste zet en kijkt of er een winnaar is
            {
                for (int i = 0; i < b.Length; i++)
                {
                    if (b[i] == 'N')
                    {
                        b[i] = 'X';
                    }
                }
                return installer.HasWinner(new string(b));
            }
            else if (HasWinningMove(board) > -1)
            {
                return Turn(board) % 2 == 0 ? 'X' : 'O'; //de gene die aan de beurt is gaat sowieso winnen
            } else
            {
                throw new Exception("Is niet goed.");
            }
        }
    }
}
