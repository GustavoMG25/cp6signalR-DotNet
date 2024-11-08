// Conexão com o SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

// Iniciar a conexão
connection.start().catch(function (err) {
    return console.error(err.toString());
});

// Receber mensagens
connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.textContent = `${user} diz: ${message}`;
    document.getElementById("messagesList").appendChild(li);
});

// Notificar quando um usuário entrar
connection.on("User Joined", function (user) {
    var li = document.createElement("li");
    var li2 = document.createElement("li");
    li.textContent = `${user} entrou no chat.`;
    li.style.fontStyle = "italic"; 
    li2.textContent = `carregando mensagens antigas...`
    li2.style.fontStyle = "italic"; 
    document.getElementById("messagesList").appendChild(li);
    document.getElementById("messagesList").appendChild(li2);

});

// Notificar quando um usuário sair
connection.on("User Left", function (user) {
    var li = document.createElement("li");
    li.textContent = `${user} saiu do chat.`;
    li.style.fontStyle = "italic"; 
    document.getElementById("messagesList").appendChild(li);
});

// Carregar o histórico de mensagens
connection.on("LoadMessageHistory", function (messages) {
    messages.forEach(function (message) {
        var li = document.createElement("li");
        li.textContent = `${message.user} diz: ${message.content}`;
        document.getElementById("messagesList").appendChild(li);
    });
});

// Enviar mensagem para todos
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
});

// Entrar em um grupo
document.getElementById("joinGroupButton").addEventListener("click", function (event) {
    var group = document.getElementById("groupInput").value;
    connection.invoke("JoinGroup", group).catch(function (err) {
        return console.error(err.toString());
    });
});

// Enviar mensagem para um grupo
document.getElementById("sendGroupButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    var group = document.getElementById("groupInput").value;

    connection.invoke("SendMessageToGroup", group, message).catch(function (err) {
        return console.error(err.toString());
    });
});