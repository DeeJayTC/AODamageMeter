﻿using AODamageMeter.UI.ViewModels;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AODamageMeter.UI.Views
{
    public partial class CharacterInfoView : Window
    {
        private bool _isEditMode;
        private string _previousCharacterName;
        private string _previousLogFilePath;

        public CharacterInfoView(CharacterInfoViewModel characterInfoViewModel = null)
        {
            InitializeComponent();
            _isEditMode = characterInfoViewModel != null;
            Title = _isEditMode ? "Edit Character" : "Add Character";
            DataContext = CharacterInfoViewModel = characterInfoViewModel ?? new CharacterInfoViewModel();
            _previousCharacterName = CharacterInfoViewModel.CharacterName;
            _previousLogFilePath = CharacterInfoViewModel.LogFilePath;
        }

        public CharacterInfoViewModel CharacterInfoViewModel { get; }

        private void ChooseButton_Click_ShowFileDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                FileName = "Log.txt",
                DefaultExt = ".txt",
                Filter = "Log File(*.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                CharacterInfoViewModel.LogFilePath = dialog.FileName;
            }
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_isEditMode && DialogResult != true)
            {
                CharacterInfoViewModel.CharacterName = _previousCharacterName;
                CharacterInfoViewModel.LogFilePath = _previousLogFilePath;
            }
        }
    }
}
