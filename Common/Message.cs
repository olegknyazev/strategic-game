using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StrategicGame.Common {
    public abstract class Message {
        public abstract void Serialize(BinaryWriter writer);
    }

    public abstract class Command : Message {
        public static Command Deserialize(BinaryReader reader) {
            var type = reader.ReadByte();
            switch (type) {
                case MessageType.MoveOrder: return new MoveOrder(reader);
                default: throw new InvalidDataException("Unknown Command type " + type);
            }
        }
    }

    static class MessageType {
        public const byte MoveOrder = 1;
        public const byte WorldParameters = 2;
        public const byte UnitPosition = 3;
    }

    public sealed class MoveOrder : Command {
        List<uint> _units;

        public readonly Int2 Destination;
        public IEnumerable<uint> Units { get { return _units; } }

        public MoveOrder(IEnumerable<uint> units, Int2 destination) {
            _units = new List<uint>(units);
            Destination = destination;
        }

        internal MoveOrder(BinaryReader reader) {
            int count = reader.ReadUInt16();
            _units = new List<uint>(count);
            for (int i = 0; i < count; ++i)
                _units.Add(reader.ReadUInt32());
            Destination = new Int2(reader.ReadInt16(), reader.ReadInt16());
        }

        public override void Serialize(BinaryWriter writer) {
            writer.Write(MessageType.MoveOrder);
            writer.Write((ushort)_units.Count);
            foreach (var unitId in _units)
                writer.Write(unitId);
            writer.Write((short)Destination.X);
            writer.Write((short)Destination.Y);
        }

        public override string ToString() {
            return string.Format("[MoveOrder ({0}) -> {1}]",
                string.Join(", ", _units.Select(x => x.ToString()).ToArray()),
                Destination);
        }
    }

    public abstract class Status : Message {
        public static Status Deserialize(BinaryReader reader) {
            var type = reader.ReadByte();
            switch (type) {
                case MessageType.WorldParameters: return new WorldParameters(reader);
                case MessageType.UnitPosition: return new UnitPosition(reader);
                default: throw new InvalidDataException("Unknown Status type " + type);
            }
        }
    }

    public sealed class WorldParameters : Status {
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
            writer.Write(MessageType.WorldParameters);
            writer.Write(Width);
            writer.Write(Height);
        }

        public override string ToString() {
            return string.Format("[WorldParameters {0} x {1}]", Width, Height);
        }
    }

    public sealed class UnitPosition : Status {
        public readonly uint Id;
        public readonly float X;
        public readonly float Y;
        public readonly uint Frame;
        
        public UnitPosition(uint id, float x, float y, uint frame) {
            Id = id;
            X = x;
            Y = y;
            Frame = frame;
        }
        
        internal UnitPosition(BinaryReader reader) {
            Id = reader.ReadUInt32();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Frame = reader.ReadUInt32();
        }
        
        public override void Serialize(BinaryWriter writer) {
            writer.Write(MessageType.UnitPosition);
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
