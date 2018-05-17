using System;

namespace StrategicGame.Server {
    static class Program {
        static void Main(string[] args) {
            using (var server = new Server(log: Console.WriteLine))
                server.Execute();
        }
    }
}
