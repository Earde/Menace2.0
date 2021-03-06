﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Menace2._0
{
    class Tester
    {
        Trainer trainer;
        int testGames;
        string firstPath, secondPath;
        List<TestRun> menaceFirst = new List<TestRun>();
        List<TestRun> menaceSecond = new List<TestRun>();

        public Tester(Trainer trainer, int testGames, int everyXGames)
        {
            this.trainer = trainer;
            this.testGames = testGames;
            this.firstPath = AppDomain.CurrentDomain.BaseDirectory + @"\menaceFirstStatsEvery" + everyXGames.ToString() + "Rounds.csv";
            this.secondPath = AppDomain.CurrentDomain.BaseDirectory + @"\menaceSecondStatsEvery" + everyXGames.ToString() + "Rounds.csv";
        }

        public void Run()
        {
            menaceFirst.Add(DoRun(true));
            menaceSecond.Add(DoRun(false));
        }

        public void CreateCsv()
        {
            var csv = new StringBuilder();
            csv.AppendLine("win;draw;lose");
            var allLines = (from tr in menaceFirst select new object[] {
                tr.win, tr.draw, tr.lose
            }).ToList();
            allLines.ForEach(line => csv.AppendLine(string.Join(";", line)));
            File.WriteAllText(firstPath, csv.ToString());

            csv = new StringBuilder();
            csv.AppendLine("win;draw;lose");
            allLines = (from tr in menaceSecond
                            select new object[] {
                tr.win, tr.draw, tr.lose
            }).ToList();
            allLines.ForEach(line => csv.AppendLine(string.Join(";", line)));
            File.WriteAllText(secondPath, csv.ToString());
            Console.WriteLine("Test stats saved to " + AppDomain.CurrentDomain.BaseDirectory);
        }

        public void PrintOutput()
        {
            Console.WriteLine("Menace playing first:");
            menaceFirst.ForEach(mf => Console.WriteLine("WIN: " + mf.win.ToString("D3") + " DRAW: " + mf.draw.ToString("D3") + " LOSE: " + mf.lose.ToString("D3")));
            Console.WriteLine(" ");
            Console.WriteLine("Menace playing second:");
            menaceSecond.ForEach(mf => Console.WriteLine("WIN: " + mf.win.ToString("D3") + " DRAW: " + mf.draw.ToString("D3") + " LOSE: " + mf.lose.ToString("D3")));
        }

        public bool IsPerfect()
        {
            if (menaceFirst.Last().lose == 0.0 && menaceSecond.Last().lose == 0.0)
            {
                int tempTestGames = testGames;
                testGames = 10000;
                var perfectRun1 = DoRun(true);
                var perfectRun2 = DoRun(true);
                testGames = tempTestGames;
                if (perfectRun1.lose == 0.0 && perfectRun2.lose == 0.0)
                {
                    return true;
                }
            }
            return false;
        }

        private TestRun DoRun(bool first)
        {
            List<char> testResults = new List<char>();
            int playFor = first ? 0 : 1;
            for (int i = 0; i < testGames; i++)
            {
                string board = "NNNNNNNNN";
                int turn = 0;
                char result;
                while (true)
                {
                    if (trainer.installer.HasWinner(board) != 'N' || turn == 9)
                    {
                        result = trainer.installer.HasWinner(board);
                        break;
                    }
                    if (turn % 2 == playFor)
                    {
                        board = trainer.GetBestBoard(board, false);
                    }
                    else
                    {
                        int nextBoard = -1;
                        List<int> nextBoards = trainer.installer.GetNextBoards(board);//kies een random bord
                        if (trainer.HasWinningMove(board) == -1 && nextBoards.Count > 0)//kijk of er een winnende move is
                        {
                            nextBoard = nextBoards[trainer.rand.Next(0, nextBoards.Count)];
                        }
                        //Als er geen volgend speelbord gevonden is resulteert de volgende zet sowieso in winst of gelijkspel
                        if (nextBoard == -1)
                        {
                            result = trainer.DoFinalMove(board);
                            break;
                        }
                        else
                        {
                            board = trainer.installer.boards[nextBoard];
                        }
                    }
                    turn++;
                }
                testResults.Add(result);
            }
            return new TestRun(testResults, first);
        }
    }

    class TestRun
    {
        public int win, draw, lose;
        public TestRun(List<char> testResults, bool first)
        {
            if (first)
            {
                win = (int)((double)testResults.Where(tr => tr == 'X').ToList().Count / testResults.Count * 100.0);
                lose = (int)((double)testResults.Where(tr => tr == 'O').ToList().Count / testResults.Count * 100.0);
            } else
            {
                win = (int)((double)testResults.Where(tr => tr == 'O').ToList().Count / testResults.Count * 100.0);
                lose = (int)((double)testResults.Where(tr => tr == 'X').ToList().Count / testResults.Count * 100.0);
            }
            draw = 100 - win - lose;
        }
    }
}
