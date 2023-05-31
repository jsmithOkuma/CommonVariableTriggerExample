using System;
using System.Threading;
using Okuma.CLDATAPI.DataAPI;

namespace CVTriggerExample
{
    class Monitor
    {
        // Event that gets triggered when the value changes.
        public event EventHandler<double> OnValueChanged;

        private Double _value;
        private Timer _timer;
        private CancellationToken _token;
        private CVariables _cVariables;

        // Property that holds the current value.
        // If a new value is set that is different from the old value, the OnValueChanged event is triggered.
        public double Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnValueChanged?.Invoke(this, _value);
                }
            }
        }

        //Constructor to initialize value, token, CVariables, and timer
        public Monitor(double initialValue, CancellationToken token, CVariables cVariables)
        {
            _cVariables = cVariables;
            _value = initialValue;
            _token = token;
            //Initialioze timer. Sets timer due time to infinity so it doesn't immediately start.
            _timer = new Timer(CheckValueChanged, null, Timeout.Infinite, 1000);
        }

        // Method to start monitoring the value.
        // It changes the timer due time to 0, so the timer starts immediately.
        public void StartMonitoring()
        {
            _timer.Change(0, 1000);
        }

        // Method that is called by the timer every second.
        // It checks if a cancellation is requested, and if so, it disposes the timer.
        // Otherwise, it fetches the current value of common variable 100 and updates the Value property.
        private void CheckValueChanged(object state)
        {
            if (_token.IsCancellationRequested)
            {
                _timer.Dispose();
                return;
            }

            //Monitors common variable 100, can make this user defined using a text box
            double newValue = _cVariables.GetCommonVariableValue(100);
            Value = newValue;
        }


    }
}
