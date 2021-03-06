﻿using System;
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
                case MessageType.SendUnits: return new SendUnits(reader);
                default: throw new Exception("Unknown Command type " + type);
            }
        }
    }

    static class MessageType {
        public const byte SendUnits = 1;
        public const byte WorldParameters = 2;
        public const byte UnitPosition = 3;
    }

    public sealed class SendUnits : Command {
        List<UnitId> _units;

        public readonly Int2 Destination;
        public IEnumerable<UnitId> Units { get { return _units; } }

        public SendUnits(IEnumerable<UnitId> units, Int2 destination) {
            _units = new List<UnitId>(units);
            Destination = destination;
        }

        internal SendUnits(BinaryReader reader) {
            int count = reader.ReadUInt16();
            _units = new List<UnitId>(count);
            for (int i = 0; i < count; ++i)
                _units.Add(new UnitId(reader.ReadUInt32()));
            Destination = new Int2(reader.ReadInt16(), reader.ReadInt16());
        }

        public override void Serialize(BinaryWriter writer) {
            writer.Write(MessageType.SendUnits);
            writer.Write((ushort)_units.Count);
            foreach (var unitId in _units)
                writer.Write(unitId.value);
            writer.Write((short)Destination.X);
            writer.Write((short)Destination.Y);
        }

        public override string ToString() {
            return string.Format("[SendUnits [{0}] -> {1}]",
                string.Join(", ", _units.Select(x => x.ToString()).ToArray()),
                Destination);
        }
    }

    public abstract class StatePortion : Message {
        public static StatePortion Deserialize(BinaryReader reader) {
            var type = reader.ReadByte();
            switch (type) {
                case MessageType.WorldParameters: return new WorldParameters(reader);
                case MessageType.UnitPosition: return new UnitPosition(reader);
                default: throw new Exception("Unknown Status type " + type);
            }
        }
    }

    public sealed class WorldParameters : StatePortion {
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

    public sealed class UnitPosition : StatePortion {
        public readonly UnitId Id;
        public readonly float X;
        public readonly float Y;
        public readonly uint Frame;
        public readonly bool Moving;
        
        public UnitPosition(UnitId id, float x, float y, uint frame, bool moving) {
            Id = id;
            X = x;
            Y = y;
            Frame = frame;
            Moving = moving;
        }
        
        internal UnitPosition(BinaryReader reader) {
            Id = new UnitId(reader.ReadUInt32());
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Frame = reader.ReadUInt32();
            Moving = reader.ReadBoolean();
        }
        
        public override void Serialize(BinaryWriter writer) {
            writer.Write(MessageType.UnitPosition);
            writer.Write(Id.value);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Frame);
            writer.Write(Moving);
        }

        public override string ToString() {
            return string.Format(
                "[UnitPosition {0} ({1}, {2}) @ {3}{4}]",
                Id, X, Y, Frame,
                Moving ? " moving" : "");
        }
    }
}
