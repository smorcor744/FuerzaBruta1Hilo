using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace FuerzaBrutaMultiHilo
{
    public class FuerzaBruta
    {
        private string passwordHash;
        private string password;
        private string[] passwords;
        private string filePath = "C:\\2ºDAM\\PSP\\FuerzaBruta1Hilo\\FuerzaBruta1Hilo\\password.txt";
        private Random random = new Random();
        private int optimalThreads = 1;

        public FuerzaBruta()
        {
            passwords = File.ReadAllLines(filePath);
            password = passwords[random.Next(0, passwords.Length)];
            passwordHash = HashPassword(password);
        }

        public void Start()
        {
            Console.WriteLine($"El hash de la contraseña es {passwordHash}");
            Console.WriteLine($"La contraseña es {password}");

            int numThreads = 1;
            long previousTime = long.MaxValue;

            while (true)
            {
                long timeTaken = MeasureExecutionTime(numThreads);
                Console.WriteLine($"Hilos: {numThreads}, Tiempo: {timeTaken} ms");

                if (timeTaken > previousTime)
                {
                    optimalThreads = numThreads - 1;
                    break;
                }

                previousTime = timeTaken;
                numThreads++;
            }

            Console.WriteLine($"Número óptimo de hilos: {optimalThreads}");
        }

        private long MeasureExecutionTime(int threadCount)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Thread[] threads = new Thread[threadCount];
            bool found = false;
            object lockObj = new object();
            int chunkSize = passwords.Length / threadCount;

            for (int i = 0; i < threadCount; i++)
            {
                int start = i * chunkSize;
                int end = (i == threadCount - 1) ? passwords.Length : start + chunkSize;

                threads[i] = new Thread(() =>
                {
                    for (int j = start; j < end; j++)
                    {
                        if (found) break;
                        if (HashPassword(passwords[j]) == passwordHash)
                        {
                            lock (lockObj)
                            {
                                found = true;
                            }
                            break;
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
