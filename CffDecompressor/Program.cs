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

﻿using CffDecompressor.BFCardView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CffDecompressor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: CffDecompressor en.cff");
                return;
            }

            Console.WriteLine("Decompressing...");

            CffFile f = new CffFile();
            f.Load(args[0]);
            
            string dir = Path.GetFileNameWithoutExtension(args[0]);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            foreach (var file in f.Files)
                File.WriteAllBytes(Path.Combine(dir, file.Key.ToString()), file.Value);

            Console.WriteLine("Decompressed");
        }
    }
}
