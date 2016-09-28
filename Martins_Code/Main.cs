using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace Martins_Code
{

    public class Main
    {

        public bool fileIsCorrupt;

        private volatile string stopReason; // idk if this needs to be volatile

        private string lastSector;

        public string errorMSG;
        public string fatalErrorMSG;
        public string warningMSG;
        public string filePath;
        public string fileURL;
        public string fileExtentsion;
        public string targetName;
        public string strTargetID;

        public ProcessThreadCollection targetThreadCollection;

        private FileInfo downloadedFileInfo;
        private FileInfo uploadedFileInfo;

        public double percent;

        private Int32 errorCode;

        public volatile Int32 percentInt; // idk if this needs to be volatile

        public Int32 targetID;
        public Int32 targetThreadCount;
        public Int32 timeout;

        public Int64 downloadSize;
        public Int64 downloadedSize;
        public Int64 uploadSize;
        public Int64 uploadedSize;

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
                    stopReason = "An internet connection could not be established.";
                    errorCode = 0x00000001;
                    MessageBox.Show("The download/upload could not complete." + Environment.NewLine + "Reason: " + stopReason + Environment.NewLine + "errorCode: " + errorCode, "Critical Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, true);
                }
                return InternetGetConnectedState(out desc, -1);
            }
        }

        public bool isTargetWebsiteUp(Uri targetWebsite)
        {
            string[] sector = targetWebsite.Segments;
            string strURL = targetWebsite.ToString();
            char[] delimeterChars = sector[0].ToCharArray();
            sector = strURL.Split(delimeterChars);

            for (int i = 0; i < sector.Length; i++)
            {
                Console.WriteLine(String.Format("Sector{0}: " + sector[i], i));
            }
            lastSector = sector[sector.Length];
            fileExtentsion = lastSector.Split('.')[lastSector.Split('.').Length];
            Ping ping = new System.Net.NetworkInformation.Ping();

            var result = ping.Send(sector[2]);
            if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
            {
                errorCode = -1;
                Console.WriteLine(result.Address + " failed to relpy");
                return false;
            }
            else
            {
                Console.WriteLine(result.Address + " replied in " + result.RoundtripTime + "ms" + Environment.NewLine + "Reason: " + result.Status);
                return true;
            }
        }

        //TODO if string != nullorwhitspace return false;
        public bool isTargetWebsiteUp(string targetWebsite)
        {
            Uri url = new Uri(targetWebsite);
            string[] sector = url.Segments;
            string strURL = url.ToString();
            char[] delimeterChars = sector[0].ToCharArray();
            sector = strURL.Split(delimeterChars);

            for (int i = 0; i < sector.Length; i++)
            {
                Console.WriteLine(String.Format("Sector{0}: " + sector[i], i));
            }
            lastSector = sector[sector.Length - 1];
            fileExtentsion = lastSector.Split('.')[lastSector.Split('.').Length - 1];
            Ping ping = new System.Net.NetworkInformation.Ping();

            var result = ping.Send(sector[2]);
            if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
            {
                errorCode = -1;
                Console.WriteLine(result.Address + " failed to relpy" + Environment.NewLine + "Reason: " + result.Status);
                return false;
            }
            else
            {
                Console.WriteLine(result.Address + " replied in " + result.RoundtripTime + "ms" + Environment.NewLine + "Reason: " + result.Status);
                return true;
            }
        }

        public void ping(Uri targetWebsite)
        {
            string[] sector = targetWebsite.Segments;
            string strURL = targetWebsite.ToString();
            char[] delimeterChars = sector[0].ToCharArray();
            sector = strURL.Split(delimeterChars);
            for (int i = 0; i < sector.Length; i++)
            {
                Console.WriteLine(String.Format("Sector{0}: " + sector[i], i));
            }
            lastSector = sector[sector.Length];// maybe remove from ping();
            fileExtentsion = lastSector.Split('.')[lastSector.Split('.').Length];
            Ping ping = new System.Net.NetworkInformation.Ping();

            var result = ping.Send(sector[2]);
            if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
            {
                errorCode = -1;
                Console.WriteLine(result.Address + " failed to relpy" + Environment.NewLine + "Reason: " + result.Status);
            }
            else
            {
                Console.WriteLine(result.Address + " replied in " + result.RoundtripTime + "ms");
            }
        }

        public void ping(string targetWebsite)
        {
            Uri url = new Uri(targetWebsite);
            string[] sector = url.Segments;
            string strURL = url.ToString();
            char[] delimeterChars = sector[0].ToCharArray();
            sector = strURL.Split(delimeterChars);
            for (int i = 0; i < sector.Length; i++)
            {
                Console.WriteLine(String.Format("Sector{0}: " + sector[i], i));
            }
            lastSector = sector[sector.Length];// maybe remove from ping();
            fileExtentsion = lastSector.Split('.')[lastSector.Split('.').Length];
            Ping ping = new System.Net.NetworkInformation.Ping();

            var result = ping.Send(sector[2]);
            if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
            {
                errorCode = -1;
                Console.WriteLine(result.Address + " failed to relpy" + Environment.NewLine + "Reason: " + result.Status);
            }
            else
            {
                Console.WriteLine(result.Address + " replied in " + result.RoundtripTime + "ms");
            }
        }

        public void downloadFileHttp(Uri url)
        {
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Get;
            downloadSize = response.ContentLength;
            webClient.DownloadFile(url, filePath);
            response.Close();
        }

        public void downloadFileHttp(string url)
        {
            Uri _url = new Uri(url);
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Get;
            downloadSize = response.ContentLength;
            webClient.DownloadFile(_url, filePath);
            response.Close();
        }

        public void downloadFileHttpAsnyc(Uri url)
        {
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Get;
            downloadSize = response.ContentLength;
            webClient.DownloadFileAsync(url, filePath);
            response.Close();
        }

        public void downloadFileHttpAsync(string url)
        {
            Uri _url = new Uri(url);
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(_url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Get;
            downloadSize = response.ContentLength;
            webClient.DownloadFileAsync(_url, filePath);
            response.Close();
        }

        public void downloadFileFtp(Uri url)
        {
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            downloadSize = response.ContentLength;
            webClient.DownloadFile(url, filePath);
            response.Close();
        }

        public void downloadFileFtp(string url)
        {
            Uri _url = new Uri(url);
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(_url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            downloadSize = response.ContentLength;
            webClient.DownloadFile(_url, filePath);
            response.Close();
        }

        public void downloadFileFtpAsync(Uri url)
        {
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            downloadSize = response.ContentLength;
            webClient.DownloadFileAsync(url, filePath);
            response.Close();
        }

        public void downloadFileFtpAsync(string url)
        {
            Uri _url = new Uri(url);
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(_url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            downloadSize = response.ContentLength;
            webClient.DownloadFileAsync(_url, filePath);
            response.Close();
        }

        public void uploadFileHttp(Uri url)
        {
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Post;
            uploadedSize = response.ContentLength;
            webClient.UploadFileAsync(url, filePath);
            response.Close();
        }

        public void uploadFileHttp(string url)
        {
            Uri _url = new Uri(url);
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(_url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Post;
            uploadedSize = response.ContentLength;
            webClient.UploadFile(_url, filePath);
            response.Close();
        }

        public void uploadReplacementFileHttp(Uri url)
        {
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Put;
            uploadedSize = response.ContentLength;
            webClient.UploadFile(url, filePath);
            response.Close();
        }

        public void uploadReplacementFileHttp(string url)
        {
            Uri _url = new Uri(url);
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(_url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Put;
            uploadedSize = response.ContentLength;
            webClient.UploadFile(_url, filePath);
        }

        public void uploadFileHttpAsync(Uri url)
        {
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Post;
            uploadedSize = response.ContentLength;
            webClient.UploadFileAsync(url, filePath);
            response.Close();
        }

        public void uploadFileHttpAsync(string url)
        {
            Uri _url = new Uri(url);
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(_url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Post;
            uploadedSize = response.ContentLength;
            webClient.UploadFileAsync(_url, filePath);
            response.Close();
        }

        public void uploadReplacementFileHttpAsync(Uri url)
        {
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Put;
            uploadedSize = response.ContentLength;
            webClient.UploadFileAsync(url, filePath);
            response.Close();
        }

        public void uploadReplacementFileHttpAsync(string url)
        {
            Uri _url = new Uri(url);
            WebClient webClient;
            WebRequest request;
            WebResponse response;
            webClient = new WebClient();
            request = WebRequest.Create(_url);
            response = request.GetResponse();
            request.Method = WebRequestMethods.Http.Put;
            uploadedSize = response.ContentLength;
            webClient.UploadFileAsync(_url, filePath);
            response.Close();
        }

        public void downloadTick()
        {
            downloadedFileInfo = new FileInfo(filePath);
            downloadedSize = downloadedFileInfo.Length;
            percent = (double)((downloadedSize / downloadSize) * 100);
            percentInt = (int)Math.Truncate(percent);
            Debug.WriteLine("percent: " + percent + "%" + " int verison: " + percentInt + "%" + " Download Size(Bytes): " + downloadSize + " Downloaded Size(Bytes): " + downloadedSize);
        }

        public void uploadTick()
        {
            uploadedFileInfo = new FileInfo(filePath);
            uploadSize = uploadedFileInfo.Length;
            percent = (uploadedSize / uploadSize) * 100;
            percentInt = (int)Math.Truncate(percent);
            Debug.WriteLine("percent: " + percent + "%" + " int verison: " + percentInt + "%" + " Upload Size(Bytes): " + uploadSize + " Uploaded Size(Bytes): " + uploadedSize);
        }

        public void checkDownloadedFile(string path)
        {
            downloadedFileInfo = new FileInfo(filePath);

            if (!(System.IO.File.Exists(path)) || downloadedFileInfo.Length != downloadSize)
            {
                fileIsCorrupt = true;
            }
            else
            {
                fileIsCorrupt = false;
            }
        }

        public void checkUploadedFile(Uri url)
        {
            //<TODO>Research on how to make this a working fuction</TODO>  
        }

        public void checkUploadedFile(string url)
        {
           //<TODO>Research on how to make this a working fuction</TODO>  
        }

        public bool runFile(Process targetProcess)
        {
            targetProcess = new Process();

            try
            {
                targetProcess.Start();
                targetName = targetProcess.ProcessName;
                targetID = targetProcess.Id;
                strTargetID = Convert.ToString(targetProcess.Id);
                targetThreadCount = Convert.ToInt32(targetProcess.Threads);
            }
            catch (Win32Exception win32)
            {
                warningMSG = "[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + win32.StackTrace + Environment.NewLine + "Source:" + win32.Source + ".dll" + " HResult: " + win32.HResult + Environment.NewLine + "Reason: " + win32.Message + Environment.NewLine;
                Debug.WriteLine("[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + win32.StackTrace + Environment.NewLine + "Source:" + win32.Source + ".dll" + " HResult: " + win32.HResult + Environment.NewLine + "Reason: " + win32.Message);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                warningMSG = "[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ioe.StackTrace + Environment.NewLine + "Source:" + ioe.Source + ".dll" + " HResult: " + ioe.HResult + Environment.NewLine + "Reason: " + ioe.Message + Environment.NewLine;
                Debug.WriteLine("[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ioe.StackTrace + Environment.NewLine + "Source:" + ioe.Source + ".dll" + " HResult: " + ioe.HResult + Environment.NewLine + "Reason: " + ioe.Message);
                return false;
            }
            return true;
        }

        public bool runFile(Process targetProcess, string arguments)
        {
            targetProcess = new Process();
            targetProcess.StartInfo.Arguments = arguments;

            try
            {
                targetProcess.Start();
                targetName = targetProcess.ProcessName;
                targetID = targetProcess.Id;
                strTargetID = Convert.ToString(targetProcess.Id);
                targetThreadCount = Convert.ToInt32(targetProcess.Threads);
            }
            catch (Win32Exception win32)
            {
                warningMSG = "[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + win32.StackTrace + Environment.NewLine + "Source:" + win32.Source + ".dll" + " HResult: " + win32.HResult + Environment.NewLine + "Reason: " + win32.Message + Environment.NewLine;
                Debug.WriteLine("[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + win32.StackTrace + Environment.NewLine + "Source:" + win32.Source + ".dll" + " HResult: " + win32.HResult + Environment.NewLine + "Reason: " + win32.Message);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                warningMSG = "[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ioe.StackTrace + Environment.NewLine + "Source:" + ioe.Source + ".dll" + " HResult: " + ioe.HResult + Environment.NewLine + "Reason: " + ioe.Message + Environment.NewLine;
                Debug.WriteLine("[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ioe.StackTrace + Environment.NewLine + "Source:" + ioe.Source + ".dll" + " HResult: " + ioe.HResult + Environment.NewLine + "Reason: " + ioe.Message);
                return false;
            }
            return true;
        }

        public bool runFile(string target)
        {
            Process targetProcess = new Process();
            targetProcess.StartInfo.FileName = target;

            try
            {
                targetProcess.Start();
                targetName = targetProcess.ProcessName;
                targetID = targetProcess.Id;
                strTargetID = Convert.ToString(targetProcess.Id);
                targetThreadCount = Convert.ToInt32(targetProcess.Threads);
            }
            catch (Win32Exception win32)
            {
                warningMSG = "[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + win32.StackTrace + Environment.NewLine + "Source: " + win32.Source + ".dll" + " HResult: " + win32.HResult + Environment.NewLine + "Reason: " + win32.Message + Environment.NewLine;
                Debug.WriteLine("[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + win32.StackTrace + Environment.NewLine + "Source: " + win32.Source + ".dll" + " HResult: " + win32.HResult + Environment.NewLine + "Reason: " + win32.Message);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                warningMSG = "[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ioe.StackTrace + Environment.NewLine + "Source: " + ioe.Source + ".dll" + " HResult: " + ioe.HResult + Environment.NewLine + "Reason: " + ioe.Message + Environment.NewLine;
                Debug.WriteLine("[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ioe.StackTrace + Environment.NewLine + "Source: " + ioe.Source + ".dll" + " HResult: " + ioe.HResult + Environment.NewLine + "Reason: " + ioe.Message);
                return false;
            }
            return true;
        }

        public bool runFile(string target, string arguments = null) // try combine to one func if possible (args = null);
        {
            Process targetProcess = new Process();
            targetProcess.StartInfo.FileName = target;
            targetProcess.StartInfo.Arguments = arguments;

            try
            {
                targetProcess.Start();
                targetName = targetProcess.ProcessName;
                targetID = targetProcess.Id;
                strTargetID = Convert.ToString(targetProcess.Id);
                targetThreadCount = Convert.ToInt32(targetProcess.Threads);
            }
            catch (Win32Exception win32)
            {
                warningMSG = "[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + win32.StackTrace + Environment.NewLine + "Source: " + win32.Source + ".dll" + " HResult: " + win32.HResult + Environment.NewLine + "Reason: " + win32.Message + Environment.NewLine;
                Debug.WriteLine("[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + win32.StackTrace + Environment.NewLine + "Source: " + win32.Source + ".dll" + " HResult: " + win32.HResult + Environment.NewLine + "Reason: " + win32.Message);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                warningMSG = "[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ioe.StackTrace + Environment.NewLine + "Source: " + ioe.Source + ".dll" + " HResult: " + ioe.HResult + Environment.NewLine + "Reason: " + ioe.Message + Environment.NewLine;
                Debug.WriteLine("[WARNING]: " + DateTime.Now + Environment.NewLine + "Stack Trace:" + Environment.NewLine + ioe.StackTrace + Environment.NewLine + "Source: " + ioe.Source + ".dll" + " HResult: " + ioe.HResult + Environment.NewLine + "Reason: " + ioe.Message);
                return false;
            }
            return true;
        }
    }
}
