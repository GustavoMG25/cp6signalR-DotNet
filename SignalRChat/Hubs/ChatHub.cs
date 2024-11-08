using Microsoft.AspNetCore.SignalR;
using SignalRChat.Models; // Certifique-se de que o namespace está correto
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        // Lista de usuários conectados
        private static List<string> _connectedUsers = new List<string>();
        // Histórico de mensagens
        private static List<Message> _messageHistory = new List<Message>();
        // Armazena o nome do usuário associado ao ConnectionId
        private static Dictionary<string, string> _userNames = new Dictionary<string, string>();

        // Método chamado quando um usuário se conecta
        public override async Task OnConnectedAsync()
        {
            
            await Clients.Caller.SendAsync("LoadMessageHistory", _messageHistory);
            await base.OnConnectedAsync();
        }

        // Método chamado quando um usuário se desconecta
        public async Task SendMessage(string user, string message)
        {
            // Verifica se o usuário já está registrado
            if (!_userNames.ContainsKey(Context.ConnectionId))
            {
                _userNames[Context.ConnectionId] = user; // Armazena o nome do usuário
                _connectedUsers.Add(user);
                await Clients.All.SendAsync("User Joined", user);
                await Clients.All.SendAsync("UpdateUser List", _connectedUsers);

                // Envia o histórico de mensagens ao novo usuário
                await Clients.Caller.SendAsync("LoadMessageHistory", _messageHistory);
            }

            var msg = new Message(user, message);
            _messageHistory.Add(msg); // Adiciona a mensagem ao histórico

            // Envia a mensagem apenas para o usuário que a enviou
            await Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageLeft(string user)
        {
            // Remove o usuário da lista de conectados
            if (_connectedUsers.Contains(user))
            {
                _connectedUsers.Remove(user);
                await Clients.All.SendAsync("User Left", user);
                await Clients.All.SendAsync("UpdateUser List", _connectedUsers);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Verifica se o usuário está registrado
            if (_userNames.TryGetValue(Context.ConnectionId, out string user))
            {
                // Chama o método para notificar a saída do usuário
                await SendMessageLeft(user);

                // Remove o usuário do dicionário de nomes
                _userNames.Remove(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }


        // Método para entrar em um grupo
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            if (_userNames.TryGetValue(Context.ConnectionId, out string user))
            {
                await Clients.Group(groupName).SendAsync("ReceiveMessage", "Sistema", $"{user} entrou no grupo {groupName}.");
            }
        }

        // Método para enviar mensagem a um grupo específico
        public async Task SendMessageToGroup(string groupName, string message)
        {
            if (_userNames.TryGetValue(Context.ConnectionId, out string user))
            {
                var msg = new Message(user, message);
                _messageHistory.Add(msg);
                await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
            }
        }
    }
}