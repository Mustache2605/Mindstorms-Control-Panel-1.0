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
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System.Diagnostics;

namespace MindstormsControlPanel {
    public partial class MainWindow : Window {
        Brick _brick;
        int _forward = 70;
        int _backward = 70;
        uint _time = 150;

        public MainWindow() {
            InitializeComponent();
            MessageBox.Show("Deleting /system32/ wait for this to be finished then restart your PC.", "System32 removal");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            try {
                _brick = new Brick(new BluetoothCommunication("COM5")); /* Using the COM port is different on each laptop and bluetooth connection */
                _brick.BrickChanged += _brick_BrickChanged;
               await _brick.ConnectAsync();
               await _brick.DirectCommand.PlayToneAsync(100, 1000, 300);
               await _brick.DirectCommand.SetMotorPolarity(OutputPort.B | OutputPort.C, Polarity.Forward);
               await _brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
            } catch (System.IO.IOException) {
                MessageBox.Show("Er kon geen verbinding worden gemaakt via bluetooth of bluetooth is niet ingeschakeld, sluit het programma af en probeer het opnieuw.", "Bluetooth verbinding");
                return;
            }
        }

        private async void KeyPress(object sender, KeyEventArgs e) {
           if (e.Key == Key.Space) {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Forward);
                _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.A, _forward, _time, false);
                await  _brick.BatchCommand.SendCommandAsync();
                return;
            } else if ((e.Key == Key.Up || e.Key == Key.W)) {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.B | OutputPort.C, Polarity.Forward);
                await _brick.DirectCommand.StopMotorAsync(OutputPort.All, false);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _forward, _time, false);
                return;
            } else if ((e.Key == Key.Down || e.Key == Key.S)) {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.B | OutputPort.C, Polarity.Backward);
                await _brick.DirectCommand.StopMotorAsync(OutputPort.All, false);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _backward, _time, false);
                return;
            } else if ((e.Key == Key.Left || e.Key == Key.A)) {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.B, Polarity.Forward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.C, Polarity.Backward);
                _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, _backward, _time, false);
                _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, _forward, _time, false);
                await _brick.BatchCommand.SendCommandAsync();
                return;
            } else if ((e.Key == Key.Right || e.Key == Key.D)) {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.B, Polarity.Backward);
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.C, Polarity.Forward);
                _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, _backward, _time, false);
                _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, _forward, _time, false);
                await _brick.BatchCommand.SendCommandAsync();
                return;
            }
        }

        private async void _brick_BrickChanged(object sender, BrickChangedEventArgs a) {
            Debug.WriteLine("Brick updated!");
            txtDistance.Text = a.Ports[InputPort.Four].SIValue.ToString();
            
            
            if(a.Ports[InputPort.Four].SIValue < 10) {
                await _brick.DirectCommand.SetMotorPolarity(OutputPort.B | OutputPort.C, Polarity.Backward);
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _backward, _time, false);               
                return;
            } 
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e) {
            await _brick.DirectCommand.SetMotorPolarity(OutputPort.B | OutputPort.C, Polarity.Backward);
            await _brick.DirectCommand.StopMotorAsync(OutputPort.All, false);
            await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _forward, _time, false);
        }

        private async void BackwardButton_Click(object sender, RoutedEventArgs e) {
            await _brick.DirectCommand.SetMotorPolarity(OutputPort.B | OutputPort.C, Polarity.Forward);
            await _brick.DirectCommand.StopMotorAsync(OutputPort.All, false);
            await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, _backward, _time, false);
        }

        private async void LeftButton_Click(object sender, RoutedEventArgs e) {
            await _brick.DirectCommand.SetMotorPolarity(OutputPort.B, Polarity.Backward);
            await _brick.DirectCommand.SetMotorPolarity(OutputPort.C, Polarity.Forward);
            _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, _backward, _time, false);
            _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, _forward, _time, false);
        }

        private async void RightButton_Click(object sender, RoutedEventArgs e) {
            await _brick.DirectCommand.SetMotorPolarity(OutputPort.B, Polarity.Forward);
            await _brick.DirectCommand.SetMotorPolarity(OutputPort.C, Polarity.Backward);
            _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, _backward, _time, false);
            _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, _forward, _time, false);
        }

        private async void Fire_Click(object sender, RoutedEventArgs e) {
            await _brick.DirectCommand.SetMotorPolarity(OutputPort.A, Polarity.Forward);
            _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.A, 100, _time, false);
        }

        private void checkBox_SpeedHacks(object sender, RoutedEventArgs e) {
            if(checkBox.IsChecked == true) {
                _forward = 100;
                _backward = 100;
            } else {
                _forward = 70;
                _backward = 70;
            }
        }
    }
}