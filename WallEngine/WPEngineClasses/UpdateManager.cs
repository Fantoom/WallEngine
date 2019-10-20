using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Onova;
using Onova.Services;

namespace WPEngine.WPEngineClasses
{
	class UpdateManager
	{
		private readonly  IUpdateManager _updateManager = new Onova.UpdateManager(
			new GithubPackageResolver("Fantoom", "WallEngine", "WallEngine-v*"), new UpdateUnzipper());

		public async void CheckForUpdate()
		{
			Onova.Models.CheckForUpdatesResult check;

			try
			{
				check = await _updateManager.CheckForUpdatesAsync();

			}
			catch (Exception)
			{
				return;
			}

			// If there are none, notify user and return
			if (!check.CanUpdate)
			{
				return;
			}
			else
			{
				 System.Windows.Forms.MessageBox.Show("There are updates available.");
			}

			// Prepare the latest update
			await _updateManager.PrepareUpdateAsync(check.LastVersion);

			// Launch updater and exit
			_updateManager.LaunchUpdater(check.LastVersion);
			if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 0)
			{
				//System.Windows.MessageBox.Show("Closed to update", "WallEngine Updater", System.Windows.MessageBoxButton.OK);
				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
		}
	}
	class UpdateUnzipper : IPackageExtractor
	{
		public async Task ExtractAsync(string sourceFilePath, string destDirPath, IProgress<double> progress = null, CancellationToken cancellationToken = default)
		{
				 FastZip fastZip = new FastZip();
				 await Task.Run(() => fastZip.ExtractZip(sourceFilePath, destDirPath, null));
		}
	}
}
