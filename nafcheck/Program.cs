using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nafcheck
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = args[0];
            string monitorFile = args[1];

            inputFile = @"S:\workspaces\nafcheck\NifExport.pc.naf";
            monitorFile = @"S:\workspaces\nafcheck\NifExport.pc.naf.CSV";

            Console.WriteLine("Parsing file...");

            string[] monitorLines = File.ReadAllLines(monitorFile);

            List<FileAccess> fileAccesses = new List<FileAccess>();

            foreach (string monitorLine in monitorLines)
            {
                string[] lineParts = monitorLine.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                lineParts[0] = lineParts[0].Substring(1);
                lineParts[lineParts.Length - 1] = lineParts[lineParts.Length - 1].Substring(0, lineParts[lineParts.Length - 1].Length - 1);
                if (lineParts.Length >= 4
                    && (lineParts[3] == "FASTIO_READ"
                        || lineParts[3] == "IRP_MJ_READ")
                    )
                {
                    lineParts[6] = lineParts[6].Replace("Offset: ", "").Replace("Length: ", "");
                    string[] readParts = lineParts[6].Split(new string[] { ", " }, StringSplitOptions.None);

                    int offset = int.Parse(readParts[0].Replace(",",""));
                    int length = int.Parse(readParts[1].Replace(",", ""));

                    fileAccesses.Add(new FileAccess(offset, length));
                }
            }
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(inputFile)))
            {
                int previousPLV = 0;
                foreach (FileAccess fileAccess in fileAccesses)
                {
                    binaryReader.BaseStream.Seek(fileAccess.Offset, SeekOrigin.Begin);
                    binaryReader.Read(fileAccess.ByteContent, 0, fileAccess.Length);

                    if (previousPLV == fileAccess.Length)
                    {
                        fileAccess.LengthFromPrevious = true;
                    }

                    if (fileAccess.Length == 1)
                    {
                        fileAccess.PotentialLengthValue = fileAccess.ByteContent[0];
                    }
                    else if (fileAccess.Length == 2)
                    {
                        fileAccess.PotentialLengthValue = BitConverter.ToInt16(fileAccess.ByteContent, 0);
                    }
                    else if (fileAccess.Length == 4)
                    {
                        fileAccess.PotentialLengthValue = BitConverter.ToInt32(fileAccess.ByteContent, 0);
                    }

                    Console.Write("Offset: {0},", fileAccess.Offset);
                    if (fileAccess.LengthFromPrevious)
                    {
                        Console.Write("    Length: *{0}*", fileAccess.Length);
                    }
                    else
                    {
                        Console.Write("    Length: {0}", fileAccess.Length);
                    }
                    Console.Write("    PLV: {0},", fileAccess.PotentialLengthValue);
                    if (fileAccess.PotentialLengthValue == 42)
                    {
                        Console.Write(" * TERMINATOR");
                    }
                    Console.WriteLine();

                   previousPLV = fileAccess.PotentialLengthValue;
                }
            }

            Console.WriteLine("Complete.");
            Console.ReadLine();
        }
    }
}
