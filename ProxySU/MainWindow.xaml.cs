﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Renci.SshNet;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ProxySU
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RadioButtonPasswordLogin.IsChecked = true;
            RadioButtonNoProxy.IsChecked = true;
            RadioButtonProxyNoLogin.IsChecked = true;
        }
        //System.Diagnostics.Process exitProgram = System.Diagnostics.Process.GetProcessById(System.Diagnostics.Process.GetCurrentProcess().Id);
        private void Button_Login_Click(object sender, RoutedEventArgs e)

        {
            //ProgressBarSetUpProcessing.IsIndeterminate = true;
            #region 检测输入的内空是否有错，并读取内容
            if (string.IsNullOrEmpty(TextBoxHost.Text) == true || string.IsNullOrEmpty(TextBoxPort.Text) == true || string.IsNullOrEmpty(TextBoxUserName.Text) == true)
            {
                MessageBox.Show("主机地址、主机端口、用户名为必填项，不能为空");
                //exitProgram.Kill();
                return;
            }
            string sshHostName = TextBoxHost.Text.ToString();
            int sshPort = int.Parse(TextBoxPort.Text);
            string sshUser = TextBoxUserName.Text.ToString();
            if (RadioButtonPasswordLogin.IsChecked == true && string.IsNullOrEmpty(PasswordBoxHostPassword.Password) == true)
            {
                MessageBox.Show("登录密码为必填项，不能为空");
                //exitProgram.Kill();
                return;
            }
            string sshPassword = PasswordBoxHostPassword.Password.ToString();
            if (RadioButtonCertLogin.IsChecked == true && string.IsNullOrEmpty(TextBoxCertFilePath.Text) == true)
            {
                MessageBox.Show("密钥文件为必填项，不能为空");
                //exitProgram.Kill();
                return;
            }
            string sshPrivateKey = TextBoxCertFilePath.Text.ToString();
            ProxyTypes proxyTypes = new ProxyTypes();//默认为None
            //MessageBox.Show(proxyTypes.ToString());
            //proxyTypes = ProxyTypes.Socks5;
            if (RadioButtonHttp.IsChecked==true)
            {
                proxyTypes = ProxyTypes.Http;
            }
            else if (RadioButtonSocks4.IsChecked==true)
            {
                proxyTypes = ProxyTypes.Socks4;
            }
            else if (RadioButtonSocks5.IsChecked==true)
            {
                proxyTypes = ProxyTypes.Socks5;
            }
            else
            {
                proxyTypes = ProxyTypes.None;
            }

            //MessageBox.Show(proxyTypes.ToString());
            if (RadioButtonNoProxy.IsChecked==false&&(string.IsNullOrEmpty(TextBoxProxyHost.Text)==true||string.IsNullOrEmpty(TextBoxProxyPort.Text)==true))
            {
                MessageBox.Show("如果选择了代理，则代理地址与端口不能为空");
                //exitProgram.Kill();
                return;
            }
            string sshProxyHost = TextBoxProxyHost.Text.ToString();
            int sshProxyPort = int.Parse(TextBoxProxyPort.Text.ToString());
            if (RadiobuttonProxyYesLogin.IsChecked == true && (string.IsNullOrEmpty(TextBoxProxyUserName.Text) == true || string.IsNullOrEmpty(PasswordBoxProxyPassword.Password) == true))
            {
                MessageBox.Show("如果代理需要登录，则代理登录的用户名与密码不能为空");
                //exitProgram.Kill();
                return;
            }
            string sshProxyUser = TextBoxProxyUserName.Text.ToString();
            string sshProxyPassword = PasswordBoxProxyPassword.Password.ToString();

            //TextBlockSetUpProcessing.Text = "登录中";
            //ProgressBarSetUpProcessing.IsIndeterminate = true;
            #endregion

           // try
            //{

                //var connectionInfo = new PasswordConnectionInfo(sshHostName, sshPort, sshUser, sshPassword);

                var connectionInfo = new ConnectionInfo(
                                        sshHostName,
                                        sshPort,
                                        sshUser,
                                        proxyTypes,
                                        sshProxyHost,
                                        sshProxyPort,
                                        sshProxyUser,
                                        sshProxyPassword,
                                        new PasswordAuthenticationMethod(sshUser, sshPassword)
                                        //new PrivateKeyAuthenticationMethod(sshUser, new PrivateKeyFile(sshPrivateKey))
                                        );

                if (RadioButtonCertLogin.IsChecked == true)
                {
                    connectionInfo = new ConnectionInfo(
                                            sshHostName,
                                            sshPort,
                                            sshUser,
                                            proxyTypes,
                                            sshProxyHost,
                                            sshProxyPort,
                                            sshProxyUser,
                                            sshProxyPassword,
                                            //new PasswordAuthenticationMethod(sshUser, sshPassword)
                                            new PrivateKeyAuthenticationMethod(sshUser, new PrivateKeyFile(sshPrivateKey))
                                            );

                }

                //using (var client = new SshClient(sshHostName, sshPort, sshUser, sshPassword))
                //Action<ConnectionInfo, TextBlock> startSetUpAction = new Action<ConnectionInfo, TextBlock>(StartSetUpRemoteHost);
                Task task = new Task(() => StartSetUpRemoteHost(connectionInfo, TextBlockSetUpProcessing, ProgressBarSetUpProcessing));
                task.Start();


          
            //catch (Exception ex1)//例外处理   
            #region 例外处理,未使用
            //{
            //    //MessageBox.Show(ex1.Message);
            //    if (ex1.Message.Contains("连接尝试失败") == true)
            //    {
            //        MessageBox.Show($"{ex1.Message}\n请检查主机地址及端口是否正确，如果通过代理，请检查代理是否正常工作");
            //    }
               
            //    else if (ex1.Message.Contains("denied (password)") == true)
            //    {
            //        MessageBox.Show($"{ex1.Message}\n密码错误或用户名错误");
            //    }
            //    else if (ex1.Message.Contains("Invalid private key file") == true)
            //    {
            //        MessageBox.Show($"{ex1.Message}\n所选密钥文件错误或者格式不对");
            //    }
            //    else if (ex1.Message.Contains("denied (publickey)") == true)
            //    {
            //        MessageBox.Show($"{ex1.Message}\n使用密钥登录，密钥文件错误或用户名错误");
            //    }
            //    else if (ex1.Message.Contains("目标计算机积极拒绝") == true)
            //    {
            //        MessageBox.Show($"{ex1.Message}\n主机地址错误，如果使用了代理，也可能是连接代理的端口错误");
            //    }
            //    else
            //    {
            //        MessageBox.Show("未知错误");
            //    }
            //    //TextBlockSetUpProcessing.Text = "主机登录失败";
            //    //ProgressBarSetUpProcessing.IsIndeterminate = false;
            //    //ProgressBarSetUpProcessing.Value = 0;
            //}
            #endregion
        }

        #region 端口数字防错代码，密钥选择代码
        private void Button_canel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
       // private static readonly Regex _regex = new Regex("[^0-9]+");
        private void TextBoxPort_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBoxPort_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void ButtonOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Cert Files (*.*)|*.*"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                TextBoxCertFilePath.Text = openFileDialog.FileName;
            }
        }
        #endregion

        #region 界面控件的有效无效控制代码块
        private void RadioButtonNoProxy_Checked(object sender, RoutedEventArgs e)
        {
            TextBlockProxyHost.IsEnabled = false;
            TextBlockProxyHost.Visibility = Visibility.Collapsed;
            TextBoxProxyHost.IsEnabled = false;
            TextBoxProxyHost.Visibility = Visibility.Collapsed;
            TextBlockProxyPort.IsEnabled = false;
            TextBlockProxyPort.Visibility = Visibility.Collapsed;
            TextBoxProxyPort.IsEnabled = false;
            TextBoxProxyPort.Visibility = Visibility.Collapsed;
            RadioButtonProxyNoLogin.IsEnabled = false;
            RadioButtonProxyNoLogin.Visibility = Visibility.Collapsed;
            RadiobuttonProxyYesLogin.IsEnabled = false;
            RadiobuttonProxyYesLogin.Visibility = Visibility.Collapsed;
            TextBlockProxyUser.IsEnabled = false;
            TextBlockProxyUser.Visibility = Visibility.Collapsed;
            TextBoxProxyUserName.IsEnabled = false;
            TextBoxProxyUserName.Visibility = Visibility.Collapsed;
            TextBlockProxyPassword.IsEnabled = false;
            TextBlockProxyPassword.Visibility = Visibility.Collapsed;
            PasswordBoxProxyPassword.IsEnabled = false;
            PasswordBoxProxyPassword.Visibility = Visibility.Collapsed;
        }

        private void RadioButtonNoProxy_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBlockProxyHost.IsEnabled = true;
            TextBlockProxyHost.Visibility = Visibility.Visible;
            TextBoxProxyHost.IsEnabled = true;
            TextBoxProxyHost.Visibility = Visibility.Visible;
            TextBlockProxyPort.IsEnabled = true;
            TextBlockProxyPort.Visibility = Visibility.Visible;
            TextBoxProxyPort.IsEnabled = true;
            TextBoxProxyPort.Visibility = Visibility.Visible;
            RadioButtonProxyNoLogin.IsEnabled = true;
            RadioButtonProxyNoLogin.Visibility = Visibility.Visible;
            RadiobuttonProxyYesLogin.IsEnabled = true;
            RadiobuttonProxyYesLogin.Visibility = Visibility.Visible;
            if (RadioButtonProxyNoLogin.IsChecked == true)
            {
                TextBlockProxyUser.IsEnabled = false;
                TextBlockProxyUser.Visibility = Visibility.Collapsed;
                TextBlockProxyPassword.IsEnabled = false;
                TextBlockProxyPassword.Visibility = Visibility.Collapsed;
                TextBoxProxyUserName.IsEnabled = false;
                TextBoxProxyUserName.Visibility = Visibility.Collapsed;
                PasswordBoxProxyPassword.IsEnabled = false;
                PasswordBoxProxyPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                TextBlockProxyUser.IsEnabled = true;
                TextBlockProxyUser.Visibility = Visibility.Visible;
                TextBoxProxyUserName.IsEnabled = true;
                TextBoxProxyUserName.Visibility = Visibility.Visible;
                TextBlockProxyPassword.IsEnabled = true;
                TextBlockProxyPassword.Visibility = Visibility.Visible;
                PasswordBoxProxyPassword.IsEnabled = true;
                PasswordBoxProxyPassword.Visibility = Visibility.Visible;
            }
        }

        private void RadioButtonPasswordLogin_Checked(object sender, RoutedEventArgs e)
        {
            ButtonOpenFileDialog.IsEnabled = false;
            ButtonOpenFileDialog.Visibility = Visibility.Collapsed;
            TextBoxCertFilePath.IsEnabled = false;
            TextBoxCertFilePath.Visibility = Visibility.Collapsed;
            TextBlockPassword.Text = "密码：";
            //TextBlockPassword.Visibility = Visibility.Visible;
            PasswordBoxHostPassword.IsEnabled = true;
            PasswordBoxHostPassword.Visibility = Visibility.Visible;
        }

        private void RadioButtonCertLogin_Checked(object sender, RoutedEventArgs e)
        {
            TextBlockPassword.Text = "密钥：";
            //TextBlockPassword.Visibility = Visibility.Collapsed;
            PasswordBoxHostPassword.IsEnabled = false;
            PasswordBoxHostPassword.Visibility = Visibility.Collapsed;
            ButtonOpenFileDialog.IsEnabled = true;
            ButtonOpenFileDialog.Visibility = Visibility.Visible;
            TextBoxCertFilePath.IsEnabled = true;
            TextBoxCertFilePath.Visibility = Visibility.Visible;
        }

        private void RadioButtonProxyNoLogin_Checked(object sender, RoutedEventArgs e)
        {
            TextBlockProxyUser.IsEnabled = false;
            TextBlockProxyUser.Visibility = Visibility.Collapsed;
            TextBlockProxyPassword.IsEnabled = false;
            TextBlockProxyPassword.Visibility = Visibility.Collapsed;
            TextBoxProxyUserName.IsEnabled = false;
            TextBoxProxyUserName.Visibility = Visibility.Collapsed;
            PasswordBoxProxyPassword.IsEnabled = false;
            PasswordBoxProxyPassword.Visibility = Visibility.Collapsed;
        }

        private void RadiobuttonProxyYesLogin_Checked(object sender, RoutedEventArgs e)
        {
            TextBlockProxyUser.IsEnabled = true;
            TextBlockProxyUser.Visibility = Visibility.Visible;
            TextBlockProxyPassword.IsEnabled = true;
            TextBlockProxyPassword.Visibility = Visibility.Visible;
            TextBoxProxyUserName.IsEnabled = true;
            TextBoxProxyUserName.Visibility = Visibility.Visible;
            PasswordBoxProxyPassword.IsEnabled = true;
            PasswordBoxProxyPassword.Visibility = Visibility.Visible;
        }
        #endregion

        //登录远程主机布署程序
        private void StartSetUpRemoteHost(ConnectionInfo connectionInfo,TextBlock textBlockName, ProgressBar progressBar)
        {
            string currentStatus = "正在登录远程主机......";
            Action<TextBlock, ProgressBar, string> updateAction = new Action<TextBlock, ProgressBar, string>(UpdateTextBlock);
            textBlockName.Dispatcher.BeginInvoke(updateAction, textBlockName, progressBar, currentStatus);

            try
            {
                #region 主机指纹，暂未启用
                //byte[] expectedFingerPrint = new byte[] {
                //                                0x66, 0x31, 0xaf, 0x00, 0x54, 0xb9, 0x87, 0x31,
                //                                0xff, 0x58, 0x1c, 0x31, 0xb1, 0xa2, 0x4c, 0x6b
                //                            };
                #endregion
                using (var client = new SshClient(connectionInfo))

                {
                    #region ssh登录验证主机指纹代码块，暂未启用
                    //    client.HostKeyReceived += (sender, e) =>
                    //    {
                    //        if (expectedFingerPrint.Length == e.FingerPrint.Length)
                    //        {
                    //            for (var i = 0; i < expectedFingerPrint.Length; i++)
                    //            {
                    //                if (expectedFingerPrint[i] != e.FingerPrint[i])
                    //                {
                    //                    e.CanTrust = false;
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            e.CanTrust = false;
                    //        }
                    //    };
                    #endregion

                    client.Connect();
                    if (client.IsConnected == true)
                    {
                        currentStatus = "主机登录成功";
                        textBlockName.Dispatcher.BeginInvoke(updateAction, textBlockName, progressBar, currentStatus);
                        Thread.Sleep(2000);
                    }
                    //检测远程主机系统环境是否符合要求
                    currentStatus = "检测系统是否符合安装要求......";
                    textBlockName.Dispatcher.BeginInvoke(updateAction, textBlockName, progressBar, currentStatus);
                    Thread.Sleep(2000);

                    var result = client.RunCommand("uname -r");
                    //var result = client.RunCommand("cat /root/test.ver");
                    string[] linuxKernelVerStr= result.Result.Split('-');
                    //MessageBox.Show(result.Result);
                    //MessageBox.Show(linuxKernelVerStr[0]);
                    bool detectResult = DetectKernelVersion(linuxKernelVerStr[0]);
                    if (detectResult == true)
                    {
                        currentStatus = "符合安装要求,布署中......";
                        textBlockName.Dispatcher.BeginInvoke(updateAction, textBlockName, progressBar, currentStatus);
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        MessageBox.Show($"当前系统内核版本为{linuxKernelVerStr[0]}，V2ray要求内核为2.6.23及以上。请升级内核再安装！");
                        currentStatus = "系统内核版本不符合要求，安装失败！！";
                        textBlockName.Dispatcher.BeginInvoke(updateAction, textBlockName, progressBar, currentStatus);
                        Thread.Sleep(2000);
                        
                    }

                     //运行命令
                    //client.RunCommand("apt update");
                    //client.RunCommand("apt install curl -y");
                    //client.RunCommand("bash <(curl -L -s https://install.direct/go.sh)");
                    //try
                    //{
                    //    using (var sftpClient = new SftpClient(connectionInfo))
                    //    {
                    //        sftpClient.Connect();
                    //        MessageBox.Show("sftp信息1" + sftpClient.ConnectionInfo.ServerVersion.ToString());
                    //        sftpClient.UploadFile(File.OpenRead("config\\config.json"),"/root/config.json", true);
                    //        sftpClient.DownloadFile("/root/id_rsa.pub", File.Create("config\\server_config.json"));
                    //        MessageBox.Show("sftp信息"+sftpClient.ConnectionInfo.ServerVersion.ToString());
                    //    }

                    //}
                    //catch(Exception ex2)
                    //{
                    //    MessageBox.Show("sftp"+ex2.ToString());
                    //    MessageBox.Show("sftp出现未知错误");
                    //}

                    // client.RunCommand("echo 1111 >> test.json");

                    //currentStatus = "安装成功";
                    //textBlockName.Dispatcher.BeginInvoke(updateAction, textBlockName, progressBar, currentStatus);
                    //Thread.Sleep(2000);
                    //MessageBox.Show("安装成功");
                    MessageBox.Show("ssh信息"+client.ConnectionInfo.ServerVersion.ToString());

                    //MessageBox.Show(client);
                    client.Disconnect();

                }
            }
            catch (Exception ex1)//例外处理   
            #region 例外处理
            {
                //MessageBox.Show(ex1.Message);
                if (ex1.Message.Contains("连接尝试失败") == true)
                {
                    MessageBox.Show($"{ex1.Message}\n请检查主机地址及端口是否正确，如果通过代理，请检查代理是否正常工作");
                }

                else if (ex1.Message.Contains("denied (password)") == true)
                {
                    MessageBox.Show($"{ex1.Message}\n密码错误或用户名错误");
                }
                else if (ex1.Message.Contains("Invalid private key file") == true)
                {
                    MessageBox.Show($"{ex1.Message}\n所选密钥文件错误或者格式不对");
                }
                else if (ex1.Message.Contains("denied (publickey)") == true)
                {
                    MessageBox.Show($"{ex1.Message}\n使用密钥登录，密钥文件错误或用户名错误");
                }
                else if (ex1.Message.Contains("目标计算机积极拒绝") == true)
                {
                    MessageBox.Show($"{ex1.Message}\n主机地址错误，如果使用了代理，也可能是连接代理的端口错误");
                }
                else
                {
                    MessageBox.Show("未知错误");
                }
                currentStatus = "主机登录失败";
                textBlockName.Dispatcher.BeginInvoke(updateAction, textBlockName, progressBar, currentStatus);

            }
            #endregion

        }
       
        //更新UI显示内容
        private void UpdateTextBlock(TextBlock textBlockName, ProgressBar progressBar, string currentStatus)
        {
            textBlockName.Text = currentStatus;
            if (currentStatus.Contains("正在登录远程主机") == true)
            {
                progressBar.IsIndeterminate = true;
            }
            else if (currentStatus.Contains("主机登录成功") == true)
            {
                progressBar.IsIndeterminate = true;
                //progressBar.Value = 100;
            }
            else if (currentStatus.Contains("检测系统是否符合安装要求") == true)
            {
                progressBar.IsIndeterminate = true;
                //progressBar.Value = 100;
            }
            else if (currentStatus.Contains("布署中") == true)
            {
                progressBar.IsIndeterminate = true;
                //progressBar.Value = 100;
            }
            else if (currentStatus.Contains("安装成功") == true)
            {
                progressBar.IsIndeterminate = false;
                progressBar.Value = 100;
            }
            else if (currentStatus.Contains("失败") == true)
            {
                progressBar.IsIndeterminate = false;
                progressBar.Value = 0;
            }


        }
        //检测系统内核是否符合安装要求
        private static bool DetectKernelVersion(string kernelVer)
        {
            string[] linuxKernelCompared = kernelVer.Split('.');
            if (int.Parse(linuxKernelCompared[0]) > 2)
            {
                //MessageBox.Show($"当前系统内核版本为{result.Result}，符合安装要求！");
                return true;
            }
            else if (int.Parse(linuxKernelCompared[0]) < 2)
            {
                //MessageBox.Show($"当前系统内核版本为{result.Result}，V2ray要求内核为2.6.23及以上。请升级内核再安装！");
                return false;
            }
            else if (int.Parse(linuxKernelCompared[0]) == 2)
            {
                if (int.Parse(linuxKernelCompared[1]) > 6)
                {
                    //MessageBox.Show($"当前系统内核版本为{result.Result}，符合安装要求！");
                    return true;
                }
                else if (int.Parse(linuxKernelCompared[1]) < 6)
                {
                    //MessageBox.Show($"当前系统内核版本为{result.Result}，V2ray要求内核为2.6.23及以上。请升级内核再安装！");
                    return false;
                }
                else if (int.Parse(linuxKernelCompared[1]) == 6)
                {
                    if (int.Parse(linuxKernelCompared[2]) < 23)
                    {
                        //MessageBox.Show($"当前系统内核版本为{result.Result}，V2ray要求内核为2.6.23及以上。请升级内核再安装！");
                        return false;
                    }
                    else
                    {
                        //MessageBox.Show($"当前系统内核版本为{result.Result}，符合安装要求！");
                        return true;
                    }

                }
            }
            return false;

        }
        //检测发行版本号是否为centos7/8 debian 8/9/10 ubuntu 16.04及以上
        private static bool DetectReleaseVersion(string releasever)
        {

            return true;
        }

    }
    
}
