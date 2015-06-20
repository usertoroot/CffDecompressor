/*   This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>. */

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using zlib;

namespace CffDecompressor
{
    public class CffFile
    {
        private static byte[] Signature = new byte[] { 0x12, 0xDD, 0x72, 0xDD };

        public Dictionary<int, byte[]> Files;

        public byte[] this[int key]
        {
            get
            {
                byte[] value;
                if (Files.TryGetValue(key, out value))
                    return value;
                else
                    return null;
            }
        }

        public CffFile()
        {

        }

        public bool Load(byte[] files)
        {
            for (int x = 0; x < Signature.Length; x++)
            {
                if (files[x] != Signature[x])
                    return false;
            }

            int compressedSize;
            int blockCount = 0;
            int offset = 20;
            while (offset + 4 < files.Length)
            {
                blockCount++;
                compressedSize = BitConverter.ToInt32(files, offset + 6);
                offset += 16 + compressedSize;
            }

            int id;
            int decompressedSize;
            byte[] data;

            offset = 20;
            Files = new Dictionary<int, byte[]>(blockCount);
            for (int x = 0; x < blockCount; x++)
            {
                id = BitConverter.ToInt32(files, offset);
                offset += 6;

                compressedSize = BitConverter.ToInt32(files, offset);
                offset += 6;

                decompressedSize = BitConverter.ToInt32(files, offset);
                offset += 4;

                data = new byte[decompressedSize];
                using (MemoryStream stream = new MemoryStream(files, offset, compressedSize))
                {
                    using (ZInputStream inputStream = new ZInputStream(stream))
                    {
                        int size = 0;
                        while (size < decompressedSize)
                            size += inputStream.read(data, size, decompressedSize - size);
                    }
                }

                offset += compressedSize;
                Files.Add(id, data);
            }

            return true;
        }

        public bool Load(string fileName)
        {
            if (!File.Exists(fileName))
                return false;

            return Load(File.ReadAllBytes(fileName));
        }
    }
}
