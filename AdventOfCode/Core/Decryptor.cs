using System.Diagnostics;

namespace AdventOfCode.Core
{
    internal static class Decryptor
    {

        private static readonly string GPG = "gpg";
        private static readonly string ARGS = "--quiet --batch --yes --decrypt --passphrase \"{2}\" --output {1} {0}";
        private static readonly string AOC_INPUTS_ENV_VAR = "AOC_INPUTS";

        internal static void DecryptFile(string encryptedFilePath, string resultFilePath)
        {
            Console.WriteLine("Decrypting file " + encryptedFilePath);

            string? inputPass = Environment.GetEnvironmentVariable(AOC_INPUTS_ENV_VAR);

            using Process process = new();

            process.StartInfo.FileName = GPG;
            process.StartInfo.Arguments = string.Format(ARGS, encryptedFilePath, resultFilePath, inputPass);
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            
            process.Start();
                        
            process.WaitForExit();
        }


    }
}
