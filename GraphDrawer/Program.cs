/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.Tests.Utils;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace GraphDrawer
{
    internal class Program
    {
        private const string HELP = @"
Tool for plotting graphs comparing Vecto 2.2 and Vecto 3

--split <leng> ... split input into parts of length <len> (in m), only distance output

";

        private static void Main(string[] args)
        {
            if (args.Contains("--split")) {
                Console.WriteLine("plotting graphs splitted by distance");
                var idx = Array.FindIndex(args, x => x == "--split");
                var lenght = int.Parse(args[idx + 1]);
                var success = true;
                var start = 0;
                do {
                    Console.WriteLine("plotting {0} - {1}", start / 1000, (start + lenght) / 1000);
                    success = GraphWriter.WriteDistanceSlice(args[0], args[1], start, start + lenght);
                    start += lenght;
                } while (success);
                Console.WriteLine("plotting full cycle");
                GraphWriter.Write(args[0], args[1]);
                Console.WriteLine("done");
                return;
            }
            Console.WriteLine("plotting graphs...");
            GraphWriter.Write(args[0], args[1]);
            Console.WriteLine("done");
        }
    }
}