using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using System.IO;

namespace Martins_Code
{

    public class Main
    {
        public bool hasInternet;
        public bool fileIsCorrupt;
        public string fileDestination;
        public string fileURL;
        private string fileName;
        public string targetName;
        public string strTargetID;
        public Int32 targetID;
        public Int32 targetThreadCount;

        public Int64 downloadSize;
        public Int64 downloadedSize;

        private WebClient webClient;
        private WebRequest request;
        private WebResponse response;

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedVal);

        public bool checkInternet()
        {
            int desc;
            if (InternetGetConnectedState(out desc, 0))
            {
                return InternetGetConnectedState(out desc, 0);
            }
            else
            {
                if (MessageBox.Show("404 Error: Internet not found!", "Critical Stop", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, true) == DialogResult.Retry)
                {
                    checkInternet();
                }
                else
                {
                    MessageBox.Show("404 Error: Internet not found!", "Critical Stop", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, true);
                }
                hasInternet = InternetGetConnectedState(out desc, 0);

                return InternetGetConnectedState(out desc, -1);
            }
        }

        public void checkDownload()
        {

        }

        public void downloadFile(Uri url)
        {
            webClient.DownloadFileAsync(url, fileDestination);
            response.
            request.GetResponse();
            downloadSize = response.ContentLength;
        }

        public void downloadTick()
        {


        }

        public void checkFile(string path)
        {
            if (!System.IO.File.Exists(path))
            {

            }
        }

        public void runFile(string target)
        {
            Process targetProcess = new Process();
            targetProcess.StartInfo.FileName = target;
            targetName = targetProcess.ProcessName;
            targetID = targetProcess.Id;
            strTargetID = Convert.ToString(targetProcess.Id);
            targetProcess.Start();
        }

        public void runFile(Process targetProcess)
        {
            targetProcess = new Process();
            targetName = targetProcess.ProcessName;
            targetID = targetProcess.Id;
            strTargetID = Convert.ToString(targetProcess.Id);
            targetProcess.Start();
        }

        public void runFile(Process targetProcess, string agruments)
        {
            targetProcess = new Process();
            targetName = targetProcess.ProcessName;
            targetID = targetProcess.Id;
            strTargetID = Convert.ToString(targetProcess.Id);
            targetProcess.Threads = targetProcess;
            targetProcess.Start(arguments);
        }
    }
}
