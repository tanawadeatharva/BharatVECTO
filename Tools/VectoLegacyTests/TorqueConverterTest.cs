using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VECTO;

namespace VectoLegacyTests
{
	[TestClass]
	public class TorqueConverterTest
	{
		[ClassInitialize]
		public static void InitTests(TestContext ctx)
		{
			if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ".") {
				return;
			}
			try {
				Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			} catch (Exception ex) {}
		}

		[TestMethod]
		public void TestTorqueConverter()
		{
			cGBX cGbx = new cGBX();
			VECTO_Global.MyAppPath = @".\";
			VECTO_Global.MyDeclPath = @"E:\QUAM\Workspace\VECTO_quam\Declaration\";
			VECTO_Global.sKey = new csKey();
			VECTO_Global.Declaration = new cDeclaration();
			VECTO_Global.Cfg = new Configuration() {
				DeclMode = false
			};
			VECTO_Global.DEV = new cDEV() {
				TClimitOn = true,
				TClimit = 1600,
				TCaccmin = 0.025f,
				TCiterPrec = 0.001f,
			};
			VECTO_Global.VEC = new cVECTO() {
				EngOnly = false,
				AuxiliaryAssembly = "CLASSIC"
			};
			VECTO_Global.VEC.set_DesMaxFile(true,
				@"E:\QUAM\Workspace\VECTO_quam\Generic Vehicles\Engineering Mode\12t Delivery Truck\Truck.vacc");
			VECTO_Global.VEC.Init();

			VECTO_Global.DRI = new cDRI();
			VECTO_Global.DRI.FilePath = @"..\..\..\..\Declaration\MissionCycles\Citybus_Suburban.vdri";
			VECTO_Global.DRI.ReadFile();
			VECTO_Global.DRI.GradToAlt();

			VECTO_Global.MODdata = new cMOD();
			VECTO_Global.MODdata.Init();

			if (VECTO_Global.DRI.Scycle) {
				VECTO_Global.MODdata.Vh.SetAlt();
				var foo = VECTO_Global.DRI.ConvStoT();
			}

			VECTO_Global.MODdata.CycleInit();

			VECTO_Global.GBX = cGbx;
			VECTO_Global.ENG = new cENG();
			VECTO_Global.ENG.FilePath =
				@"..\..\..\..\Generic Vehicles\Engineering Mode\AT-TC Demo Vehicle\Engine.veng";
			//"..\\..\\..\\Generic Vehicles\\Declaration Mode\\12t Delivery Truck\\12t Delivery Truck.veng";
			VECTO_Global.ENG.ReadFile(true);
			VECTO_Global.ENG.Init();
			VECTO_Global.LogFile = new VECTO_Global.cLogFile();
			cGbx.FilePath = @"..\..\..\..\Generic Vehicles\Engineering Mode\AT-TC Demo Vehicle\Gearbox.vgbx";
			//"..\\..\\..\\Generic Vehicles\\Declaration Mode\\12t Delivery Truck\\12t Delivery Truck.vgbx";
			var flag = cGbx.ReadFile(true);
			flag = cGbx.GSinit();
			if (cGbx.TCon) {
				cGbx.TCinit();
			}
			flag = ((cENG)VECTO_Global.ENG).Init();
			//cGbx.DeclInit();

			for (var nOut = 10; nOut < 200; nOut += 10) {
				for (var Pout = 10; Pout < 200; Pout += 20) {
					flag = cGbx.TCiteration(1, nOut, Pout, 0);
					Assert.IsTrue(flag);
					Debug.WriteLine("n_out: {0}, P_out: {1}, n_in: {2}, Tq_in: {3}, reduce: {4}", nOut, Pout, cGbx.TCnUin, cGbx.TCMin,
						cGbx.TCReduce);
				}
			}

			//flag = cGbx.TCiteration(1, 100, 5, 0);
			//Assert.IsTrue(flag);

			//Assert.AreEqual(0, cGbx.TCnUin);
			//Assert.AreEqual(0, cGbx.TCMin);
		}
	}
}