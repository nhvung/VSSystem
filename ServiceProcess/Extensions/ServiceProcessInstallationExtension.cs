using System;
using System.IO;
using System.Text;

namespace VSSystem.ServiceProcess.Extensions
{
    public static class ServiceProcessInstallationExtension
    {
        #region Ubuntu
        public static void CreateUbuntuInstallationFile(this object sender, DirectoryInfo workingFolder, string name)
        {
            try
            {
                FileInfo file = new FileInfo(workingFolder.FullName + "/install-service.sh");
                if (!file.Exists)
                {
                    string cmd =
                    "#!/usr/bin/env bash" + Environment.NewLine + Environment.NewLine +
                    "if [ $(id -u) -ne 0 ]; then echo \"please run as root\"; exit 1; fi" + Environment.NewLine + Environment.NewLine +
                    "DIR_NAME=${PWD##*/}" + Environment.NewLine +
                    "app_file=\"" + name + "\"" + Environment.NewLine +
                    "echo \" > DIR_NAME = '$DIR_NAME' : app_file = '$app_file'\"" + Environment.NewLine + Environment.NewLine +

                    "SERVICE_NAME=" + name + ".service" + Environment.NewLine +
                    "SERVICE_FILE=/etc/systemd/system/$SERVICE_NAME" + Environment.NewLine +
                    "CURRENT_PATH=$PWD" + Environment.NewLine +
                    "echo \"START INSTALL\"" + Environment.NewLine + Environment.NewLine +

                    "_CONTENT=\"#!/usr/bin/env bash" + Environment.NewLine + Environment.NewLine +

                    "cd $CURRENT_PATH" + Environment.NewLine +
                    "export LD_LIBRARY_PATH=./" + Environment.NewLine +
                    "./$app_file --tlog=1 --serviceName=\"\"" + name + "\"\" --binDir=\"\"" + workingFolder.FullName + "\"\"\"" + Environment.NewLine +

                    "SH_FILE=run_service.sh" + Environment.NewLine +
                    "if [ -f $SH_FILE ]; then rm $SH_FILE; fi" + Environment.NewLine +
                    "echo \"$_CONTENT\" >> $SH_FILE" + Environment.NewLine +
                    "chmod +x $SH_FILE" + Environment.NewLine +
                    "chmod 777 $SH_FILE" + Environment.NewLine + Environment.NewLine +

                    "_CONTENT=\"[Unit]" + Environment.NewLine +
                    "Description=$DIR_NAME Service" + Environment.NewLine +
                    "After=sysinit.target" + Environment.NewLine +

                    "[Service]" + Environment.NewLine +
                    "Type=simple" + Environment.NewLine +
                    "ExecStart=$CURRENT_PATH/$SH_FILE" + Environment.NewLine +
                    "Restart=always" + Environment.NewLine +
                    "KillMode=process" + Environment.NewLine +
                    "RestartSec=10" + Environment.NewLine +
                    "User=root" + Environment.NewLine +
                    "Group=root" + Environment.NewLine +

                    "[Install]" + Environment.NewLine +
                    "WantedBy=multi-user.target\"" + Environment.NewLine + Environment.NewLine +

                    "if [ -f $SERVICE_FILE ]; then rm $SERVICE_FILE; fi" + Environment.NewLine +
                    "echo \"save service script $SERVICE_FILE\"" + Environment.NewLine +
                    "echo \"$_CONTENT\" >> $SERVICE_FILE" + Environment.NewLine + Environment.NewLine +

                    "systemctl enable $SERVICE_NAME" + Environment.NewLine + Environment.NewLine

                    //     "SH_FILE=start-service.sh" + Environment.NewLine +
                    //     "echo \"sudo systemctl start $SERVICE_NAME\" >> $SH_FILE" + Environment.NewLine +
                    //     "chmod +x $SH_FILE" + Environment.NewLine +
                    //     "chmod 777 $SH_FILE" + Environment.NewLine + Environment.NewLine +

                    //     "SH_FILE=status-service.sh" + Environment.NewLine +
                    //    "echo \"sudo systemctl status $SERVICE_NAME\" >> $SH_FILE" + Environment.NewLine +
                    //     "chmod +x $SH_FILE" + Environment.NewLine +
                    //     "chmod 777 $SH_FILE" + Environment.NewLine + Environment.NewLine

                    // "SH_FILE=stop-service.sh" + Environment.NewLine +
                    // "echo \"sudo systemctl stop $SERVICE_NAME\" >> $SH_FILE" + Environment.NewLine +
                    //  //$"echo \"sudo kill -9 `cat {name}.pid`\" >> $SH_FILE" + Environment.NewLine +
                    //  "chmod +x $SH_FILE" + Environment.NewLine +
                    // "chmod 777 $SH_FILE" + Environment.NewLine
                    ;

                    File.WriteAllText(file.FullName, cmd, Encoding.UTF8);

                }

                CreateUbuntuStartServiceFile(sender, workingFolder, name);

            }
            catch { }
        }
        public static void CreateUbuntuUninstallationFile(this object sender, DirectoryInfo workingFolder, string name)
        {
            try
            {
                FileInfo file = new FileInfo(workingFolder.FullName + "/uninstall-service.sh");
                if (!file.Exists)
                {
                    string cmd =
                     "#!/usr/bin/env bash" + Environment.NewLine + Environment.NewLine +
                    "if [ $(id -u) -ne 0 ]; then echo \"please run as root\"; exit 1; fi" + Environment.NewLine + Environment.NewLine +
                    "DIR_NAME=${PWD##*/}" + Environment.NewLine +
                    "SERVICE_NAME=" + name + ".service" + Environment.NewLine + Environment.NewLine +

                    "echo \"START REMOVE $SERVICE_NAME\"" + Environment.NewLine +
                    "systemctl stop $SERVICE_NAME" + Environment.NewLine +
                    "systemctl disable $SERVICE_NAME" + Environment.NewLine + Environment.NewLine +

                    "path=/etc/systemd/system/$SERVICE_NAME" + Environment.NewLine +
                    "if [ -f $path ]; then rm $path; fi" + Environment.NewLine + Environment.NewLine +

                    "path=/etc/systemd/system/multi-user.target.wants/$SERVICE_NAME" + Environment.NewLine +
                    "if [ -f $path ]; then rm $path; fi" + Environment.NewLine + Environment.NewLine +

                    "systemctl daemon-reload" + Environment.NewLine +
                    "systemctl reset-failed" + Environment.NewLine + Environment.NewLine +

                    // "SH_FILE=start-service.sh" + Environment.NewLine +
                    // "if [ -f $SH_FILE ]; then rm $SH_FILE; fi" + Environment.NewLine + Environment.NewLine +

                    // "SH_FILE=status-service.sh" + Environment.NewLine +
                    // "if [ -f $SH_FILE ]; then rm $SH_FILE; fi" + Environment.NewLine + Environment.NewLine +

                    // "SH_FILE=stop-service.sh" + Environment.NewLine +
                    // "if [ -f $SH_FILE ]; then rm $SH_FILE; fi" + Environment.NewLine + Environment.NewLine +

                    // "SH_FILE=run_service.sh" + Environment.NewLine +
                    // "if [ -f $SH_FILE ]; then rm $SH_FILE; fi" + Environment.NewLine + Environment.NewLine +

                    "echo \"FINISH UNINSTALL\"" + Environment.NewLine
                    ;

                    File.WriteAllText(file.FullName, cmd);
                }

            }
            catch { }
        }

