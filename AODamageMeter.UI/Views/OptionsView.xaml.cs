using AODamageMeter.UI.Properties;
using AODamageMeter.UI.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace AODamageMeter.UI.Views
{
    public partial class OptionsView : Window
    {
        private string _previousFontFamily = Settings.Default.FontFamily;
        private double _previousFontSize = Settings.Default.FontSize;
        private int _previousRefreshInterval = Settings.Default.RefreshInterval;
        private int _previousMaxNumberOfDetailRows = Settings.Default.MaxNumberOfDetailRows;
        private bool _previousShowPercentOfTotal = Settings.Default.ShowPercentOfTotal;
        private bool _previousShowRowNumbers = Settings.Default.ShowRowNumbers;
        private bool _previousIncludeTopLevelNPCRows = Settings.Default.IncludeTopLevelNPCRows;
        private bool _previousIncludeTopLevelZeroDamageRows = Settings.Default.IncludeTopLevelZeroDamageRows;
		private string _selectedScriptsPath = Settings.Default.SelectedScriptsPath;

		public OptionsViewModel OptionsViewModel { get; }
		public OptionsView(OptionsViewModel optionsViewModel)
        {
            InitializeComponent();
			DataContext = this;
			ShowPercentOfTotalRadioButton.IsChecked = Settings.Default.ShowPercentOfTotal;
            ShowPercentOfMaxRadioButton.IsChecked = !ShowPercentOfTotalRadioButton.IsChecked;
			txtSelectedPath.Text = Settings.Default.SelectedScriptsPath;

		}

        private void OKButton_Click_CloseDialog(object sender, RoutedEventArgs e)
            => DialogResult = true;

        private void HeaderRow_MouseDown_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

		private void ChooseButton_Click_ShowFileDialog(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				_selectedScriptsPath = dialog.SelectedPath;
				Settings.Default.SelectedScriptsPath = dialog.SelectedPath;
				txtSelectedPath.Text = _selectedScriptsPath;
			}
		}

		private void ShowPercentOfTotalRadioButton_Checked_Persist(object sender, RoutedEventArgs e)
            => Settings.Default.ShowPercentOfTotal = true;

        private void ShowPercentOfTotalRadioButton_Unchecked_Persist(object sender, RoutedEventArgs e)
            => Settings.Default.ShowPercentOfTotal = false;

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != true)
            {
                Settings.Default.FontFamily = _previousFontFamily;
                Settings.Default.FontSize = _previousFontSize;
                Settings.Default.RefreshInterval = _previousRefreshInterval;
                Settings.Default.MaxNumberOfDetailRows = _previousMaxNumberOfDetailRows;
                Settings.Default.ShowPercentOfTotal = _previousShowPercentOfTotal;
                Settings.Default.ShowRowNumbers = _previousShowRowNumbers;
                Settings.Default.IncludeTopLevelNPCRows = _previousIncludeTopLevelNPCRows;
                Settings.Default.IncludeTopLevelZeroDamageRows = _previousIncludeTopLevelZeroDamageRows;
				Settings.Default.SelectedScriptsPath = _selectedScriptsPath;
			}
            else
            {
                Settings.Default.Save();

				if (!string.IsNullOrEmpty(Settings.Default.SelectedScriptsPath) && !System.IO.Directory.Exists(Settings.Default.SelectedScriptsPath + "\\scripts"))
				{
					System.IO.Directory.CreateDirectory(Settings.Default.SelectedScriptsPath + "\\scripts");
				}

			}

            base.OnClosing(e);
        }
    }
}
