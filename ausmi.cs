using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmongUsSimpleModInstaller
{
    public partial class ausmi : Form
    {
        public ausmi()
        {
            InitializeComponent();
        }

        string defaultTargetLocation = $@"{Application.ExecutablePath.ToString().Substring(0,Application.ExecutablePath.ToString().LastIndexOf(@"\"))}\Mods";
        readonly string amongUsAppId = "945360";
        static string updateURI = "";
        string libFoldersFilePath = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            //this needs automating... it should never be longer than 6 months.
            DateTime expiry = DateTime.Parse("01/06/2023");
            this.Text = $"AUSMI BETA - This expires on {expiry.ToString("yyyy-MM-dd")}.";
            if (expiry <= DateTime.Now)
            {
                MessageBox.Show("Program has expired and will no longer open, this needs the expiry date editing on github for it to work again.");
                Application.Exit();
            }
            string strSteamInstallPath = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", "").ToString();

            Console.WriteLine("The install path is " + strSteamInstallPath);
            lblSteamInstallPath.Text = $"SteamInstallLocation={strSteamInstallPath}";
            libFoldersFilePath = strSteamInstallPath + @"\steamapps\libraryfolders.vdf";
            lblTargetLocation.Text = $"{defaultTargetLocation}";
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (cmbModSelection.SelectedIndex != -1)
            {
                Console.WriteLine(cmbModSelection.SelectedItem.ToString());
                installMod(libFoldersFilePath, cmbModSelection.SelectedItem.ToString(), defaultTargetLocation);
            }
        }

        private void installMod(string libFoldersFilePath, string modName, string targetPath)
        {
            lblStatus.Text = "";
            cmbModSelection.Enabled = false;
            btnInstall.Enabled = false;
            string modURI = getModDownloadURI(modName);
            string riURI = getModDownloadURI("Mini.RegionInstall");
            if (modURI != "")
            {
                readLibraryFoldersFile(libFoldersFilePath, modName);
                findModDownloadZip(modURI, ".zip", targetPath, modName);
                //this is not working yet
                //downloadMiniRegionInstall(riURI, ".dll", targetPath, modName);
            }
            else
            {
                //no mod asked to be downloaded...?
            }
            cmbModSelection.Enabled = true;
            btnInstall.Enabled = true;
            lblStatus.Text = "Installed";
        }

        private string getModDownloadURI(string modName)
        {
            if (modName=="The Other Roles")
            {
                return "https://api.github.com/repos/Eisbison/TheOtherRoles/releases/latest";
            }
            else if(modName=="Town Of Us-Reactivated")
            {
                return "https://api.github.com/repos/eDonnes124/Town-Of-Us-R/releases/latest";
            }
            else if(modName=="Las Monjas")
            {
                return "https://api.github.com/repos/KiraYamato94/LasMonjas/releases/latest";
            }
            //not working yet
            else if (modName == "Mini.RegionInstall")
            {
                return "https://api.github.com/repos/miniduikboot/Mini.RegionInstall/releases/latest";
            }
            //not up to date at time of release
            else if(modName=="Town Of Us(Anusien)")
            {
                return "https://api.github.com/repos/Anusien/Town-Of-Us/releases/latest";
            }
            //do nothing
            return "";
        }

        #region downloading mod files
        private static async Task<bool> findModDownloadZip(string downloadURI, string zipName, string targetPath, string modName)
        {
            try
            {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "SimpleModInstaller");
                var response = await http.GetAsync(new System.Uri(downloadURI), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    System.Console.WriteLine("Server returned no data: " + response.StatusCode.ToString());
                    //return false;
                }
                string json = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(json);

                string tagname = data["tag_name"]?.ToString();
                if (tagname == null)
                {
                    return false; // Something went wrong
                }

                string changeLog = data["body"]?.ToString();
                //if (changeLog != null) announcement = changeLog;

                JToken assets = data["assets"];
                if (!assets.HasValues)
                    return false;

                for (JToken current = assets.First; current != null; current = current.Next)
                {
                    string browser_download_url = current["browser_download_url"]?.ToString();
                    if (browser_download_url != null && current["content_type"] != null)
                    {
                        if ((current["content_type"].ToString().Equals("application/zip") || current["content_type"].ToString().Equals("application/x-zip-compressed")) &&
                            browser_download_url.EndsWith(zipName))
                        {
                            updateURI = browser_download_url;
                            await downloadUpdate(targetPath, modName);
                            return true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                //TheOtherRolesPlugin.Instance.Log.LogError(ex.ToString());
                System.Console.WriteLine(ex);
            }
            return false;
        }

        public static async Task<bool> downloadUpdate(string targetPath,string modName)
        {
            try
            {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "SimpleModInstaller");
                var response = await http.GetAsync(new System.Uri(updateURI), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    System.Console.WriteLine("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                System.UriBuilder uri = new System.UriBuilder(codeBase);
                string fullname = System.Uri.UnescapeDataString(uri.Path);
                //string fullname = targetPath; --- this is the path to where the mod needs to be installed, this is not required right now.
                string tempModDownloadLocation = fullname.Substring(0, fullname.LastIndexOf(@"/")) + @"/temp/";
                Console.WriteLine(tempModDownloadLocation);
                //if (File.Exists(fullname + ".old")) // Clear old file in case it wasnt;
                if (Directory.Exists(tempModDownloadLocation))
                    Directory.Delete(tempModDownloadLocation, true);
                Console.WriteLine("temp folder deleted successfully");
                Directory.CreateDirectory(tempModDownloadLocation);

                string modTempDownloadPath = tempModDownloadLocation + "mod.zip";
                //File.Move(fullname, fullname + ".old"); // rename current executable to old

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = File.Create(modTempDownloadPath))
                    { // probably want to have proper name here
                        responseStream.CopyTo(fileStream);
                    }
                }
                //showPopup("The Other Roles\nupdated successfully\nPlease restart the game.");
                Console.WriteLine("ModDownloaded Successfully");
                await combineModWithAmongUsVanillaClient(modTempDownloadPath, targetPath,modName);
                if (Directory.Exists(tempModDownloadLocation))
                    Directory.Delete(tempModDownloadLocation, true);
                Console.WriteLine("temp folder deleted successfully (again)");

                return true;
            }
            catch (System.Exception ex)
            {
                //TheOtherRolesPlugin.Instance.Log.LogError(ex.ToString());
                System.Console.WriteLine(ex);
            }
            //showPopup("Update wasn't successful\nTry again later,\nor update manually.");
            return false;
        }
        #endregion

        #region mini.regioninstall adding -- this currently doesnt work.
        private static async Task<bool> downloadMiniRegionInstall(string downloadURI,string dllName,string targetPath,string modName)
        {
            try
            {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "SimpleModInstaller");
                var response = await http.GetAsync(new System.Uri(downloadURI), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    System.Console.WriteLine("Server returned no data: " + response.StatusCode.ToString());
                    //return false;
                }
                string json = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(json);

                string tagname = data["tag_name"]?.ToString();
                if (tagname == null)
                {
                    return false; // Something went wrong
                }

                string changeLog = data["body"]?.ToString();
                //if (changeLog != null) announcement = changeLog;

                JToken assets = data["assets"];
                if (!assets.HasValues)
                    return false;

                for (JToken current = assets.First; current != null; current = current.Next)
                {
                    string browser_download_url = current["browser_download_url"]?.ToString();
                    if (browser_download_url != null && current["content_type"] != null)
                    {
                        if ((current["content_type"].ToString().Equals("application/octet-stream")) &&
                            browser_download_url.EndsWith(dllName))
                        {
                            updateURI = browser_download_url;
                            await installMiniRegionInstall(targetPath, modName);
                            return true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                //TheOtherRolesPlugin.Instance.Log.LogError(ex.ToString());
                System.Console.WriteLine(ex);
            }
            return false;
        }

        public static async Task<bool> installMiniRegionInstall(string targetPath, string modName)
        {
            try
            {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "SimpleModInstaller");
                var response = await http.GetAsync(new System.Uri(updateURI), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    System.Console.WriteLine("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                System.UriBuilder uri = new System.UriBuilder(codeBase);
                string fullname = System.Uri.UnescapeDataString(uri.Path);
                //string fullname = targetPath; --- this is the path to where the mod needs to be installed, this is not required right now.
                string tempModDownloadLocation = fullname.Substring(0, fullname.LastIndexOf(@"/")) + @"/temp/";
                Console.WriteLine(tempModDownloadLocation);
                //if (File.Exists(fullname + ".old")) // Clear old file in case it wasnt;
                if (Directory.Exists(tempModDownloadLocation))
                    Directory.Delete(tempModDownloadLocation, true);
                Console.WriteLine("temp folder deleted successfully");
                Directory.CreateDirectory(tempModDownloadLocation);

                string modTempDownloadPath = tempModDownloadLocation + "ri.zip";
                //File.Move(fullname, fullname + ".old"); // rename current executable to old

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = File.Create(modTempDownloadPath))
                    { // probably want to have proper name here
                        responseStream.CopyTo(fileStream);
                    }
                }
                //showPopup("The Other Roles\nupdated successfully\nPlease restart the game.");
                Console.WriteLine("ModDownloaded Successfully");
                await combineModWithAmongUsVanillaClient(modTempDownloadPath, targetPath, modName);
                //await addRegionInstallToModWithAmongUsVanillaClient(modTempDownloadPath, targetPath, modName);
                if (Directory.Exists(tempModDownloadLocation))
                    Directory.Delete(tempModDownloadLocation, true);
                Console.WriteLine("temp folder deleted successfully (again)");

                return true;
            }
            catch (System.Exception ex)
            {
                //TheOtherRolesPlugin.Instance.Log.LogError(ex.ToString());
                System.Console.WriteLine(ex);
            }
            //showPopup("Update wasn't successful\nTry again later,\nor update manually.");
            return false;
        }
        #endregion

        #region configure regioninstall by reading and editing the file

        #endregion

        #region combining client and mod
        public static async Task<bool> combineModWithAmongUsVanillaClient(string tempDownloadLocation, string targetPath,string modName)
        {
            try
            {
                var unzippedFiles = unzipZipAndPutContentsInArray(tempDownloadLocation);
                string rootFilePath = unzippedFiles.Find(x => x.Contains("steam_appid.txt"));
                string rootFilePathParent = rootFilePath.Substring(0, rootFilePath.LastIndexOf(@"\"));
                var cleanedFiles = removeFilesNotNeeded(rootFilePathParent, unzippedFiles);
                installModFilesToClient(rootFilePathParent, cleanedFiles, targetPath,modName);

                return true;
            }
            catch (System.Exception ex)
            {
                //TheOtherRolesPlugin.Instance.Log.LogError(ex.ToString());
                System.Console.WriteLine(ex);
            }
            //showPopup("Update wasn't successful\nTry again later,\nor update manually.");
            return false;
        }

        public static List<string> unzipZipAndPutContentsInArray(string zippath)
        {
            List<string> files = new List<string>();
            Console.WriteLine(zippath);
            string tempZipFolder = zippath.Replace(".zip", "");
            ZipFile.ExtractToDirectory(zippath, tempZipFolder);
            foreach(var file in Directory.EnumerateFiles(tempZipFolder,"*",SearchOption.AllDirectories))
            {
                Console.WriteLine($"File:{file}");
                files.Add(file);
            }
            Console.WriteLine("Zip Extracted");
            return files;
        }

        public static List<string> removeFilesNotNeeded(string root, List<string> files)
        {
            foreach (var file in files)
            {
                if(!file.Contains(root))
                {
                    files.Remove(file);
                    Console.WriteLine($"File Removed:- {file}");
                }
            }
            Console.WriteLine("Cleaned Mod Files List");
            return files;
        }

        public static void installModFilesToClient(string root, List<string> files, string targetPath,string modName)
        {
            foreach (var file in files)
            {
                Console.WriteLine(file.Substring(root.Length));
                Console.WriteLine("hold");
                Directory.CreateDirectory(targetPath + @"\" + modName + file.Substring(0, file.LastIndexOf(@"\")).Substring(root.Length));
                File.Copy(file, targetPath + @"\" + modName + file.Substring(root.Length));
            }
        }
        #endregion



        #region duplicating vanilla among us folder from steam library
        //copying steam client from here
        private void readLibraryFoldersFile(string path,string modFolder)
        {
            StreamReader reader = File.OpenText(path);
            string line;
            string libraryLine = "";
            while ((line = reader.ReadLine()) != null)
            {
                //string[] items = line.Split('\t');
                if (line.Contains("path"))
                {
                    libraryLine = line.Substring(line.IndexOf("\"path\"		\"") + 9);
                    libraryLine = libraryLine.Substring(0, libraryLine.Length - 1);
                    libraryLine = libraryLine.Replace(@"\\", @"\");
                }

                if (line.Contains(amongUsAppId))
                {
                    copyLibaryFiles(libraryLine,modFolder);
                    break;
                }
                //Console.WriteLine(line);
            }
        }

        private void copyLibaryFiles(string libraryPath,string modFolder)
        {
            Console.WriteLine(libraryPath);
            string amongUsLibAppPath = "";
            string acfFilePath = "";
            acfFilePath = libraryPath + $@"\steamapps\appmanifest_{amongUsAppId}.acf";
            Console.WriteLine(acfFilePath);

            Console.WriteLine($"AU ACF File={acfFilePath}");
            string folder = readACFFile(acfFilePath);

            amongUsLibAppPath = libraryPath + @"\steamapps\common\" + folder;
            Console.WriteLine(amongUsLibAppPath);
            Console.WriteLine($"AU Installed Folder Location={amongUsLibAppPath}");
            string modTargetPath = defaultTargetLocation + $@"\{modFolder}\";
            //delete old mod files in folder
            if (Directory.Exists(modTargetPath))
            {
                Directory.Delete(modTargetPath,true);
            }
            //build among us files
            foreach (var file in Directory.EnumerateFiles(amongUsLibAppPath,"*",SearchOption.AllDirectories))
            {
                Console.WriteLine(file);
                Console.WriteLine(file.Substring(file.IndexOf(@"\",amongUsLibAppPath.Length-1)+1));
                string fileSubPath = file.Substring(file.IndexOf(@"\", amongUsLibAppPath.Length - 1) + 1);
                string targetFilePath = modTargetPath + @"\" + fileSubPath;
                Directory.CreateDirectory(targetFilePath.Substring(0,targetFilePath.LastIndexOf(@"\")+1));
                File.Copy(file,targetFilePath);
            }
        }

        private string readACFFile(string path)
        {
            StreamReader reader = File.OpenText(path);
            string line;
            string folder = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("name"))
                {
                    folder = line.Substring(line.IndexOf("\"name\"      \"") + 11);
                    folder = folder.Substring(0, folder.Length - 1);
                    break;
                    //folder = folder.Replace(@"\\", @"\");
                }
            }
            return folder;
        }
        #endregion

        private void btnModsFolder_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(lblTargetLocation.Text);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = lblTargetLocation.Text,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }
    }
}
