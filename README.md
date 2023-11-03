# ASP Websockets example

A simple "Ping / Pong" server and client using ASP Websockets.

## Server

The server uses an ASP MVC Controller to listen for websocket connections at the address ws://localhost:5001/ws.

[You can find the controller code here.](Server/Controllers/WebsocketController.cs)

## Client

The client is simplistic, in a loop it reads the standard input, sends the input to the server and logs the response

All the client code can be found in it's [Program.cs](Client/Program.cs)

## Running the example

From the root of the repository, build the solution:
```
dotnet build
```

In one terminal, run the server
```
dotnet run --project Server
```

In a second terminal, run the client:
```
dotnet run --project Client
```

If both projects are running correctly you should see the following:

```
dotnet run --project Client
Connection successful, waiting for input.
--> 
```

You can now type any message and hit enter to send the message to the server.