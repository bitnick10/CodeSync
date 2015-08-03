using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string address;
        string tempFolderPath; // C:\github.com/user/projectname
        string tempPath; // C:\github.com/user/projectname/master.zip
        public MainWindow()
        {
            InitializeComponent();

            address = labelUrl.Content.ToString();
            int a1 = address.IndexOf("//");
            string a2 = address.Substring(a1 + 2, address.Length - a1 - 2);
            string[] arr = a2.Split('/');
            string website = arr[0];
            string username = arr[1];
            string projectname = arr[2];
            string filename = arr[4];
            tempFolderPath = string.Format(@"C:\CodeSyncTemp\{0}\{1}\{2}", website, username, projectname);
            tempPath = string.Format(@"C:\CodeSyncTemp\{0}\{1}\{2}\{3}", website, username, projectname, filename);
        }
        void DonwloadCode()
        {
            WebClient client = new WebClient();
            try
            {
                //string fileName = labelPath.Content.ToString() + @"\a.zip";
                client.DownloadFile(labelUrl.Content.ToString(), tempPath);
            }
            catch (Exception e)
            {
                string str = e.ToString();
            }
        }
        void Unzip(string zipPath)
        {
            string extractPath = tempFolderPath;

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(@"/", StringComparison.OrdinalIgnoreCase) || entry.FullName.EndsWith(@"\", StringComparison.OrdinalIgnoreCase))
                    {
                        string destinationFolderName = System.IO.Path.Combine(extractPath, entry.FullName);
                        IfDirectoryDoesNotExsitsCreateOne(destinationFolderName);
                    }
                    else
                    {
                        string destinationFileName = System.IO.Path.Combine(extractPath, entry.FullName);
                        entry.ExtractToFile(destinationFileName,true);
                    }
                }
            }
        }
        void IfDirectoryDoesNotExsitsCreateOne(string folePath)
        {
            if (Directory.Exists(folePath) == false)
            {
                Directory.CreateDirectory(folePath);
            }
        }
        void CopyToPlace()
        {
            string[] a = Directory.GetDirectories(tempFolderPath);
            DirectoryCopy(a[0], labelPath.Content.ToString(), true);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IfDirectoryDoesNotExsitsCreateOne(tempFolderPath);
            DonwloadCode();
            Unzip(tempPath);
            CopyToPlace();
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
