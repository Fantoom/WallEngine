using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DialogController
{
    /// <summary>
    /// Interaction logic for OkDialog.xaml
    /// </summary>
    public partial class OkDialog : UserControl
    {
        public OkDialog(OkDialogViewModel viewModel)
        {
            InitializeComponent();
			DataContext = viewModel;
        }
    }

	public class OkDialogViewModel
	{
		public string Message { get; set; } = "Enter some text";
		public string AffirmativeButtonText { get; set; } = "OK";
		
	}
}
