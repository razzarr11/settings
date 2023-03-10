using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using x360ce.Engine;
using x360ce.App.Properties;

namespace x360ce.App.Controls
{
	public partial class OptionsControl : UserControl
	{
		public OptionsControl()
		{
			InitializeComponent();
			if (DesignMode) return;
		}

		public void InitOptions()
		{
			DebugModeCheckBox_CheckedChanged(DebugModeCheckBox, null);
		}

		void DebugModeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			var cbx = (CheckBox)sender;
			if (!cbx.Checked) Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Program.Application_ThreadException);
			else Application.ThreadException -= new System.Threading.ThreadExceptionEventHandler(Program.Application_ThreadException);
		}

		/// <summary>
		/// Link control with INI key. Value/Text of control will be automatically tracked and INI file updated.
		/// </summary>
		public void InitSettingsManager()
		{
			// INI setting keys with controls.
			string section = @"Options\";
			SettingManager.AddMap(section, () => SettingName.UseInitBeep, UseInitBeepCheckBox);
			SettingManager.AddMap(section, () => SettingName.DebugMode, DebugModeCheckBox);
			SettingManager.AddMap(section, () => SettingName.Log, EnableLoggingCheckBox);
			SettingManager.AddMap(section, () => SettingName.Console, ConsoleCheckBox);
			SettingManager.AddMap(section, () => SettingName.InternetDatabaseUrl, InternetDatabaseUrlComboBox);
			SettingManager.AddMap(section, () => SettingName.InternetFeatures, InternetCheckBox);
			SettingManager.AddMap(section, () => SettingName.AllowOnlyOneCopy, AllowOnlyOneCopyCheckBox);
			SettingManager.AddMap(section, () => SettingName.ProgramScanLocations, GameScanLocationsListBox);
			SettingManager.AddMap(section, () => SettingName.Version, ConfigurationVersionTextBox);
			SettingManager.AddMap(section, () => SettingName.CombineEnabled, CombineEnabledCheckBox);
			SettingManager.AddMap(section, "InternetAutoload", InternetAutoloadCheckBox, SettingManager.Current.SettingsMap);
			SettingManager.AddMap(section, "ExcludeSupplementalDevices", ExcludeSupplementalDevicesCheckBox, SettingManager.Current.SettingsMap);
			SettingManager.AddMap(section, "ExcludeVirtualDevices", ExcludeVirtualDevicesCheckBox, SettingManager.Current.SettingsMap);
		}

		void InternetCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			InternetAutoloadCheckBox.Enabled = InternetCheckBox.Checked;
		}

		private void AddLocationButton_Click(object sender, EventArgs e)
		{
			var path = LocationFolderBrowserDialog.SelectedPath;
			if (string.IsNullOrEmpty(path)) path = GameScanLocationsListBox.Text;
			if (string.IsNullOrEmpty(path)) path = System.IO.Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			LocationFolderBrowserDialog.SelectedPath = path;
			LocationFolderBrowserDialog.Description = "Browse for Scan Location";
			var result = LocationFolderBrowserDialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				// Don't allow to add windows folder.
				var winFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Windows);
				if (LocationFolderBrowserDialog.SelectedPath.StartsWith(winFolder, StringComparison.OrdinalIgnoreCase))
				{
					MessageBoxForm.Show("Windows folders are not allowed.", "Windows Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					if (!Contains(LocationFolderBrowserDialog.SelectedPath))
					{
						GameScanLocationsListBox.Items.Add(LocationFolderBrowserDialog.SelectedPath);
						// Change selected index for change event to fire.
						GameScanLocationsListBox.SelectedIndex = GameScanLocationsListBox.Items.Count - 1;
					}
				}
			}
		}

		private void RemoveLocationButton_Click(object sender, EventArgs e)
		{
			if (GameScanLocationsListBox.SelectedIndex == -1) return;
			var currentIndex = GameScanLocationsListBox.SelectedIndex;
			GameScanLocationsListBox.Items.RemoveAt(currentIndex);
			// Change selectd index for change event to fire.
			GameScanLocationsListBox.SelectedIndex = Math.Min(currentIndex, GameScanLocationsListBox.Items.Count - 1);
		}

		private void ProgramScanLocationsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			RemoveLocationButton.Enabled = GameScanLocationsListBox.SelectedIndex > -1;
		}

		bool Contains(string path)
		{
			var paths = GameScanLocationsListBox.Items.Cast<string>().ToArray();
			for (int i = 0; i < paths.Length; i++)
			{
				if (paths[i].ToLower() == path.ToLower()) return true;
			}
			return false;
		}

		private void RefreshLocationsButton_Click(object sender, EventArgs e)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			if (!Contains(path)) GameScanLocationsListBox.Items.Add(path);
			path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			if (!Contains(path)) GameScanLocationsListBox.Items.Add(path);
			DriveInfo[] allDrives = DriveInfo.GetDrives();
			foreach (DriveInfo d in allDrives)
			{
				if (d.IsReady == true && d.DriveType == DriveType.Fixed)
				{
					try
					{
						var programDirs = d.RootDirectory.GetDirectories("Program Files*");
						for (int i = 0; i < programDirs.Count(); i++)
						{
							path = programDirs[i].FullName;
							if (!Contains(path)) GameScanLocationsListBox.Items.Add(path);
						}
					}
					catch (Exception) { }
				}
			}
		}

		private void SaveSettingsButton_Click(object sender, EventArgs e)
		{
			MainForm.Current.SaveSettings();
		}

		private void OpenSettingsFolderButton_Click(object sender, EventArgs e)
		{
			GameDatabaseManager.Current.CheckSettingsFolder();
			EngineHelper.BrowsePath(GameDatabaseManager.Current.GdbFile.FullName);
		}

		private void MinimizeToTrayCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			MainForm.Current.SetMinimizeToTray(!Settings.Default.MinimizeToTray);
		}
	}
}
