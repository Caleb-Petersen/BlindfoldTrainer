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

namespace BlindfoldTrainer
{
    using ilf.pgn;
    using ilf.pgn.Data;
    using Microsoft.Win32;
    using System.IO;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private BlindfoldTrainerVM _trainingVM = new BlindfoldTrainerVM();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _trainingVM;
            DG1.DataContext = _trainingVM.Games;
        }


        private void AddPgn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "Browse PGN Files";
            openFileDialog.DefaultExt = "pgn";
            openFileDialog.Filter = "pgn files (*.pgn)|*.pgn|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            var result = openFileDialog.ShowDialog();

            if(result.HasValue && result.Value)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    bool addResult = _trainingVM.AddPgn(file, "", out string msg);

                    if (!addResult)
                    {
                        string errorMsg = "Error loading " + file + ". " + msg;
                        MessageBox.Show(errorMsg, "Load Pgn Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void StopSimul_Click(object sender, RoutedEventArgs e)
        {
            _trainingVM.StopSimul();

            MessageBox.Show("Successfully Stopped Simul", "SimulMessage", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StartSimul_Click(object sender, RoutedEventArgs e)
        {
            _trainingVM.StartSimul();
        }

        private void AcknowledgeMove_Click(object sender, RoutedEventArgs e)
        {
            AcknowledgeSimulMove();
        }

        private void MainWindow_KeyPressed(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                AcknowledgeSimulMove();
            }
            e.Handled = true;
        }

        private void AcknowledgeSimulMove()
        {
            _trainingVM.AcknowledgeSimulMove();
        }
    }
}
