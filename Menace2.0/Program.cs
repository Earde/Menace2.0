using System;

namespace Menace2._0
{
    class Program
    {
        static bool doTestRuns = true;

        static int trainGames = 1000;
        static bool printTrainOutput = true;

        static void Main(string[] args)
        {
            Installer installer = new Installer();
            installer.Run();

            Trainer trainer = new Trainer(installer);
            trainer.Init();

            Tester tester = new Tester(trainer);

            Console.WriteLine("Training " + trainGames.ToString() + " Games...");
            if (doTestRuns)
            {
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
                    Console.WriteLine(loading.ToString() + "%");
                }
                //play train game
                trainer.PlayTrainGame();
                //do test run
                if (doTestRuns)
                {
                    tester.Run();
                }
            }
            Console.WriteLine("Done training.");
            //print test output
            if (doTestRuns)
            {
                tester.PrintOutput();
                tester.CreateCsv();
            }
            //print training output
            if (printTrainOutput)
            {
                trainer.PrintOutput();
            }
            //play game
            Console.WriteLine("Enter exit to quit.");
            Console.Write("Enter board (ex. NONXXNNON): ");
            string input = Console.ReadLine();
            while (input.ToLower() != "exit")
            {
                string best = trainer.GetBestBoard(input);
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
            }
        }
    }
}
