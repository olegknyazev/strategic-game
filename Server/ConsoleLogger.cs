using StrategicGame.Common;
using System;

namespace StrategicGame.Server {
    class ConsoleLogger : ILogger {
        ConsoleLogger() { }

        public static readonly ConsoleLogger Instance = new ConsoleLogger();

        public void Log(string message, params object[] args) {
            Console.WriteLine(message, args);
        }
    }
}
