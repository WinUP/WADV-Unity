using System;
using System.IO;
using System.Text;
using Core.VisualNovel.Compiler;

namespace Core.VisualNovel.Interoperation {
    public class LiteMemorySteamWriter {
        private MemoryStream Stream { get; }
        private ExtendedBinaryWriter Writer { get; }

        public LiteMemorySteamWriter() {
            Stream = new MemoryStream();
            Writer = new ExtendedBinaryWriter(Stream, Encoding.UTF8);
        }

        public LiteMemorySteamWriter Write(bool value) {
            Writer.Write(value);
            return this;
        }

        public LiteMemorySteamWriter Write(byte value) {
            Writer.Write(value);
            return this;
        }

        public LiteMemorySteamWriter Write(byte[] buffer) {
            Writer.Write(buffer);
            return this;
        }

        public LiteMemorySteamWriter Write(byte[] buffer, int index, int count) {
            Writer.Write(buffer, index, count);
            return this;
        }

        public LiteMemorySteamWriter Write(char ch) {
            Writer.Write(ch);
            return this;
        }

        public LiteMemorySteamWriter Write(char[] chars) {
            Writer.Write(chars);
            return this;
        }

        public LiteMemorySteamWriter Write(char[] chars, int index, int count) {
            Writer.Write(chars, index, count);
            return this;
        }

        public LiteMemorySteamWriter Write(decimal value) {
            Writer.Write(value);
            return this;
        }

        public LiteMemorySteamWriter Write(double value) {
            Writer.Write(value);
            return this;
        }

        public LiteMemorySteamWriter Write(short value) {
            Writer.Write(value);
            return this;
        }

        public LiteMemorySteamWriter Write(int value) {
            Writer.Write(value);
            return this;
        }

        public LiteMemorySteamWriter Write(long value) {
            Writer.Write(value);
            return this;
        }

        [CLSCompliant(false)]
        public LiteMemorySteamWriter Write(sbyte value) {
            Writer.Write(value);
            return this;
        }

        public LiteMemorySteamWriter Write(float value) {
            Writer.Write(value);
            return this;
        }

        public LiteMemorySteamWriter Write(string value) {
            Writer.Write(value);
            return this;
        }

        [CLSCompliant(false)]
        public LiteMemorySteamWriter Write(ushort value) {
            Writer.Write(value);
            return this;
        }

        [CLSCompliant(false)]
        public LiteMemorySteamWriter Write(uint value) {
            Writer.Write(value);
            return this;
        }

        [CLSCompliant(false)]
        public LiteMemorySteamWriter Write(ulong value) {
            Writer.Write(value);
            return this;
        }

        public byte[] ToArray() {
            return Stream.ToArray();
        }
    }
}