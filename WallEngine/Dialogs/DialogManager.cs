using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace DialogController
{
	/// <summary>
	/// Helper extensions for showing dialogs.
	/// </summary>
	public class DialogManager
	{
		/// <summary>
		/// Shows a dialog to get <see cref="User Input"/> (DialogHost is "MainDialogHost"). Use in async method
		/// </summary>
		/// <param name="Message"> A Message that will be shown in dialog.</param>
		/// <returns>String</returns>  
		/// <example> 
		/// This sample shows how to call the <see cref="InputDialog"/> method.
		/// <code>
		/// var result = await Dmanager.InputDialog("Your Message");
		/// </code>
		/// </example>
		public async Task<string> InputDialog(string Message)
		{
			var viewModel = new TextInputDialogViewModel
			{
				Message = Message
			};

			var dialog = new TextInputDialog(viewModel);
			var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "MainDialogHost");


			if (result)
			{
				return viewModel.Text;
			}
			return "";
		}

		/// <summary>
		/// Shows a dialog to get <see cref="User Input"/> with custom DialogHost. Use in async method
		/// </summary>
		/// <param name="Message"> A Message that will be shown in dialog.</param>
		/// <param name="DialogHost"> Name of DialogHost where dialog will be shown.</param>
		/// <returns>String</returns>  
		/// <example> 
		/// This sample shows how to call the <see cref="InputDialog"/> method.
		/// <code>
		/// var result = await Dmanager.InputDialog("Your Message","MainDialogHost");
		/// </code>
		/// </example>
		public async Task<string> InputDialog(string Message, string DialogHost)
		{
			var viewModel = new TextInputDialogViewModel
			{
				Message = Message
			};

			var dialog = new TextInputDialog(viewModel);
			var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, DialogHost);


			if (result)
			{
				return viewModel.Text;
			}
			return "";
		}

		/// <summary>
		/// Shows a dialog to get <see cref="User Input"/> with custom buttons text. Use in async method
		/// </summary>
		/// <param name="Message"> A Message that will be shown in dialog.</param>
		/// <param name="DialogHost"> Name of DialogHost where dialog will be shown.</param>
		/// <param name="AffirmativeButton"> Text of Affirmative Button </param>
		/// <param name="NegativeButton"> Text of Negative Button</param>
		/// <returns>String</returns>  
		/// <example> 
		/// This sample shows how to call the <see cref="InputDialog"/> method.
		/// <code>
		/// var result = await Dmanager.InputDialog("Your Message","MainDialogHost" , "OK","Cancel");
		/// </code>
		/// </example>
		public async Task<string> InputDialog(string Message, string DialogHost, string AffirmativeButton, string NegativeButton)
		{
			var viewModel = new TextInputDialogViewModel
			{
				Message = Message,
				AffirmativeButtonText = AffirmativeButton,
				NegativeButtonText = NegativeButton
			};

			var dialog = new TextInputDialog(viewModel);
			var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, DialogHost);


			if (result)
			{
				return viewModel.Text;
			}
			return "";
		}

		/// <summary>
		/// Shows a dialog to get <see cref="User Input"/> with custom buttons text. Use in async method
		/// </summary>
		/// <param name="Message"> A Message that will be shown in dialog.</param>
		/// <param name="AffirmativeButton"> Text of Affirmative Button </param>
		/// <param name="NegativeButton"> Text of Negative Button</param>
		/// <returns>String</returns>  
		/// <example> 
		/// This sample shows how to call the <see cref="InputDialog"/> method.
		/// <code>
		/// var result = await Dmanager.InputDialog("Your Message","MainDialogHost" , "OK","Cancel");
		/// </code>
		/// </example>
		public async Task<string> InputDialog(string Message, string AffirmativeButton, string NegativeButton)
		{
			var viewModel = new TextInputDialogViewModel
			{
				Message = Message,
				AffirmativeButtonText = AffirmativeButton,
				NegativeButtonText = NegativeButton
			};

			var dialog = new TextInputDialog(viewModel);
			var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, "MainDialogHost");


			if (result)
			{
				return viewModel.Text;
			}
			return "";
		}

		/// <summary>
		/// Shows a dialog to show <see cref="Text"/>.
		/// </summary>
		/// <param name="Message"> A Message that will be shown in dialog.</param>
		/// <param name="DialogHost"> [Optional] Name of DialogHost where dialog will be shown. (Default "MainDialogHost" ) </param>
		/// <param name="AffirmativeButton"> [Optional] Text of Affirmative Button. (Default "OK" ) </param>
		/// <returns>Nothing</returns>  
		/// <example> 
		/// This sample shows how to call the <see cref="ShowText"/> method.
		/// <code>
		/// Dmanager.ShowText("Your Message","MainDialogHost" , "OK");
		/// </code>
		/// </example>
		/// 
		public void ShowText(string Message, [Optional] string DialogHost, [Optional] string AffirmativeButton)
		{
			string host = DialogHost ?? "MainDialogHost";
			var viewModel = new OkDialogViewModel
			{
				Message = Message,
				AffirmativeButtonText = AffirmativeButton ?? "OK"
			};
			var dialog = new OkDialog(viewModel);
			MaterialDesignThemes.Wpf.DialogHost.Show(dialog, host);
		}

		public void ShowSelectableMessage(string Message, string SelectableText, [Optional] string DialogHost, [Optional] string AffirmativeButton)
		{
			string host = DialogHost ?? "MainDialogHost";
			var viewModel = new SelectableMessageViewModel
			{
				Message = Message,
				AffirmativeButtonText = AffirmativeButton ?? "OK",
				Text = SelectableText
			};
			var dialog = new SelectableMessage(viewModel);
			MaterialDesignThemes.Wpf.DialogHost.Show(dialog, host);
		}

		/// <summary>
		/// Shows a dialog to show <see cref="Text"/>. An Async variant of showtext for getting bool return
		/// </summary>
		/// <param name="Message"> A Message that will be shown in dialog.</param>
		/// <param name="DialogHost"> [Optional] Name of DialogHost where dialog will be shown. (Default "MainDialogHost" ) </param>
		/// <param name="AffirmativeButton"> [Optional] Text of Affirmative Button. (Default "OK" ) </param>
		/// <returns>Nothing</returns>  
		/// <example> 
		/// This sample shows how to call the <see cref="InputDialog"/> method.
		/// <code>
		/// var result = await Dmanager.ShowText("Your Message","MainDialogHost" , "OK");
		/// </code>
		/// </example>
		/// 
		public async Task<bool> ShowTextAsync(string Message, [Optional] string DialogHost, [Optional] string AffirmativeButton)
		{
			string host = DialogHost ?? "MainDialogHost";
			var viewModel = new OkDialogViewModel
			{
				Message = Message,
				AffirmativeButtonText = AffirmativeButton ?? "OK"
			};
			var dialog = new OkDialog(viewModel);
			var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, host);
			return result;
		}

		/// <summary>
		/// Shows a dialog to show <see cref="Text"/>. An Async variant of showtext for getting bool return
		/// </summary>
		/// <param name="Message"> A Message that will be shown in dialog.</param>
		/// <param name="DialogHost"> [Optional] Name of DialogHost where dialog will be shown. (Default "MainDialogHost" ) </param>
		/// <param name="AffirmativeButton"> [Optional] Text of Affirmative Button. (Default "OK" ) </param>
		/// <returns>Nothing</returns>  
		/// <example> 
		/// This sample shows how to call the <see cref="InputDialog"/> method.
		/// <code>
		/// var result = await Dmanager.ShowText("Your Message","MainDialogHost" , "OK");
		/// </code>
		/// </example>
		/// 
		public async Task<bool> ShowOkCancel(string Message, [Optional] string DialogHost, [Optional] string AffirmativeButton , [Optional] string NegativeButton)
		{
			string host = DialogHost ?? "MainDialogHost";
			var viewModel = new OkCancelDialogViewModel
			{
				Message = Message,
				AffirmativeButtonText = AffirmativeButton ?? "OK",
				NegativeButtonText = NegativeButton ?? "CANCEL"
			};
			var dialog = new OkCancelDialog(viewModel);
			var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(dialog, host);
			return result;
		}




	}
}
