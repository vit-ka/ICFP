using System.IO;
using System.Windows;
using System.Windows.Threading;
using DnaRunner;
using Visualizer.Properties;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DnaRunner.DnaRunner _dnaRunner;

        /// <summary>
        /// Main windows of application.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RunDnaButtonClick(object sender, RoutedEventArgs e)
        {
            _dnaRunner.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dnaRunner =
                new DnaRunner.DnaRunner(
                    new FileStream(Settings.Default.PathToEndoDNAFile.Trim(), FileMode.Open, FileAccess.Read, FileShare.None),
                    new MemoryStream());

            _dnaRunner.SomeCharsWrittenToRna += DnaRunnerSomeCharsWrittenToRna;
            _dnaRunner.SomeCommandOfDnaHasBeenProcessed += DnaRunnerSomeCommandOfDnaHasBeenProcessed;
        }

        private void DnaRunnerSomeCommandOfDnaHasBeenProcessed(object sender,
                                                               SomeCommandOfDnaHasBeenProcessedEventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        _logListBox.Items.Add(e.TotalCommandProcessed);
                        _logListBox.ScrollIntoView(e.TotalCommandProcessed);
                    }));
        }

        private void DnaRunnerSomeCharsWrittenToRna(object sender, SomeCharsWrittenToRnaEventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        _logListBox.Items.Add(e.TotalCharsCount);
                        _logListBox.ScrollIntoView(e.TotalCharsCount);
                    }));
        }
    }
}