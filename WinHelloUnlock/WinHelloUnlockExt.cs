﻿using KeePass.Plugins;
using KeePass.Forms;
using KeePass.UI;
using System;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using KeePassLib;
using KeePassLib.Utility;
using KeePassLib.Serialization;

namespace WinHelloUnlock
{
    public class WinHelloUnlockExt : Plugin
    {
        private static IPluginHost host = null;
        public const string ShortProductName = "HelloUnlock";
        public const string ProductName = "WinHelloUnlock";
        public static string dbName;
        public static PwDatabase database = null;
        public static bool enablePlugin = false;
        public static int tries = 0;
        public static bool opened = true;

        public static IPluginHost Host
        {
            get { return host; }
        }

        public override Image SmallIcon
        {
            get { return Properties.Resources.windows_hello16x16; }
        }

        public override string UpdateUrl
        {
            get { return "https://github.com/Angelelz/WinHelloUnlock/raw/master/WinHelloUnlock/keepass.version"; }
        }

        public override bool Initialize(IPluginHost _host)
        {
            if (host != null)
            {
                Debug.Assert(false);
                Terminate();
            }
            if (_host == null) { return false; }

            host = _host;

            GlobalWindowManager.WindowAdded += WindowAddedHandler;
            host.MainWindow.FileOpened += FileOpenedHandler;
            host.MainWindow.DocumentManager.ActiveDocumentSelected += ActiveDocChanged;

            return true;
        }

        public override void Terminate()
        {
            if (host == null) { return; }

            GlobalWindowManager.WindowAdded -= WindowAddedHandler;
            host.MainWindow.FileOpened -= FileOpenedHandler;
            host.MainWindow.DocumentManager.ActiveDocumentSelected -= ActiveDocChanged;

            host = null;
        }

        /// <summary>
		/// Called everytime a database is opened.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private async void FileOpenedHandler(object sender, FileOpenedEventArgs e)
        {
            var ioInfo = e.Database.IOConnectionInfo;
            if (e.Database.CustomData.Get(ProductName) == null)
            {
                e.Database.CustomData.Set(ProductName, "true");
                e.Database.Modified = true;
                try { e.Database.Save(null); }
                catch { }
            }

            if (e.Database.CustomData.Get(ProductName) == "true") enablePlugin = true;
            if (e.Database.CustomData.Get(ProductName) == "false")
            {
                enablePlugin = false;
                dbName = Library.CharChange(ioInfo.Path);
                database = e.Database;
                return;
            }
            
            dbName = Library.CharChange(ioInfo.Path);
            database = e.Database;

            bool firstTime = await UWPLibrary.FirstTime(dbName);

            if (firstTime)
            {
                bool yesOrNo = MessageService.AskYesNo("Do You want to set " +
                    WinHelloUnlockExt.ProductName + " for " + dbName + " now?", WinHelloUnlockExt.ShortProductName, true);

                if (yesOrNo)
                    await UWPLibrary.CreateHelloData(dbName);
            }
            tries = 0;
            opened = true;
        }

        /// <summary>
		/// Used to modify other form when they load.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void WindowAddedHandler(object sender, GwmWindowEventArgs e)
        {
            
            if (e.Form is KeyPromptForm keyPromptForm)
            {
                var fieldInfo = keyPromptForm.GetType().GetField("m_ioInfo", BindingFlags.Instance | BindingFlags.NonPublic);
                var ioInfo = fieldInfo.GetValue(keyPromptForm) as IOConnectionInfo;
                string dbName = Library.CharChange(ioInfo.Path);
                bool isHelloAvailable = await UWPLibrary.IsHelloAvailable();

                if (!await UWPLibrary.FirstTime(dbName) && isHelloAvailable)
                {
                    if (opened)
                    {
                        opened = false;
                        Library.UnlockDatabase(ioInfo, keyPromptForm);
                    }
                    else
                        Library.CloseFormWithResult(keyPromptForm, DialogResult.Cancel);
                }
                else if (!await UWPLibrary.FirstTime(dbName))
                {
                    MessageService.ShowInfo("This Database has credential data saved. Enable Windows Hello to use.");
                }
            }
            if (e.Form is OptionsForm optionsForm)
            {
                if (!host.MainWindow.ActiveDatabase.IsOpen) return;
                optionsForm.Shown += delegate (object sender2, EventArgs e2)
                {
                    
                    try
                    {
                        Library.AddWinHelloOptions(optionsForm);
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("WinHelloUnlock Error: " + ex.Message);
                    }
                };
            }
        }

        private void ActiveDocChanged(object sender, EventArgs e)
        {
            database = Host.MainWindow.ActiveDatabase;
            var ioInfo = database.IOConnectionInfo;
            dbName = Library.CharChange(ioInfo.Path);
        }

    }
}