        public static void CreateUbuntuFindPortCmdFile(this object sender, DirectoryInfo workingFolder, string name, params int[] ports)
        {
            try
            {
                if (ports?.Length > 0)
                {
                    FileInfo file = new FileInfo(workingFolder.FullName + "/findports-service.sh");
                    File.WriteAllLines(file.FullName, new string[]{
                   "#!/usr/bin/env bash",
                    "sudo lsof -i :" + string.Join(",", ports)
                }, Encoding.UTF8);
                }
            }
            catch { }
        }

        public static void CreateUbuntuStartServiceFile(this object sender, DirectoryInfo workingFolder, string name)
        {
            try
            {
                FileInfo startFile = new FileInfo(workingFolder.FullName + "/start-service.sh");
                string cmd = "sudo systemctl start " + name;
                File.WriteAllText(startFile.FullName, cmd);
            }
            catch { }
        }
        public static void CreateUbuntuStopServiceFile(this object sender, DirectoryInfo workingFolder, string name, int processID = 0)
        {
            try
            {
                FileInfo startFile = new FileInfo(workingFolder.FullName + "/stop-service.sh");
                string cmd = "sudo systemctl stop " + name;
                if (processID > 0)
                {
                    cmd += Environment.NewLine + Environment.NewLine + "sudo kill -9 " + processID;
                }
                File.WriteAllText(startFile.FullName, cmd);

            }
            catch { }
        }

        public static void CreateUbuntuRestartServiceFile(this object sender, DirectoryInfo workingFolder, string name, int processID = 0)
        {
            try
            {
                FileInfo startFile = new FileInfo(workingFolder.FullName + "/restart-service.sh");
                string cmd = "sudo systemctl stop " + name;
                if (processID > 0)
                {
                    cmd += Environment.NewLine + Environment.NewLine + "sudo kill -9 " + processID;
                }
                cmd += Environment.NewLine + Environment.NewLine + "sudo systemctl start " + name;
                File.WriteAllText(startFile.FullName, cmd);

            }
            catch { }
        }

        #endregion

        #region Windows


