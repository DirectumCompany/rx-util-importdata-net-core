using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;

namespace ImportUtilServer.Models
{
    public class Import
    {
        private List<string> outputData = new();
        private bool isImportActive = false;
        private string utilPath = string.Empty;

        public Import(IConfiguration configuration, IHostEnvironment env)
        {
            utilPath = configuration.GetValue<string>("UtilPath");
            if (env.IsProduction())
            {
                utilPath = Path.GetFullPath(utilPath, Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            }

            else if (env.IsDevelopment())
            {
                var solutionRoot = FindSolutionRoot();
                if (solutionRoot == null)
                    throw new Exception("Не удалось найти корень solution (.sln).");
                utilPath = Path.GetFullPath(utilPath, solutionRoot);
            }
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
                catch (Exception ex)
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

        /// <summary>
        /// Найти путь к папке с решением ImportData.
        /// </summary>
        /// <returns>Путь до папки с решением.</returns>
        private static string FindSolutionRoot()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            while (dir != null)
            {
                if (dir.GetFiles("ImportData.sln").Any())
                    return dir.FullName;

                dir = dir.Parent;
            }

            return string.Empty;
        }
    }
}