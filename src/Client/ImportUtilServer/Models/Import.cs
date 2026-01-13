using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using ImportUtilServer.Controllers;

namespace ImportUtilServer.Models
{
    public class Import
    {
        private List<string> outputData = new();
        private bool isImportActive = false;
        private string utilPath = string.Empty;

        public Import(IConfiguration configuration)
        {
            utilPath = configuration.GetValue<string>("UtilPath");
            utilPath = Path.GetFullPath(utilPath, Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        }

        public void Execute(string arguments)
        {
            if (isImportActive)
                return;
            
            ThreadPool.QueueUserWorkItem<object>(_ =>
            {
                try
                {
                    Console.WriteLine($"Import.Execute: path - {utilPath} arguments - {arguments}");
                    isImportActive = true;
                    if (!File.Exists(utilPath))
                        throw new FileNotFoundException($"Утилита импорта по пути {utilPath} не найдена");
                    using Process process = new(); 
                    process.StartInfo.FileName = RuntimeInformation
                        .IsOSPlatform(OSPlatform.Windows) ? "dotnet.exe" : "dotnet";
                    process.StartInfo.Arguments = $"\"{utilPath}\" {arguments}";
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