        public static void CreateWindowsInstallationFile(this object sender, DirectoryInfo workingFolder, string name)
        {
            try
            {

                var username = string.Empty;

                var password = string.Empty;
                if (_ConfirmUseCustomAccount(sender))
                {
                    username = _ReadUsername();
                    password = _ReadPassword();
                    Console.Clear();
                }

                FileInfo file = new FileInfo(workingFolder.FullName + "/install-service.bat");
                string cmd = "sc create \"" + name + "\" binPath= \"\\\"" + workingFolder.FullName + "\\" + name + ".exe\\\" --serviceName=\\\"" + name + "\\\" --tlog=\"1\" --binDir=\\\"" + workingFolder.FullName + "\\\" \"";

                cmd += " start= auto type= own";

                if (!string.IsNullOrWhiteSpace(username))
                {
                    cmd += " obj= \".\\" + username + "\"";
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        cmd += " password= \"" + password + "\"";
                    }
                }

                File.WriteAllText(file.FullName, cmd);

                CreateWindowsStartServiceFile(sender, workingFolder, name);

            }
            catch { }
        }
        public static void CreateWindowsUninstallationFile(this object sender, DirectoryInfo workingFolder, string name)
        {
            try
            {
                FileInfo file = new FileInfo(workingFolder.FullName + "/uninstall-service.bat");
                string cmd = "net stop " + name + Environment.NewLine + "sc delete " + name;
                File.WriteAllText(file.FullName, cmd);

            }
            catch { }
        }


        public static void CreateWindowsStartServiceFile(this object sender, DirectoryInfo workingFolder, string name)
        {
            try
            {
                FileInfo startFile = new FileInfo(workingFolder.FullName + "/start-service.bat");
                string cmd = "net start " + name;
                File.WriteAllText(startFile.FullName, cmd);

            }
            catch { }
        }
        public static void CreateWindowsStopServiceFile(this object sender, DirectoryInfo workingFolder, string name, int processID = 0)
        {
            try
            {
                FileInfo startFile = new FileInfo(workingFolder.FullName + "/stop-service.bat");
                string cmd = "net stop " + name;
                if (processID > 0)
                {
                    cmd += Environment.NewLine + Environment.NewLine + "taskkill /pid " + processID;
                }
                File.WriteAllText(startFile.FullName, cmd);

            }
            catch { }
        }

        public static void CreateWindowsRestartServiceFile(this object sender, DirectoryInfo workingFolder, string name, int processID = 0)
        {
            try
            {
                FileInfo startFile = new FileInfo(workingFolder.FullName + "/restart-service.bat");
                string cmd = "net stop " + name;
                if (processID > 0)
                {
                    cmd += Environment.NewLine + Environment.NewLine + "taskkill /pid " + processID;
                }
                cmd += Environment.NewLine + Environment.NewLine + "net start " + name;
                File.WriteAllText(startFile.FullName, cmd);

            }
            catch { }
        }

        #endregion
        static string _ReadPassword()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Enter service password [Enter without type for empty password]: ");
            ConsoleKeyInfo keyInfo;
            string result = string.Empty;
            do
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key != ConsoleKey.Backspace)
                {
                    if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        result += keyInfo.KeyChar;
                        Console.Write("*");
                    }
                }
                else
                {
                    if (result.Length > 0)
                    {
                        result = result.Substring(0, result.Length - 1);
                        Console.Write("\b \b");
                    }
                }

            } while (keyInfo.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return result;
        }
        static string _ReadUsername()
        {
            string result = string.Empty;
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Enter service username [Enter without type for default account]: ");
                ConsoleKeyInfo keyInfo;

                do
                {
                    keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key != ConsoleKey.Backspace)
                    {
                        if (keyInfo.Key != ConsoleKey.Enter)
                        {
                            result += keyInfo.KeyChar;
                            Console.Write(keyInfo.KeyChar);
                        }
                    }
                    else
                    {
                        if (result.Length > 0)
                        {
                            result = result.Substring(0, result.Length - 1);
                            Console.Write("\b \b");
                        }

                    }

                } while (keyInfo.Key != ConsoleKey.Enter);
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = Environment.UserName;
                }
            }
            catch { }
            Console.WriteLine();
            return result;
        }
        public static bool ConfirmCreateInstallationFiles(this object sender)
        {
            bool result = false;
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Do you want to create install and uninstall service files [yes / no or press Enter]?: ");
                string answer = Console.ReadLine();
                result = answer.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
            }
            catch { }
            return result;
        }
        static bool _ConfirmUseCustomAccount(this object sender)
        {
            bool result = false;
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Do you want to use custom authentication for this service [yes / no or press Enter]?: ");
                string answer = Console.ReadLine();
                result = answer.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
            }
            catch { }
            return result;
        }
        public static string ReadServiveName(this object sender, string defaultName)
        {
            string result = string.Empty;
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Enter service name [Enter without type for default name]: ");
                ConsoleKeyInfo keyInfo;

                do
                {
                    keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key != ConsoleKey.Backspace)
                    {
                        if (keyInfo.Key != ConsoleKey.Enter)
                        {
                            result += keyInfo.KeyChar;
                            Console.Write(keyInfo.KeyChar);
                        }
                    }
                    else
                    {
                        if (result.Length > 0)
                        {
                            result = result.Substring(0, result.Length - 1);
                            Console.Write("\b \b");
                        }

                    }

                } while (keyInfo.Key != ConsoleKey.Enter);
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = defaultName;
                }
            }
            catch { }
            Console.WriteLine();
            return result;
        }
    }
}
