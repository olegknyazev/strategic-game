namespace StrategicGame.Common {
    public struct UnitId {
        internal readonly uint value;
        
        static uint _s_id;
        
        public static UnitId New() {
            return new UnitId(++_s_id);
        }

        internal UnitId(uint value) { this.value = value; }

        public override string ToString() { return "#" + value; }
    }
}
