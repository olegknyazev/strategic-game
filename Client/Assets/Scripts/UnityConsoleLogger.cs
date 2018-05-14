using UnityEngine;

namespace StrategicGame.Client {
    class UnityConsoleLogger : Common.ILogger {
        UnityConsoleLogger() { }

        public static readonly UnityConsoleLogger Instance = new UnityConsoleLogger();

        public void Log(string message, params object[] args) {
            Debug.LogFormat(message, args);
        }
    }
}
