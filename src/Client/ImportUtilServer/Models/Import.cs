using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using ImportUtilServer.Controllers;

namespace ImportUtilServer.Models
{
    public class Import
    {
        private List<string> outputData = new();
        private bool isImportActive = false;
        private string utilExePath = string.Empty;

        public Import(IConfiguration configuration)
        {
            utilExePath = configuration.GetValue<string>("UtilExePath");
        }

        public async Task ExecuteAsync(string arguments)
        {
            if (isImportActive)
                return;
            
            ThreadPool.QueueUserWorkItem<object>(_ =>
            {
                try
                {
                    Console.WriteLine($"Import.ExecuteAsync: exePath - {utilExePath} arguments - {arguments}");
                    isImportActive = true;
                    using Process process = new();
                    {
                        process.StartInfo.FileName = utilExePath;
                        //process.StartInfo.WorkingDirectory = @"c:\temp\";
                        process.StartInfo.Arguments = arguments;
                        process.StartInfo.CreateNoWindow = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.OutputDataReceived +=
                            DataReceivedEventHandler; //обработчик события при получении очередной строки с данными
                        process.ErrorDataReceived +=
                            ErrorReceivedEventHandler; //обработчик события при получении ошибки

                        process.Start(); //запускаем процесс
                        process.BeginOutputReadLine(); //начинаем считывать данные из потока 
                        process.BeginErrorReadLine(); //начинаем считывать данные об ошибках 
                        process.WaitForExitAsync().Wait(); //ожидаем окончания работы приложения, чтобы очистить буфер
                        process.Close(); //завершает процесс
                    }
                }
                catch(Exception ex)
                {
                    lock (outputData)
                        outputData.Add(ex.Message);
                }
                finally
                {
                    isImportActive = false;
                }
            }, null, false);
        }

        public IEnumerable<string> GetOutputData()
        {
            lock (outputData)
                return new List<string>(outputData);
        }

        public void ClearOutputData()
        {
            lock (outputData)
                outputData.Clear();
        }

        public bool IsActive() => isImportActive;

        private void DataReceivedEventHandler(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Console.WriteLine(e.Data);
                lock (outputData)
                    outputData.Add(e.Data);
            }
        }

        private void ErrorReceivedEventHandler(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                lock (outputData)
                    outputData.Add(e.Data);
            }
        }
    }
}