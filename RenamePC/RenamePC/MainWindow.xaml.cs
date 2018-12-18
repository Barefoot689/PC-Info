using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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

namespace RenamePC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void RenameRemotePC(String oldName, String newName, String domain, System.Net.NetworkCredential accountWithPermissions)
        {
            var remoteControlObject = new ManagementPath
            {
                ClassName = "Win32_ComputerSystem",
                Server = oldName,
                Path = oldName + "\\root\\cimv2:Win32_ComputerSystem.Name='" + oldName + "'",
                NamespacePath = "\\\\" + oldName + "\\root\\cimv2"
            };

            var conn = new ConnectionOptions
            {
                Authentication = AuthenticationLevel.PacketPrivacy,
                Username = oldName + "\\" + accountWithPermissions.UserName,
                Password = accountWithPermissions.Password
            };

            var remoteScope = new ManagementScope(remoteControlObject, conn);

            var remoteSystem = new ManagementObject(remoteScope, remoteControlObject, null);

            ManagementBaseObject newRemoteSystemName = remoteSystem.GetMethodParameters("Rename");
            var methodOptions = new InvokeMethodOptions();

            newRemoteSystemName.SetPropertyValue("Name", newName);
            newRemoteSystemName.SetPropertyValue("UserName", accountWithPermissions.UserName);
            newRemoteSystemName.SetPropertyValue("Password", accountWithPermissions.Password);

            methodOptions.Timeout = new TimeSpan(0, 10, 0);
            ManagementBaseObject outParams = remoteSystem.InvokeMethod("Rename", newRemoteSystemName, null);

        }
    }
}
