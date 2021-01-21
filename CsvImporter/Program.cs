using Azure.Storage.Blobs.Specialized;
using CsvImporter.Data;
using CsvImporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CsvImporter
{
    class Program
    {
        private const string _fileNameStock = "StockStore";

        static async Task<int> Main(string[] args)
        {
            Console.WriteLine(string.Format("Time {0} - Starting CSV Importer ...", DateTime.Now.ToString()));

            CsvImporterDbContext contextStock = new CsvImporterDbContext();
            var db = contextStock.Database.CanConnect();

            if (db)
            {
                Console.WriteLine("Succesful connection to database.");
            }
            else
            {
                Console.WriteLine("Not possible to connect to database, check your configuration.");
                return 0;
            }            

            try
            {
                var result = await DownloadFileStockAsync();
                if (result)
                {
                    Console.WriteLine(string.Format("Time {0} - Download complete.", DateTime.Now.ToString()));
                    var resultProcess = await ProcessStockAsync();
                    if (resultProcess)
                    {
                        Console.WriteLine(string.Format("Time: {0} - Stock Process succesful.", DateTime.Now.ToString()));
                        return 1;
                    }                    
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }           
        }        

        /// <summary>
        /// Download file from a public Azure Storage
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> DownloadFileStockAsync()
        {
            string file = GetFilePath();

            if (File.Exists(file))
            {
                string fileBkp = string.Format("{0}\\Download\\bkp_{1}_{2}.csv", GetApplicationRoot(), DateTime.Now.Ticks, _fileNameStock);
                File.Move(file, fileBkp);
            }

            BlockBlobClient blob = new BlockBlobClient(new Uri(@"https://storage10082020.blob.core.windows.net/y9ne9ilzmfld/Stock.CSV"));
            if (blob.Exists())
            {
                Console.WriteLine("Downloading file...take several minutes, please wait.");
                await blob.DownloadToAsync(file);

                return true;
            }           

            Console.WriteLine("Error into Storage, aborting download.");
            return false;
        }

        /// <summary>
        /// Process a file local to Table Stock
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> ProcessStockAsync()
        {
            var fileLocal = GetFilePath();
            if (File.Exists(fileLocal))
            {
                CsvImporterDbContext contextStock = new CsvImporterDbContext();

                #region Delete all table content
                
                do
                {
                    Console.WriteLine("Deleting data older...");
                    var listDelete = contextStock.Stock.Take(10000).ToList();
                    contextStock.BulkDelete(listDelete); 

                } while (contextStock.Stock.Count() != 0);                

                #endregion

                List<Stock> listStockStore = new List<Stock>();
                int countRow = 0;
                Console.WriteLine("Processing file...take several minutes, please wait.");

                using (StreamReader sr = new StreamReader(fileLocal))
                {
                    string line;
                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (countRow != 0)
                        {
                            string[] data = line.Split(";");

                            Stock newData = new Stock
                            {                               
                                PointOfSale = data[0],
                                Product = data[1],
                                Date = Convert.ToDateTime(data[2]),
                                AvailableQuantity = Convert.ToInt32(data[3])
                            };
                            listStockStore.Add(newData);                            
                        }
                        countRow++;                        
                    }
                }

                if (countRow != 0)
                {
                    Console.WriteLine(string.Format("{0} data will be inserted, please wait a fiew minutes.", countRow - 1));
                    await contextStock.BulkInsertAsync(listStockStore);                    
                    return true;
                }

                return false;
            }
            else
            {
                Console.WriteLine("Not found the file, abort process.");
                return false;
            }            
        }

        /// <summary>
        /// Utilities
        /// </summary>
        /// <returns></returns>
        private static string GetFilePath()
        {
            return string.Format("{0}\\Download\\{1}.csv", GetApplicationRoot(), _fileNameStock);            
        }

        /// <summary>
        /// Utilities
        /// </summary>
        /// <returns></returns>
        private static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);

            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }
}
