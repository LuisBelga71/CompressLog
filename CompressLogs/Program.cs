using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace CompressLogs
{
    class Program
    {
        //*************************************************** EXAMPLE COMMAND LINE ***************************************************
        // 7z.exe a -t7z -ms=e5f -sdel "c:\LogsComprimidos\QponAPI\2022-02-23.7z" "c:\apps\.....LOG\QponApi-log-*-2022-02-22*.log" 
        //****************************************************************************************************************************

        static void Main(string[] args)
        {
            Process process = null;

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

                string path7z = args[0].Replace('@', ' ');
                string filescompressPath = args[1];   // @"D:\Apps\Qpon\Logs\Api\log-*-2022-02-22.log";
                string destinationPath = args[2];     //@"D:\LogsComprimidos\QponAPI\2022-02-22.7z";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = path7z, //@"D:\Program Files\7-Zip\7z.exe",

                    Arguments = "a -t7z -ms=e5f -sdel \"" + destinationPath + "\" \"" + filescompressPath + "\"",

                    WindowStyle = ProcessWindowStyle.Hidden
                };
            
                Logger.LoggerInstance.Info(string.Format("ProcessStartInfo, arguments: {0} , fileName: {1}", psi.Arguments, psi.FileName));

                Logger.LoggerInstance.Info("Process Start");

                process = Process.Start(psi);

                Logger.LoggerInstance.Info("Process WaitForExit");

                process.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.LoggerInstance.Error("Win32Exception: " + ex.ToString());
                Environment.Exit(-2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.LoggerInstance.Error("Exception: " + ex.ToString());
                Environment.Exit(-2);
            }
            finally 
            {
                Logger.LoggerInstance.Info("Process dispose");
                process?.Dispose();
            }       
        }
    }
}
