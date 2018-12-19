/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCommon.InputData
{
	public interface IVTPDeclarationInputDataProvider : IInputDataProvider
	{
		IVTPDeclarationJobInputData JobInputData { get; }

	}

	public interface IVTPDeclarationJobInputData
	{
		IVehicleDeclarationInputData Vehicle { get; }

		IManufacturerReport ManufacturerReportInputData { get; }

		IVectoHash VectoJobHash { get; }

		IVectoHash VectoManufacturerReportHash { get; }

		Meter Mileage { get; }

		IList<ICycleData> Cycles { get; }

		IEnumerable<double> FanPowerCoefficents { get; }

		bool SavedInDeclarationMode { get; }

		Meter FanDiameter { get; }
	}

	public interface IManufacturerReport
	{
		string Source { get; }

		IDictionary<VectoComponents,IList<string>> ComponentDigests { get; }

		DigestData JobDigest { get; }
	}

	public class DigestData
	{
		private const string ReferenceQueryXPath = ".//*[local-name()='Reference']/@URI";
		private const string AlgorithmQueryXPath = ".//*[local-name()='Transform']/@Algorithm";
		private const string DigestMethodQueryXPath = ".//*[local-name()='DigestMethod']/@Algorithm";
		private const string DigestValueQuerXPath = ".//*[local-name()='DigestValue']";

		public DigestData(string reference, string[] c14n, string digestMethod, string digestValue)
		{
			Reference = reference;
			CanonicalizationMethods = c14n;
			DigestMethod = digestMethod;
			DigestValue = digestValue;
		}

		public DigestData(XPathNavigator navigator)
		{
			Reference = navigator.SelectSingleNode(ReferenceQueryXPath)?.InnerXml;
			var nodes = navigator.Select(AlgorithmQueryXPath);
			var c14n = new List<string>();
			while (nodes.MoveNext()) {
				c14n.Add(nodes.Current.InnerXml);
			}
			CanonicalizationMethods = c14n.ToArray();
			DigestMethod = navigator.SelectSingleNode(DigestMethodQueryXPath)?.InnerXml;
			DigestValue = navigator.SelectSingleNode(DigestValueQuerXPath)?.InnerXml;
		}

		public DigestData(XmlNode xmlNode)
		{
			Reference = xmlNode.SelectSingleNode(ReferenceQueryXPath)?.InnerXml;
			var nodes = xmlNode.SelectNodes(AlgorithmQueryXPath);
			var c14n = new List<string>();
			if (nodes != null) {
				for (var i = 0; i < nodes.Count; i++) {
					c14n.Add(nodes[i].InnerXml);
				}
			}
			CanonicalizationMethods = c14n.ToArray();
			DigestMethod = xmlNode.SelectSingleNode(DigestMethodQueryXPath)?.InnerXml;
			DigestValue = xmlNode.SelectSingleNode(DigestValueQuerXPath)?.InnerXml;
		}

		public string DigestValue { get; }

		public string Reference { get; }

		public string[] CanonicalizationMethods { get; }
		public string DigestMethod { get; }

	}
}
