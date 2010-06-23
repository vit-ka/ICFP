using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
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
        private RnaRunner.RnaRunner _rnaRunner;

        /// <summary>
        /// Main windows of application.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RunDnaButtonClick(object sender, RoutedEventArgs e)
        {
            _dnaRunner.Prefix = _prefixTextBox.Text;
            _dnaRunner.Start();
        }


        private void RunRnaButtonClick(object sender, RoutedEventArgs e)
        {
            _rnaRunner.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dnaRunner =
                new DnaRunner.DnaRunner(
                    new FileStream(
                        Settings.Default.PathToEndoDNAFile.Trim(), FileMode.Open, FileAccess.Read, FileShare.None),
                    new FileStream(
                        "D:\\OutRNA.txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite));

            _dnaRunner.SomeCharsWrittenToRna += DnaRunnerSomeCharsWrittenToRna;
            _dnaRunner.SomeCommandOfDnaHasBeenProcessed += DnaRunnerSomeCommandOfDnaHasBeenProcessed;
            _dnaRunner.DnaProcessingFinished += DnaRunnerDnaProcessingFinished;

            _rnaRunner = new RnaRunner.RnaRunner(new FileStream(
                        @"D:\Projects\ICFP\2007\source\Rna\RNA_Withoutprefix.txt", FileMode.Open, FileAccess.Read, FileShare.None));
            _rnaRunner.SomeDrawCommandsExecuted += RnaRunnerSomeDrawCommandsExecuted;
            _rnaRunner.ExecutionFinished += RnaRunnerExecutionFinished;
        }

        void RnaRunnerExecutionFinished(object sender, EventArgs e)
        {
            DrawBitmap();
        }

        private void DrawBitmap()
        {
            Dispatcher.Invoke(
                new VoidDelegate(
                    () =>
                    {
                        var memoryStream = new MemoryStream();
                        _rnaRunner.Bitmap.Save(memoryStream, ImageFormat.Png);

                        memoryStream.Seek(0, SeekOrigin.Begin);

                        memoryStream.WriteTo(new FileStream("D:\\123.png", FileMode.Create));

                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();

                        _image.Source = bitmapImage;
                    }));
        }

        void RnaRunnerSomeDrawCommandsExecuted(object sender, EventArgs e)
        {
            DrawBitmap();
        }

        private void DnaRunnerDnaProcessingFinished(object sender, EventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        const string message = "Обработка ДНК завершена.";
                        _logListBox.Items.Add(message);
                        _logListBox.ScrollIntoView(message);
                    }));
        }

        private void DnaRunnerSomeCommandOfDnaHasBeenProcessed(object sender,
                                                               SomeCommandOfDnaHasBeenProcessedEventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        string message = string.Format("Обработано {0} команд ДНК.", e.TotalCommandProcessed);
                        _logListBox.Items.Add(message);
                        _logListBox.ScrollIntoView(message);
                    }));
        }

        private void DnaRunnerSomeCharsWrittenToRna(object sender, SomeCharsWrittenToRnaEventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        string message = string.Format("Произведено {0} символов РНК.", e.TotalCharsCount);
                        _logListBox.Items.Add(message);
                        _logListBox.ScrollIntoView(message);
                    }));
        }

    }
}