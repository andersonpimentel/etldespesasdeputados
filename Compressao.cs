using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace ETLDespesasDeputados
{
    class Compressao
    {
        public static IEnumerable<Origem> Conteudo(FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                }

                var badData = new List<string>();

                using (var decompressedFileStream = File.OpenText(newFileName))
                using (var csvReader = new CsvReader(decompressedFileStream))
                {
                    csvReader.Configuration.CultureInfo = System.Globalization.CultureInfo.InvariantCulture;
                    csvReader.Configuration.Delimiter = ",";
                    var options = new TypeConverterOptions
                    {
                        NumberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign
                    };

                    csvReader.Configuration.TypeConverterOptionsCache.AddOptions(typeof(decimal), options);
                    csvReader.Configuration.BadDataFound = (ReadingContext readingContext) =>
                    {
                        badData.Add(readingContext.RawRecord);
                    };

                    Console.WriteLine("Obtendo registros do CSV");

                    return csvReader.GetRecords<Origem>().ToArray();
                }
            }
        }
    }
}