using System;

namespace Menace2._0
{
    class Program
    {
        static void Main(string[] args)
        {
            Trainer trainer = new Trainer();
            trainer.Run();
            Console.WriteLine("Enter board please: ");
            string input = Console.ReadLine();
            while (input.ToLower() != "exit")
            {
                Console.WriteLine("Do: " + trainer.GetBestBoard(input));
                input = Console.ReadLine();
            }
        }
    }
}
