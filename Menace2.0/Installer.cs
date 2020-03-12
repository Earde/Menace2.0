using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Menace2._0
{
    class Installer
    {
        string boardPath = AppDomain.CurrentDomain.BaseDirectory + @"\menaceBoards.txt";
        string turnMappingPath = AppDomain.CurrentDomain.BaseDirectory + @"\menaceTurnMap.txt";
        JsonSerializerSettings jsonSettings = new JsonSerializerSettings{ TypeNameHandling = TypeNameHandling.All };

        //tree maken
        public List<string> boards = new List<string>() { "NNNNNNNNN" };//lijst met alle speelborden
        private Dictionary<int, List<int>> nextBoardMapping = new Dictionary<int, List<int>>();//<index speelbord, indexes van speelborden die daarop volgen>

        private bool NeedInit()
        {
            if (File.Exists(boardPath) && File.Exists(turnMappingPath))
            {
                Console.WriteLine("Loading...");
                string boardJson = File.ReadAllText(boardPath);
                boards = (List<string>)JsonConvert.DeserializeObject(boardJson, jsonSettings);
                string turnMappingJson = File.ReadAllText(turnMappingPath);
                nextBoardMapping = (Dictionary<int, List<int>>)JsonConvert.DeserializeObject(turnMappingJson, jsonSettings);
                return false;
            }
            return true;
        }

        private void Save()
        {
            Console.WriteLine("Saving boards...");
            string boardJson = JsonConvert.SerializeObject(boards, jsonSettings);
            File.WriteAllText(boardPath, boardJson);
            string turnMappingJson = JsonConvert.SerializeObject(nextBoardMapping, jsonSettings);
            File.WriteAllText(turnMappingPath, turnMappingJson);
            Console.WriteLine("Saved to " + AppDomain.CurrentDomain.BaseDirectory);
        }

        public void Run()
        {
            if (NeedInit())
            {
                Console.WriteLine("Installing boards...");
                //maak een leeg bord
                char[] board = new char[9];
                for (int i = 0; i < 9; i++)
                {
                    board[i] = 'N';
                }
                //begin met spelposities vinden (probeert alle mogelijkheden d.m.v. backtracking)
                RecursiveRun(board, 'X', 0, 0);
                Console.WriteLine("Total generated boards: " + boards.Count);
                Save();
            }
        }

        public List<int> GetNextBoards(string board)
        {
            int index = Exists(board);
            if (nextBoardMapping.ContainsKey(index))
            {
                return nextBoardMapping[index];
            }
            else
            {
                return new List<int>();
            }
        }

        private void RecursiveRun(char[] board, char pos, int turn, int prevBoardIndex)
        {
            if (boards.Count == 627 && nextBoardMapping.Count == 577) return;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == 'N')
                {
                    board[i] = pos;
                    turn++;
                    int existing = Exists(new string(board));
                    if (turn < 9 && HasWinner(new string(board)) == 'N')
                    {
                        if (existing == -1)
                        {
                            existing = boards.Count;
                            boards.Insert(existing, new string(board));
                        }
                        if (nextBoardMapping.ContainsKey(prevBoardIndex))
                        {
                            if (!nextBoardMapping[prevBoardIndex].Contains(existing))
                            {
                                nextBoardMapping[prevBoardIndex].Add(existing);
                            }
                        }
                        else
                        {
                            nextBoardMapping.Add(prevBoardIndex, new List<int> { existing });
                        }
                        RecursiveRun(board, pos == 'X' ? 'O' : 'X', turn, existing);
                    }
                    turn--;
                    board[i] = 'N';
                }
                
            }
        }

        public char HasWinner(string board)
        {
            //check rows
            for (int j = 0; j < 3; j++)
            {
                if (board[j * 3] != 'N' && 
                    board[j * 3] == board[j * 3 + 1] && 
                    board[j * 3 + 1] == board[j * 3 + 2])
                {
                    return board[j * 3];
                }
            }
            //check columns
            for (int i = 0; i < 3; i++)
            {
                if (board[i] != 'N' &&
                    board[i] == board[i + 3] &&
                    board[i + 3] == board[i + 6])
                {
                    return board[i];
                }
            }
            //diagonal
            if (board[0] != 'N' &&
                board[0] == board[4] &&
                board[4] == board[8])
            {
                return board[0];
            }
            if (board[2] != 'N' &&
                board[2] == board[4] &&
                board[4] == board[6])
            {
                return board[2];
            }
            return 'N';
        }

        private int Exists(string board)
        {
            string[] combinations = new string[8];
            combinations[0] = board;
            combinations[1] = TurnRight(board);
            combinations[2] = TurnLeft(board);
            combinations[3] = TurnRight(combinations[1]);
            combinations[4] = Flip(board);
            combinations[5] = Flip(combinations[1]);
            combinations[6] = Flip(combinations[2]);
            combinations[7] = Flip(combinations[3]);
            for (int i = 0; i < boards.Count; i++)
            {
                for (int j = 0; j < combinations.Length; j++)
                {
                    if (boards[i] == combinations[j])
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private string TurnLeft(string board)
        {
            char[] output = new char[9];
            output[0] = board[2];
            output[1] = board[5];
            output[2] = board[8];
            output[3] = board[1];
            output[4] = board[4];
            output[5] = board[7];
            output[6] = board[0];
            output[7] = board[3];
            output[8] = board[6];
            return new string(output);
        }

        private string TurnRight(string board)
        {
            char[] output = new char[9];
            output[0] = board[6];
            output[1] = board[3];
            output[2] = board[0];
            output[3] = board[7];
            output[4] = board[4];
            output[5] = board[1];
            output[6] = board[8];
            output[7] = board[5];
            output[8] = board[2];
            return new string(output);
        }

        private string Flip(string board)
        {
            char[] output = new char[9];
            output[0] = board[6];
            output[1] = board[7];
            output[2] = board[8];
            output[3] = board[3];
            output[4] = board[4];
            output[5] = board[5];
            output[6] = board[0];
            output[7] = board[1];
            output[8] = board[2];
            return new string(output);
        }
    }
}
