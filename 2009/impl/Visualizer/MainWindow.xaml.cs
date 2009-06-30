﻿using System;
using System.IO;
using System.Threading;
using System.Windows;
using ICFP2009.VirtualMachineLib;

namespace ICFP2009.Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LoadButton_Click(this, null);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            using (var stream = new FileStream(_pathTextBox.Text, FileMode.Open, FileAccess.Read))
                VirtualMachine.Instance.LoadBinary(stream);
        }

        private void _nextButton_Click(object sender, RoutedEventArgs e)
        {
            SetUpInputPorts();
            VirtualMachine.Instance.RunOneStep();
            UpdateOutputPorts();
            _mainCanvas.StepCompleted();
            _mainCanvas.InvalidateVisual();
        }

        private void _next1000Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 1000; ++i)
            {
                SetUpInputPorts();
                VirtualMachine.Instance.RunOneStep();
                UpdateOutputPorts();
                _mainCanvas.StepCompleted();
                _mainCanvas.InvalidateVisual();
                InvalidateVisual();
                Thread.Sleep(10);
            }
        }

        private void UpdateOutputPorts()
        {
            _portsListBox.Items.Clear();
            foreach (var pair in VirtualMachine.Instance.Ports.Output)
                _portsListBox.Items.Add(string.Format("0x{0:x4}:{1:g}", pair.Key, pair.Value));
        }

        private void SetUpInputPorts()
        {
            VirtualMachine.Instance.Ports.Input[0x3e80] = Int16.Parse(_problemNumberTextBox.Text);
            VirtualMachine.Instance.Ports.Input[0x0002] = Int16.Parse(_dVxTextBox.Text);
            VirtualMachine.Instance.Ports.Input[0x0003] = Int16.Parse(_dVyTextBox.Text);
        }

    }
}