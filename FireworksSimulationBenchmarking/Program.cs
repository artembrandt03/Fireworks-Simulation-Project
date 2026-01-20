using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fireworks;
using ShapeLibrary;

namespace FireworksSimulationBenchmarking
{
    internal class Program
    {
        private const int WIDTH = 800;
        private const int HEIGHT = 600;
        private const int RUNS_PER_BATCH = 100;

        private static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("[Fireworks Benchmark]");
                Console.WriteLine("1) 100 fireworks");
                Console.WriteLine("2) 1 000 fireworks");
                Console.WriteLine("3) 10 000 fireworks");
                Console.WriteLine("Q) Quit");
                Console.Write("Choose a batch: ");

                string input = Console.ReadLine()?.Trim() ?? "";
                int fireworkCount = 0;

                if (input == "1") fireworkCount = 100;
                else if (input == "2") fireworkCount = 1000;
                else if (input == "3") fireworkCount = 10_000;
                else if (input.Equals("q", StringComparison.OrdinalIgnoreCase)) return;
                else continue;

                Console.WriteLine();
                Console.WriteLine("Choose firework pattern:");
                Console.WriteLine("1) Circle pattern (shape-based)");
                Console.WriteLine("2) Default pattern");
                Console.Write("Pattern: ");

                string patternInput = Console.ReadLine()?.Trim() ?? "";
                int patternChoice = 0;

                if (patternInput == "1") patternChoice = 1;
                else if (patternInput == "2") patternChoice = 2;
                else continue;

                string patternName = patternChoice == 1 ? "circle" : "default";

                Console.WriteLine();
                Console.WriteLine(
                    $"Benchmarking {fireworkCount} {patternName} fireworks ({RUNS_PER_BATCH} runs)...");
                Console.WriteLine("Press ENTER to begin.");
                Console.ReadLine();

                double avg = Benchmark(fireworkCount, RUNS_PER_BATCH, patternChoice);

                Console.WriteLine();
                Console.WriteLine(
                    $"Average time for {fireworkCount} {patternName} fireworks: {avg:F2} ms");
                Console.WriteLine("Press ENTER to return to menu...");
                Console.ReadLine();
            }
        }

        private static double Benchmark(int fireworkCount, int runs, int patternChoice)
        {
            long totalMs = 0;

            for (int run = 1; run <= runs; run++)
            {
                var env = new FireworkEnvironment();

                // create all fireworks for this run
                for (int i = 0; i < fireworkCount; i++)
                {
                    var colour = new Colour(255, 255, 255);
                    IExplosionPattern pattern = CreatePattern(patternChoice, colour);

                    var fw = FireworkFactory.Create(WIDTH, HEIGHT, colour, pattern);
                    env.AddFirework(fw);
                }

                Stopwatch sw = Stopwatch.StartNew();

                // rely on FireworkEnvironment.Update to remove finished fireworks
                while (env.Items.Count > 0)
                {
                    env.Update();
                }

                sw.Stop();
                long elapsed = sw.ElapsedMilliseconds;
                totalMs += elapsed;

                Console.WriteLine($"Run {run,3}: {elapsed} ms");
            }

            return totalMs / (double)runs;
        }

        private static IExplosionPattern CreatePattern(int patternChoice, Colour colour)
        {
            if (patternChoice == 1)
            {
                // Circle pattern (shape-based) – lots of particles
                return ExplosionPatternFactory.CreateCirclePattern(
                    WIDTH / 2f, HEIGHT / 2f, 120f, colour);
            }

            // Default pattern (non shape-based)
            return ExplosionPatternFactory.Create();
        }
    }
}

