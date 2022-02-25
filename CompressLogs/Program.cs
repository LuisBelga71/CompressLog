using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CompressLogs
{
    class Program
    {
        //*************************************************** EXAMPLE COMMAND LINE ***************************************************
        // 7z.exe a -t7z -ms=e5f -sdel "c:\LogsComprimidos\QponAPI\2022-02-23.7z" "c:\apps\.....LOG\QponApi-log-*-2022-02-22*.log" 
        //****************************************************************************************************************************

        static void Main(string[] args)
        {
            Logger.LoggerInstance.Info("Start CompressLogs");

            try
            {

                if (args is null || args.Length == 0) 
                {
                    Console.WriteLine("There are not parameters");
                    Logger.LoggerInstance.Info("There are not parameters");
                    Environment.Exit(-1);
                }

                Logger.LoggerInstance.Info("args: " + JsonConvert.SerializeObject(args));

                string filescompressPath = args[0];
                string destinationPath = args[1]; 

                destinationPath = Path.Combine(destinationPath, args[0].Split('\\').Last());

                Logger.LoggerInstance.Info("destinationPath: " + destinationPath);
                Logger.LoggerInstance.Info("filescompressPath: " + filescompressPath);

                var files = Directory.GetFiles(filescompressPath).Where(f => f.EndsWith(".log", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).ToList();

                List<string> datesProcessedFiles = new List<string>();

                foreach (var file in files)
                {
                    var fileName = file.Split('\\').Last();
                    fileName = fileName.Replace("log-all-db-", string.Empty).Replace("log-own-", string.Empty).Replace("log-error-", string.Empty).Replace("QponApi-", string.Empty).Replace("QPON_QponExChangeApi-", string.Empty).Replace("QponScheduleTask-", string.Empty).Replace("QPON_WWW-", string.Empty);

                    string date = fileName.Split('.')[0];

                    if (date.Split('-').Length > 3) 
                    {
                        date = date.Replace("-" + date.Split('-')[3], string.Empty);
                    }

                    //string pathFiles = Path.Combine(filescompressPath, "*log-*-" + date + "*.log OR *.txt");
                    string pathFiles = Path.Combine(filescompressPath, "*log-*-" + date + "*.*");
                    string pathDestination = Path.Combine(destinationPath, date + ".7z");

                    DateTime.TryParse(date, out DateTime todayDate);
                    if (!datesProcessedFiles.Contains(date) && DateTime.Now.Date != todayDate.Date) 
                    {
                        datesProcessedFiles.Add(date);
                        CompressFile(pathDestination, pathFiles);
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.LoggerInstance.Error("Exception: " + ex.ToString());
            }      
        }
        private static void CompressFile(string destinationPath, string filescompressPath)
        {
            Process process = null;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "7z.exe",

                    Arguments = @"a -t7z -ms=e5f -sdel " + destinationPath + " " + filescompressPath,

                    WindowStyle = ProcessWindowStyle.Hidden,
                                   
                };

                Logger.LoggerInstance.Info(string.Format("ProcessStartInfo, arguments: {0} , fileName: {1}", psi.Arguments, psi.FileName));

                Logger.LoggerInstance.Info("Process Start");

                process = Process.Start(psi);

                process.PriorityClass = ProcessPriorityClass.BelowNormal;

                Logger.LoggerInstance.Info("Process WaitForExit");

                process.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.LoggerInstance.Error("Win32Exception: " + ex.ToString());
                throw ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.LoggerInstance.Error("Exception: " + ex.ToString());
                throw ex;
            }
            finally
            {
                Logger.LoggerInstance.Info("Process dispose");
                process?.Dispose();
            }
        }
    }
}
