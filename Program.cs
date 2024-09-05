using System.Diagnostics;

namespace MultiThreads
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] sizes = { 100000, 1000000, 10000000 };

            foreach (var size in sizes)
            {
                int[] array = GenerateArray(size);

                Console.WriteLine($"Array size: {size}");

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                long sum = SequentalSum(array);
                stopwatch.Stop();
                Console.WriteLine($"Sequential sum: {sum}, Time: {stopwatch.ElapsedMilliseconds} ms");

                stopwatch.Restart();
                sum = ParallelSumWithThreads(array);
                stopwatch.Stop();
                Console.WriteLine($"Parallel sum with threads: {sum}, Time: {stopwatch.ElapsedMilliseconds} ms");

                stopwatch.Restart();
                sum = ParallelSum(array);
                stopwatch.Stop();
                Console.WriteLine($"Parallel sum with LINQ: {sum}, Time: {stopwatch.ElapsedMilliseconds} ms");
            }
        }


        public static int[] GenerateArray(int size)
        {
            var random = new Random();
            var array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = random.Next(0, 100);
            }
            return array;
        }

        public static long SequentalSum(int[] array)
        {
            long sum = 0;
            foreach (var item in array)
            {
                sum += item;
            }
            return sum;
        }

        static long ParallelSumWithThreads(int[] array)
        {
            int threadCount = Environment.ProcessorCount;
            var threads = new List<Thread>();
            long sum = 0;
            int chunkSize = array.Length / threadCount;
            object lockObject = new object();

            for (int i = 0; i < threadCount; i++)
            {
                int start = i * chunkSize;
                int end = (i == threadCount - 1) ? array.Length : start + chunkSize;
                threads.Add(new Thread(() =>
                {
                    long partialSum = 0;
                    for (int j = start; j < end; j++)
                    {
                        partialSum += array[j];
                    }
                    lock (lockObject)
                    {
                        sum += partialSum;
                    }
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return sum;
        }

        static long ParallelSum(int[] array)
        {
            return array.AsParallel().Sum();
        }

    }
}
