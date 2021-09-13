using System;
using System.IO;

namespace AudiosBot.Infra.Helpers
{
    public class SystemHelper
    {
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    Console.WriteLine($"APAGANDO {filePath}");

                    File.Delete(filePath);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteFolder(string folderPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
                {
                    Console.WriteLine($"APAGANDO DIRETORIO '{folderPath}'");

                    Directory.Delete(folderPath, true);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
