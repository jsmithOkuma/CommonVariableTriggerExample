using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Okuma.CLDATAPI.DataAPI;


/**
 * This application is for a Lathe with API version 1.23.0
 */
namespace CVTriggerExample
{
    public partial class Form1 : Form
    {
        private CVariables cVariables;

        // The CancellationTokenSource is used to signal when the monitoring should stop.
        private CancellationTokenSource _cts;
        // The Monitor class is responsible for monitoring the value and triggering an event when it changes.
        private Monitor _monitor;

        public Form1()
        {
            InitializeComponent();

            try
            {
                (new CMachine()).Init();
                cVariables = new CVariables();


            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // Initializes the CancellationTokenSource and the EventPublisher, and starts the monitoring on form load.
        private void Form1_Load(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            // Initialize the Monitor with an initial value of 0.0 and the cancellation token,
            // and subscribe to the OnValueChanged event.
            _monitor = new Monitor(0.0, token, cVariables);
            _monitor.OnValueChanged += HandleValueChanged;

            // Start monitoring the value on a separate thread.
            Task.Run(() => _monitor.StartMonitoring(), token);
        }

        // Signals the cancellation token, which will cause the monitoring to stop.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cts.Cancel();

            // If you do not call Application.Exit, the process may continue to run in the background.
            Application.Exit();
        }

        // This method handles the OnValueChanged event of the Monitor.
        // It adds a log message.
        // This is where you will do whatever work you want to do the the common variable is triggered
        private void HandleValueChanged(object sender, double newValue)
        {
            //In this case if CV100 = 1, the method executes, it then sets CV100 back to 0
            //This function will execute every time the NC program chances CV100 to 1.
            if (newValue == 1)
            {
                AddLogMessage("Event Trigger");
                cVariables.SetCommonVariableValue(100, 0);
            }
           
        }

        public void AddLogMessage(string message)
        {
            lstLog.Items.Add($"{DateTime.Now.ToString("hh:mm:ss tt")} - {message}");
            lstLog.SelectedIndex = lstLog.Items.Count - 1;
            lstLog.ClearSelected();
        }
    }
}
