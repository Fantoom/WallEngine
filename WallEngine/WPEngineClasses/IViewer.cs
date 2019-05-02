using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPEngine.WPEngineClasses
{
	interface IViewer
	{
		IntPtr hWnd { get; }

		void OpenViewer();
		void UpdateView();
		void SaveLastProject();
	}
}
