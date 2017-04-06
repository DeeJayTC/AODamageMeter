using AODamageMeter.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace AODamageMeter.UI.ViewModels
{
    public class DamageMeterViewModel : ViewModelBase
    {
        private string _logFile;

        public DamageMeterViewModel()
        { }

        public void SetLogFile(string logFile)
        {
            DamageMeter.Stop();
            DamageMeter.Dispose();
            DamageMeter = new DamageMeter(logFile, TimeSpan.FromSeconds(.3), () =>
            {
                foreach (var character in (IEnumerable<FightCharacter>)e.UserState)
                {
                    if (Rows.Any(r => r.LeftText == character.Name))
                    {
                        Rows.Single(r => r.LeftText == character.Name).Update(character);
                    }
                    else if (character.DamageDone != 0)
                    {
                        Rows.Add(new DpsRowViewModel(character));
                    }
                }
            });
        }

        public DamageMeter DamageMeter { get; private set; } = DamageMeter.Empty;

        //private ObservableCollection<DamageDoneRow> DamageDoneRows = new ObservableCollection<DamageDoneRow>();
        private ObservableCollection<RowViewModelBase> Rows = new ObservableCollection<RowViewModelBase>();

        public ICollectionViewLiveShaping LiveCollection { get; set; }

        public ICommand ResetMeterCommand => new DelegateCommand(ResetMeter);
        public void ResetMeter()
        {
            if (logFile == null)
                return;

            worker.CancelAsync();
        }

        public ICommand FileBrowseCommand => new DelegateCommand(FileBrowse);
        public async void FileBrowse()
        {
            if (worker != null)
                worker.CancelAsync();

            if (result == true && logFile != dialog.FileName)
            {
                logFile = dialog.FileName;


                DamageMeter = await DamageMeter.Create(logFile);



                StartBackgroundWorker();
            }
        }

        public ICommand TerminateCommand => new DelegateCommand(Terminate);
        public void Terminate()
        {
            Application.Current.Shutdown();
        }

        void StartBackgroundWorker()
        {
            //DamageDoneRows.RemoveAll();
            Rows.RemoveAll();

            worker = new BackgroundWorker();

            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.RunWorkerAsync();
        }

        //public void ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    foreach (var character in (List<Character>)e.UserState)
        //    {
        //        if (DamageDoneRows.Any(c => c.Name == character.Name))
        //        {
        //            DamageDoneRows.Single(c => c.Name == character.Name).Update(character);
        //        }
        //        else if (character.DamageDone != 0)
        //        {
        //            DamageDoneRows.Add(DamageDoneRow.Create(character));
        //        }
        //    }
        //}

        public void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        public void DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    Meter = new DamageMeter(logFile);
                    return;
                }
                Meter.Update();
                worker.ReportProgress(0, Meter.CurrentFight.CharactersList);
                Thread.Sleep(300);
            }
        }

        public void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                StartBackgroundWorker();
            }
        }
    }
}






























//namespace AODamageMeter.ViewModel
//{
//    public class MainWindowViewModel : BindableBase
//    {
//        public DamageMeter Meter = new DamageMeter();

//        private BackgroundWorker worker;
//        private string logFile;

//        private ObservableCollection<DamageDoneRow> DamageDoneRows = new ObservableCollection<DamageDoneRow>();
//        public ICollectionViewLiveShaping LiveCollection { get; set; }

//        public ICommand ResetMeterCommand => new DelegateCommand(ResetMeter);
//        public void ResetMeter()
//        {
//            if (logFile == null)
//                return;

//            worker.CancelAsync();
//        }

//        public ICommand FileBrowseCommand => new DelegateCommand(FileBrowse);
//        public void FileBrowse()
//        {
//            if (worker != null)
//                worker.CancelAsync();

//            OpenFileDialog dialog = new OpenFileDialog();

//            dialog.FileName = "Log.txt";
//            dialog.DefaultExt = ".txt";
//            dialog.Filter = "Log File(*.txt)|*.txt";

//            bool? result = dialog.ShowDialog();

//            if (result == true && logFile != dialog.FileName)
//            {
//                logFile = dialog.FileName;

//                Meter = new DamageMeter(logFile);

//                StartBackgroundWorker();
//            }
//        }

//        public ICommand TerminateCommand => new DelegateCommand(Terminate);
//        public void Terminate()
//        {
//            Application.Current.Shutdown();
//        }

//        public MainWindowViewModel()
//        {
//            IEnumerable<string> localAll = Process.GetProcessesByName("AnarchyOnline").Select(p => p.MainWindowTitle);

//            LiveCollection = (ICollectionViewLiveShaping)CollectionViewSource.GetDefaultView(DamageDoneRows);
//            LiveCollection.IsLiveSorting = true;

//            MainWindow.View.DamageMeter.Items.SortDescriptions.Add(new SortDescription("DamageDone", ListSortDirection.Descending));
//        }

//        void StartBackgroundWorker()
//        {
//            DamageDoneRows.RemoveAll();

//            worker = new BackgroundWorker();

//            worker.DoWork += new DoWorkEventHandler(DoWork);
//            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
//            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
//            worker.WorkerReportsProgress = true;
//            worker.WorkerSupportsCancellation = true;

//            worker.RunWorkerAsync();
//        }

//        public void ProgressChanged(object sender, ProgressChangedEventArgs e)
//        {
//            foreach (var character in (List<Character>)e.UserState)
//            {
//                if (DamageDoneRows.Any(c => c.Name == character.Name))
//                {
//                    DamageDoneRows.Single(c => c.Name == character.Name).Update(character);
//                }
//                else if (character.DamageDone != 0)
//                {
//                    DamageDoneRows.Add(DamageDoneRow.Create(character));
//                }
//            }
//        }

//        public void DoWork(object sender, DoWorkEventArgs e)
//        {
//            while (true)
//            {
//                if (worker.CancellationPending)
//                {
//                    e.Cancel = true;
//                    Meter = new DamageMeter(logFile);
//                    return;
//                }
//                Meter.Update();
//                worker.ReportProgress(0, Meter.CurrentFight.CharactersList);
//                Thread.Sleep(300);
//            }
//        }

//        public void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            if (e.Cancelled)
//            {
//                StartBackgroundWorker();
//            }
//        }
//    }
//}