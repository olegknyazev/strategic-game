using System.IO;

namespace StrategicGame.Common {
    public abstract class Message {
        public abstract void Serialize(BinaryWriter writer);
    }

    public abstract class Command : Message {
        public static Command Deserialize(BinaryReader reader) {
            var type = reader.ReadByte();
            switch (type) {
                default:
                    throw new InvalidDataException("Unknown Command type " + type);
            }
        }
    }

    public abstract class Status : Message {
        public static Status Deserialize(BinaryReader reader) {
            var type = reader.ReadByte();
            switch (type) {
                case WorldParameters.CODE: return new WorldParameters(reader);
                case UnitPosition.CODE: return new UnitPosition(reader);
                default: throw new InvalidDataException("Unknown Status type " + type);
            }
        }
    }

    public sealed class WorldParameters : Status {
        internal const byte CODE = 1;

        public readonly short Width;
        public readonly short Height;

        public WorldParameters(short width, short height) {
            Width = width;
            Height = height;
        }
        
        internal WorldParameters(BinaryReader reader) {
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
        }

        public override void Serialize(BinaryWriter writer) {
            writer.Write(Width);
            writer.Write(Height);
        }

        public override string ToString() {
            return string.Format("[WorldParameters {0} x {1}]", Width, Height);
        }
    }

    public sealed class UnitPosition : Status {
        internal const byte CODE = 2;

        public readonly uint Id;
        public readonly short X;
        public readonly short Y;
        public readonly uint Frame;
        
        public UnitPosition(uint id, short x, short y, uint frame) {
            Id = id;
            X = x;
            Y = y;
            Frame = frame;
        }
        
        internal UnitPosition(BinaryReader reader) {
            Id = reader.ReadUInt32();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Frame = reader.ReadUInt32();
        }
        
        public override void Serialize(BinaryWriter writer) {
            writer.Write(Id);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Frame);
        }

        public override string ToString() {
            return string.Format("[UnitPosition {0} ({1}, {2}) @ {3}]", Id, X, Y, Frame);
        }
    }
}
