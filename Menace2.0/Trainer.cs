using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Menace2._0
{
    class Trainer
    {
        int trainGames = 1000; //aantal spellen om te trainen
        Installer installer = new Installer();
        Dictionary<int, int> pointMap = new Dictionary<int, int>();//<index speelbord, score>
        Random rand = new Random();

        private void Init()
        {
            installer.Run();
            for (int i = 0; i < installer.boards.Count; i++)
            {
                int initialScore = 0;
                int turn = (Turn(installer.boards[i]) + 1) / 2;
                if (turn == 1)
                {
                    initialScore = 8;
                } 
                else if (turn == 2)
                {
                    initialScore = 4;
                }
                else if (turn == 3)
                {
                    initialScore = 2;
                }
                else if (turn == 4)
                {
                    initialScore = 1;
                }
                pointMap.Add(i, initialScore);
            }
        }

        public void Run()
        {
            Init();
            Console.WriteLine("Training " + trainGames.ToString() + " Games...");
            for (int i = 0; i < trainGames; i++)
            {
                PlayGame();
            }
            Console.WriteLine("Done training.");
            Console.Write("Print output? (y/n) ");
            string get = Console.ReadLine();
            if (get.ToLower().Equals("y"))
            {
                foreach (KeyValuePair<int, int> pointsBoard in pointMap.OrderBy(x => x.Value))
                {
                    Console.WriteLine(installer.boards[pointsBoard.Key] + " " + pointsBoard.Value);
                }
            }
        }

        public string GetBestBoard(string board)
        {
            if (board.Length != 9)
            {
                return "Invalid input.";
            }
            if (CanWin(board))
            {
                return "Winnable?";
            }
            List<int> indexes = installer.GetNextBoards(board);
            int score = 0;
            int index = -1;
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
                return "Winnable?";
            } else
            {
                return installer.boards[index];
            }
        }

        private bool CanWin(string board)
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
                        return true;
                    }
                    b[i] = 'N';
                }
            }
            return false;
        }

        private void PlayGame()
        {
            string board = "NNNNNNNNN";
            bool playing = true;
            List<int> usedXBoards = new List<int>();
            List<int> usedOBoards = new List<int>();
            int turn = 0;
            while (playing)
            {
                int nextBoard = -1;
                if (!CanWin(board))//kijk of er een winnende move is
                {
                    //krijg alle mogelijke volgende borden
                    List<int> nextBoards = installer.GetNextBoards(board);
                    int score = 0;
                    //tel de scores bij elkaar op
                    foreach (int index in nextBoards)
                    {
                        score += pointMap[index];
                    }
                    //pak een random speelbord (hogere score heeft meer kans om gekozen te worden)
                    int random = rand.Next(1, score + 1);
                    score = 0;
                    foreach (int index in nextBoards)
                    {
                        score += pointMap[index];
                        if (score >= random)
                        {
                            nextBoard = index;
                            break;
                        }
                    }
                }
                //Als er geen volgend speelbord gevonden is resulteert de volgende zet sowieso in winst of gelijkspel
                if (nextBoard == -1)
                {
                    char result = DoFinalMove(board, turn);
                    UpdateScores(result, usedXBoards, usedOBoards);
                    playing = false;
                }
                else
                {
                    board = installer.boards[nextBoard];
                    turn++;
                    if (turn % 2 == 0)
                    {
                        usedOBoards.Add(nextBoard);
                    }
                    else
                    {
                        usedXBoards.Add(nextBoard);
                    }
                }
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
                xScore = 1;
                oScore = 1;
            }
            else if (result == 'X')
            {
                xScore = 3;
                oScore = -1;
            }
            else if (result == 'O')
            {
                xScore = -1;
                oScore = 3;
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

        private char DoFinalMove(string board, int turn)
        {
            char[] b = board.ToCharArray();
            if (turn == 8) //zet de laatste zet en kijkt dan wie er gewonnen/gelijkspel heeft
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
            else
            {
                return turn % 2 == 0 ? 'X' : 'O'; //de gene die aan de beurt is gaat sowieso winnen
            }
        }
    }
}
