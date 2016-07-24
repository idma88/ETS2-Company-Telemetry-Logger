﻿using System;
using System.Configuration;
using System.Reflection;
using System.Windows.Forms;
using Funbit.Ets.Telemetry.Server.Helpers;

namespace Funbit.Ets.Telemetry.Server.Setup
{
    public class FirewallSetup : ISetup
    {
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        static readonly string FirewallRuleName = string.Format(
            "ETS2 TELEMETRY SERVER (PORT {0})", ConfigurationManager.AppSettings["Port"]);

        SetupStatus _status;

        public FirewallSetup()
        {
            try
            {
                if (Settings.Instance.FirewallSetupHadErrors)
                {
                    _status = SetupStatus.Installed;
                }
                else
                {
                    string port = ConfigurationManager.AppSettings["Port"];
                    string arguments = string.Format("advfirewall firewall show rule dir=in name=all");
                    Log.Info("Checking Firewall rule...");
                    string output = ProcessHelper.RunNetShell(arguments, "Failed to check Firewall rule status");
                    // this check is kind of lame, but it works in any locale...
                    _status = output.Contains(port) && output.Contains(FirewallRuleName)
                        ? SetupStatus.Installed : SetupStatus.Uninstalled;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                _status = SetupStatus.Failed;
            }
        }

        public SetupStatus Status
        {
            get { return _status; }
        }

        public SetupStatus Install(IWin32Window owner)
        {
            if (_status == SetupStatus.Installed)
                return _status;

            try
            {
                string port = ConfigurationManager.AppSettings["Port"];
                string arguments = string.Format("advfirewall firewall add rule name=\"{0}\" " +
                    "dir=in action=allow protocol=TCP localport={1} remoteip=localsubnet", FirewallRuleName, port);
                Log.Info("Adding Firewall rule...");
                ProcessHelper.RunNetShell(arguments, "Failed to add Firewall rule");
                _status = SetupStatus.Installed;
            }
            catch (Exception ex)
            {
                _status = SetupStatus.Failed;
                Log.Error(ex);
                Settings.Instance.FirewallSetupHadErrors = true;
                Settings.Instance.Save();
                throw new Exception("Cannot configure Windows Firewall." + Environment.NewLine +
                                    "If you are using some 3rd-party firewall please open " +
                                    ConfigurationManager.AppSettings["Port"] + " TCP port manually!", ex);
            }

            return _status;
        }

        public SetupStatus Uninstall(IWin32Window owner)
        {
            if (_status == SetupStatus.Uninstalled)
                return _status;

            SetupStatus status;
            try
            {
                string arguments = string.Format("advfirewall firewall delete rule name=\"{0}\"", FirewallRuleName);
                Log.Info("Deleting Firewall rule...");
                ProcessHelper.RunNetShell(arguments, "Failed to delete Firewall rule");
                status = SetupStatus.Uninstalled;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                _status = SetupStatus.Failed;
                throw new Exception("Cannot configure Windows Firewall." + Environment.NewLine +
                                    "If you are using some 3rd-party firewall please close " +
                                    ConfigurationManager.AppSettings["Port"] + " TCP port manually!", ex);
            }
            return status;
        }
    }
}