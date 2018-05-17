# Strategic Game
A prototype of a client-server strategic game. The client is implemented in Unity 5.6, the server—in pure .NET 3.5. It's just a tiny toy project :-)

On startup, the server generates a small world of a random size and populates it by some units. The only action that user can do with this world is to select some units and send them. All clients have equal 'rights' in control so there is no 'my' or 'your' units.

## Running
1. Get a build from here.
2. Run ClientGame.Server.exe and any number of Client.exe on a single computer (in any order).

Both the server and the client are capable to reconnect, so you can close and restart them as you wish.

## Building
1. Open ServerAndCommon.sln from the root folder in Visual Studio.
2. Build the solution. This will not only build the server but also copy the shared DLL to the client.
3. Open the Client project in Unity.
4. Build the project using standard Unity tools (File / Build Settings).
