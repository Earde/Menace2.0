using System;

namespace Menace2._0
{
    class Program
    {
        static bool doTestRuns = false;
        static bool tryIsPerfect = false;
        static int testGames = 500;
        static int runTestEveryXGames = 100;

        static int trainGames = 10000;
        static bool printTrainOutput = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Menace, the Tic Tac Toe bot. Made by Earde Bearda.");
            Console.WriteLine("Press enter to start.");
            Console.ReadLine();
            Installer installer = new Installer();
            installer.Run();

            Trainer trainer = new Trainer(installer);
            trainer.Init();

            Console.WriteLine("Training " + trainGames.ToString() + " Games...");

            Tester tester = new Tester(trainer, testGames, runTestEveryXGames);
            Console.Write("Do you wish to collect test results while training? (y/n): ");
            string get = Console.ReadLine();
            if (get.ToLower().Equals("y"))
            {
                doTestRuns = true;
                Console.Write("Do you wish to stop training if player 1 and 2 don't lose for more than 10.000 games in a row? (y/n): ");
                get = Console.ReadLine();
                if (get.ToLower().Equals("y"))
                {
                    tryIsPerfect = true;
                }
            }
            if (doTestRuns)
            {
                Console.WriteLine("Every " + runTestEveryXGames.ToString() + " train games run " + (testGames * 2).ToString() + " test games.");
                Console.WriteLine("Running a total of " + (trainGames * (testGames * 2 / runTestEveryXGames)).ToString() + " games...");
                tester.Run();
            }
            int loading = 0;
            for (int i = 0; i < trainGames; i++)
            {
                //loading indicator
                int curLoading = (int)((double)i / (double)trainGames * 100.0);
                if (curLoading > loading)
                {
                    loading = curLoading;
                    Console.Write("\r{0}%     ", loading);
                }
                //play train game
                trainer.PlayTrainGame();
                //do test run
                if (doTestRuns)
                {
                    if (i % runTestEveryXGames == 0)
                    {
                        tester.Run();
                    }
                    if (tryIsPerfect && tester.IsPerfect())
                    {
                        Console.WriteLine("Perfect Solution Found");
                        break;
                    }
                }
            }
            Console.WriteLine("Done training.");
            //print test output
            if (doTestRuns)
            {
                tester.CreateCsv();
            }
            //print training output
            if (printTrainOutput)
            {
                trainer.PrintOutput();
            }
            Console.WriteLine(" ");
            //play game
            Console.WriteLine("Enter exit to quit.");
            Console.WriteLine("Enter board (ex. Input: NONXXNNON): or Press Enter to continue with current board.");
            string input = "NNNNNNNNN";
            do
            {
                string best = trainer.GetBestBoard(input, true);
                string rotated = installer.RotateBack(input, best);
                Console.WriteLine(rotated[0].ToString() + rotated[1].ToString() + rotated[2].ToString());
                Console.WriteLine(rotated[3].ToString() + rotated[4].ToString() + rotated[5].ToString());
                Console.WriteLine(rotated[6].ToString() + rotated[7].ToString() + rotated[8].ToString());
                Console.Write("Input: ");
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    input = rotated;
                }
            } while (input.ToLower() != "exit");
        }
    }
}
