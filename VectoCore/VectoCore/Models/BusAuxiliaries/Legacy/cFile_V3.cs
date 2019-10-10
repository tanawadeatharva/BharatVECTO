// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using TUGraz.VectoCore.Models.BusAuxiliaries.Util;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Legacy {
	public class cFile_V3
	{
		private TextFieldParser TxtFldParser;
		private StreamWriter StrWrter;
		private FileMode Mode;
		private string Path;
		private string Sepp;
		private bool SkipCom;
		private bool StopE;
		private bool FileOpen;
		private string[] PreLine;
		private bool FileEnd;

		// File format
		private System.Text.Encoding FileFormat = System.Text.Encoding.UTF8;

		public cFile_V3()
		{
			Reset();
		}

		private void Reset()
		{
			FileOpen = false;
			Mode = FileMode.Undefined;
			PreLine = null;
			FileEnd = false;
		}

		public bool OpenRead(string FileName, string Separator = ",", bool SkipComment = true, bool StopAtE = false)
		{
			Reset();
			StopE = StopAtE;
			Path = FileName;
			Sepp = Separator;
			SkipCom = SkipComment;
			if (!(Mode == FileMode.Undefined))
				return false;
			if (!File.Exists(Path))
				return false;
			Mode = FileMode.Read;
			try {
				TxtFldParser = new TextFieldParser(Path, System.Text.Encoding.Default);
				FileOpen = true;
			} catch (Exception ) {
				return false;
			}
			TxtFldParser.TextFieldType = FieldType.Delimited;
			TxtFldParser.Delimiters = new string[] { Sepp };

			// If TxtFldParser.EndOfData Then Return False

			ReadLine();
			return true;
		}

		public string[] ReadLine()
		{
			var line = PreLine;

			lb10:
			;
			if (TxtFldParser.EndOfData) {
				FileEnd = true;
			} else {
				PreLine = TxtFldParser.ReadFields();
				var line0 = PreLine[0].Trim().ToUpper();

				if (SkipCom) {
					if (FilePathUtils.Left(line0, 1) == "#")
						goto lb10;
				}

				if (StopE)
					FileEnd = (line0 == "E");
			}

			return line;
		}

		public void Close()
		{
			switch (Mode) {
				case FileMode.Read: {
					if (FileOpen)
						TxtFldParser.Close();
					TxtFldParser = null;
					break;
				}

				case FileMode.Write: {
					if (FileOpen)
						StrWrter.Close();
					StrWrter = null;
					break;
				}
			}
			Reset();
		}

		public bool EndOfFile
		{
			get {
				return FileEnd;
			}
		}

		public bool OpenWrite(string FileName, string Separator = ",", bool AutoFlush = false, bool Append = false)
		{
			Reset();
			Path = FileName;
			Sepp = Separator;
			if (Mode != FileMode.Undefined)
				return false;
			Mode = FileMode.Write;
			try {
				if (!Append) {
					File.Delete(Path);
				}
				StrWrter = new StreamWriter(File.OpenWrite(Path), FileFormat);
				FileOpen = true;
			} catch (Exception ) {
				return false;
			}
			StrWrter.AutoFlush = AutoFlush;
			return true;
		}

		public void WriteLine(params object[] x)
		{
			//string St;
			var StB = new System.Text.StringBuilder();
			bool Skip;
			Skip = true;
			foreach (var St in x) {
				if (Skip) {
					StB.Append(St);
					Skip = false;
				} else
					StB.Append(Sepp + St);
			}
			StrWrter.WriteLine(StB.ToString());
			StB = null;
		}

		public void WriteLine(string x)
		{
			StrWrter.WriteLine(x);
		}

		private enum FileMode
		{
			Undefined,
			Read,
			Write
		}
	}
}
