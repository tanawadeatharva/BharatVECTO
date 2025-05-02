using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Framework;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML.Reports;

[TestFixture]
public class ReportsSyntaxTests
{
	private const string BASE_DIR = "TestData/XML/DeclarationReports";
	private const string MRF_DIR = "MRF-1.0";
	private const string CIF_DIR = "CIF-1.0";

	[TestCaseSource(nameof(GetMRFSampleFiles))]
	public void TestMRFSampleFiles(string filename)
	{
		var xmlreader = XmlReader.Create(filename);
		var validator = new XMLValidator(xmlreader);

		var result = validator.ValidateXML(XmlDocumentType.ManufacturerReport);
		Assert.IsTrue(result);
	}

	[TestCaseSource(nameof(GetCIFSampleFiles))]
	public void TestCIFSampleFiles(string filename)
	{
		var xmlreader = XmlReader.Create(filename);
		var validator = new XMLValidator(xmlreader);

		var result = validator.ValidateXML(XmlDocumentType.CustomerReport);
		Assert.IsTrue(result);
	}

    protected static IEnumerable<string> GetMRFSampleFiles()
	{
		return Directory.GetFiles(Path.Combine(BASE_DIR, MRF_DIR));
	}

	protected static IEnumerable<string> GetCIFSampleFiles()
	{
		return Directory.GetFiles(Path.Combine(BASE_DIR, CIF_DIR));
	}
}