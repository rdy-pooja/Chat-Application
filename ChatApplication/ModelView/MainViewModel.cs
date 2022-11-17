using ChatClient.ModelView.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChatClient.ModelView.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        private Server _server;
        public string Username { get; set; }
        public string Message { get; set; }
        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _server = new Server();
            _server.connectEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnected += RemoveUser;
            
            ConnectToServerCommand = new RelayCommand(x => _server.ConnectToServer(Username), x => !string.IsNullOrEmpty(Username));
            
            SendMessageCommand = new RelayCommand(x => _server.SendMessageToServer(Message), x => !string.IsNullOrEmpty(Message));
        }

        private void MessageReceived()
        {
            var msg = _server.packetReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void UserConnected()
        {
            var user = new UserModel()
            {
                Username = _server.packetReader.ReadMessage(),
                UID = _server.packetReader.ReadMessage(),

            };

            if(!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
        private void RemoveUser()
        {
            var uid = _server.packetReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }
    }
}
