using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Octgn.Play
{
    using Octgn.Core;
    using Octgn.PlayTable;

    public sealed class Counter : INotifyPropertyChanged, IPlayCounter
    {
        #region Private fields

        private readonly DataNew.Entities.Counter _defintion;
        private readonly byte _id;
        private readonly IPlayPlayer _player; // Player who owns this counter, if any        
        private int _state; // Value of this counter

        #endregion

        #region Public interface

        // Find a counter given its Id

        // Name of this counter
        private readonly string _name;

        public Counter(IPlayPlayer player, DataNew.Entities.Counter def)
        {
            _player = player;
            _state = def.Start;
            _name = def.Name;
            _id = def.Id;
            _defintion = def;
        }

        public string Name
        {
            get { return _name; }
        }

        // Get or set the counter's value
        public int Value
        {
            get { return _state; }
            set { SetValue(value, Program.Player.LocalPlayer, true); }
        }

        public DataNew.Entities.Counter Definition
        {
            get { return _defintion; }
        }

        // C'tor

        public override string ToString()
        {
            return (_player != null ? _player.Name + "'s " : "Global ") + Name;
        }

        #endregion

        #region Implementation

        // Get the id of this counter
        public int Id
        {
            get { return 0x02000000 | (_player == null ? 0 : _player.Id << 16) | _id; }
        }

        // Set the counter's value
        public void SetValue(int value, IPlayPlayer who, bool notifyServer)
        {
            // Check the difference with current value
            int delta = value - _state;
            if (delta == 0) return;
            // Notify the server if needed
            if (notifyServer)
                Program.Client.Rpc.CounterReq(this, value);
            // Set the new value
            _state = value;
            OnPropertyChanged("Value");
            // Display a notification in the chat
            string deltaString = (delta > 0 ? "+" : "") + delta.ToString(CultureInfo.InvariantCulture);
            K.C.Get<GameplayTrace>().TraceEvent(TraceEventType.Information, EventIds.Event | EventIds.PlayerFlag(who),
                                     "{0} sets {1} counter to {2} ({3})", who, this, value, deltaString);
        }

        public void Reset()
        {
            if (!Definition.Reset) return;
            _state = Definition.Start;
            OnPropertyChanged("Value");
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}