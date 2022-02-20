using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Saltyfish.Editor
{
    public class LubanEditor
    {
        [MenuItem("LubanTools/Generate ExcelTable Config Data")]
        public static void GenerateExcelConfig()
        {
            if (!SystemInfo.operatingSystem.Contains("Win"))
            {
                EditorUtility.DisplayDialog("Tips", "Only support windows now", "ok");
                return;
            }
            var LogSb = new StringBuilder();
            var ErrorSb = new StringBuilder();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.WorkingDirectory = GetLubanBatchFilePath();
            startInfo.RedirectStandardError = true;
            startInfo.FileName = Path.Combine(GetLubanBatchFilePath(),"gen_code_json.bat");
            Process process = Process.Start(startInfo);
            process.OutputDataReceived += (sender, e) => { LogSb.AppendLine(e.Data); };
            process.ErrorDataReceived += (sender, e) => { ErrorSb.AppendLine( e.Data); };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.StandardInput.WriteLine("echo on");
            process.StandardInput.WriteLine(GetLubanBatchFilePath());
            EditorUtility.DisplayProgressBar("Hold on", "Generating config data", 0.5f);
            process.WaitForExit();

            Debug.Log(LogSb);

            var errorMessage = ErrorSb.ToString();
            if(!string.IsNullOrWhiteSpace(errorMessage))
                Debug.LogError(errorMessage);
            EditorUtility.DisplayDialog("Tip", "Generate done", "Ok");
            EditorUtility.ClearProgressBar();
        }

        private static string GetLubanBatchFilePath()
        {
            var rootPath = Directory.GetParent(Application.dataPath).Parent.FullName;
            var result = Path.Combine(rootPath, "LubanTools");
            return result;
        }
    }
}
