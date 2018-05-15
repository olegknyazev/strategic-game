using System;

namespace StrategicGame.Client {
    public static class Utils {
        public static void InvokeSafe(this Action ev) { if (ev != null) ev(); }
        public static void InvokeSafe<T0>(this Action<T0> ev, T0 a0) { if (ev != null) ev(a0); }
        public static void InvokeSafe<T0, T1>(this Action<T0, T1> ev, T0 a0, T1 a1) { if (ev != null) ev(a0, a1); }
    }
}
