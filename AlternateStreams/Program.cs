using System;
using System.IO;

namespace AlternateStreams
{
    class Program
    {
        private const string File = "sample.txt";
        private const string AltFile = "hidden.txt";

        static void Main(string[] args)
        {
            // Create base file
            using var fs = System.IO.File.OpenWrite(File);
            using var sw = new StreamWriter(fs);
            sw.Write("Nothing here.");
            sw.Close();
            fs.Close();

            // Write to stream
            AlternateStreamsApi.WriteAlternateStream(File, AltFile, "Baf!");

            // Read from stream
            Console.WriteLine($"Data: {AlternateStreamsApi.ReadAlternateStream(File, AltFile)}");
            
            Console.ReadLine();
        }
    }
}
