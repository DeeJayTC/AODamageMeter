using AODamageMeter.FightEvents.Attack;
using AODamageMeter.FightEvents.Heal;
using AODamageMeter.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace AODamageMeter.UI.ViewModels
{
	public sealed class OptionsViewModel : ViewModelBase
	{

		public OptionsViewModel(string logFilePath = null)
		{
			LogFilePath = logFilePath;
		}


		private string _logFilePath;
		public string LogFilePath
		{
			get => _logFilePath;
			set
			{
				_logFilePath = value;
				RaisePropertyChanged("LogFilePath");
			}
		}

	}
}
