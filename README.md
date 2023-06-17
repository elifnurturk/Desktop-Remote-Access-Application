# Desktop-Remote-Application
 Desktop Remote Application prepared by C# to provide the connection between Desktop Devices.

The project is a remote access application written in C#, it consists of 2 parts. Server and Client parts. Its purpose is to execute commands and exchange data remotely by communicating between the server and the client.

They were designed using Windows forms. The client establishes a TCP connection with the server and provides the user interface. So the user can send commands to the server via the text box. The client displays the messages from the server on the screen, can transmit commands to the server such as sending a message, beeping, playing sound, and shutting down the server.

When the server is running, it will not appear in the task manager, it is designed to be completely hidden. The server must run before the client or data cannot be provided to the client. Commands from the client are defined using enumeration.

*Multi-threading* is used in the project. Incoming commands are processed with the switch-case structure. Each command creates a thread that calls the corresponding function. We have three separate operations: message, beep and playsound. Each handler runs on a separate thread to perform the corresponding action when a command arrives. With the message command, the messagebox is displayed, and the beep and playsound play sound.
The server is shut down with the exit command, and the cleaning process is performed with cleanup.

On the client side; The connection is managed, the data stream is processed and commands are sent to the server. A thread is started with the application and TcpListener starts listening. When a connection request is received, a new thread is started and the RunClient method is called. The startListen method starts the TcpListener and accepts incoming connections. Data from the server is read and displayed on the screen. Likewise, when the application is closed, cleaning is performed with cleanup.

![image](https://github.com/elifistanya/Desktop-Remote-Access-Application/assets/69623465/a25758a6-d821-45ce-a5c1-5943c36b001e)
![image](https://github.com/elifistanya/Desktop-Remote-Access-Application/assets/69623465/5da03e9d-aa0a-4c17-997f-df416f8aa6ca)
![image](https://github.com/elifistanya/Desktop-Remote-Access-Application/assets/69623465/8ba5a091-0c2b-4072-94c6-2e2f20b77b67)
