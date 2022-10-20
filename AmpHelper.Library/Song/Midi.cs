﻿using AmpHelper.Library.Enums;
using System;
using System.IO;

namespace AmpHelper.Library.Song
{
    public static class Midi
    {
        private const UInt64 PS3Terminator = 0xcdabcdabcdabcdab;
        private const UInt64 PS4Terminator = 0x01abcdabcdabcdab;

        /// <summary>
        /// Patches mid_ps3 or mid_ps4 files with a new tempo.
        /// 
        /// The input stream will be left open and the position set back to the starting position upon completion.
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="bpm"></param>
        /// <param name="consoleType"></param>
        /// <exception cref="Exception"></exception>
        public static void PatchMidi(Stream input, float bpm, ConsoleType consoleType)
        {
            var streamStart = input.Position;
            int midiBpm = (int)Math.Round(60000000.0f / bpm);

            for (var i = input.Position = input.Length - 1024; i < input.Length - 8; i++)
            {
                input.Position = i;

                if (input.ReadUInt64LE() == (consoleType == ConsoleType.PS3 ? PS3Terminator : PS4Terminator))
                {
                    input.Position += 11;

                    if (consoleType == ConsoleType.PS3)
                    {
                        input.Position += 2;

                        input.Write(new byte[] {
                            (byte)((midiBpm >> 16) & 0xFF),
                            (byte)((midiBpm >> 8) & 0xFF),
                            (byte)((midiBpm >> 0) & 0xFF)
                        });
                    }
                    else
                    {
                        input.Write(new byte[] {
                            (byte)((midiBpm >> 0) & 0xFF),
                            (byte)((midiBpm >> 8) & 0xFF),
                            (byte)((midiBpm >> 16) & 0xFF)
                        });
                    }

                    input.Position = streamStart;

                    return;
                }
            }

            input.Position = streamStart;

            throw new Exception("Unable to patch midi");
        }
    }
}
