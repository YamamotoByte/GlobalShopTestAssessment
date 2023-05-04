using System;
using System.IO;
using System.Linq;

namespace FileMergeExample
{    class PartData
    {
        public int PartID { get; set; }
        public string PartDesc { get; set; }
        public decimal Price { get; set; }
    }

    class PartReprice
    {
        public int PartID { get; set; }
        public decimal Price { get; set; }
    }

    class MergedData
    {
        public int PartID { get; set; }
        public string PartDesc { get; set; }
        public decimal Price { get; set; }
    }

    class Program
    {
        const string Separator = "*!*";

        static void Main(string[] args)
        {
            // Get file paths
            Console.WriteLine("Enter the name of the first file:");
            string partDataFile = Console.ReadLine();
            Console.WriteLine("Enter the name of the second file:");
            string partRepriceFile = Console.ReadLine();
            Console.WriteLine("Enter the name of the output file:");
            string outputFile = Console.ReadLine();

            // Read and merge files
            try
            {
                var partData = ReadPartDataFile(partDataFile);
                var partReprice = ReadPartRepriceFile(partRepriceFile);
                var mergedData = MergePartDataAndReprice(partData, partReprice);
                WriteMergedDataToFile(outputFile, mergedData);
                Console.WriteLine("Merge complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        // Read part data file
        static PartData[] ReadPartDataFile(string filename)
        {
            return File.ReadAllLines(filename)
                .Skip(1) // skip header row
                .Select(line =>
                {
                    var fields = line.Split(Separator);
                    return new PartData
                    {
                        PartID = int.Parse(fields[0]),
                        PartDesc = fields[1],
                        Price = decimal.Parse(fields[2])
                    };
                })
                .ToArray();
        }

        // Read part reprice file
        static PartReprice[] ReadPartRepriceFile(string filename)
        {
            return File.ReadAllLines(filename)
                .Skip(1) // skip header row
                .Select(line =>
                {
                    var fields = line.Split(Separator);
                    return new PartReprice
                    {
                        PartID = int.Parse(fields[0]),
                        Price = decimal.Parse(fields[1])
                    };
                })
                .ToArray();
        }

        // Return merged data
        static MergedData[] MergePartDataAndReprice(PartData[] partData, PartReprice[] partReprice)
        {
            return partData
                .Join(partReprice, pd => pd.PartID, pr => pr.PartID, (pd, pr) => new MergedData
                {
                    PartID = pd.PartID,
                    PartDesc = pd.PartDesc,
                    Price = pr.Price
                })
                .ToArray();
        }

        // Write merged data to file
        static void WriteMergedDataToFile(string filename, MergedData[] mergedData)
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine("PartID{0}PartDesc{0}Price", Separator);
                foreach (var item in mergedData)
                {
                    writer.WriteLine("{0}{1}{2}{1}{3}", item.PartID, Separator, item.PartDesc, item.Price);
                }
            }
        }
    }
}