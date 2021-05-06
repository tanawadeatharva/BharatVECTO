using System.Linq;
using System.Xml.Linq;
using Castle.Core.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using VECTO3GUI2020.Util.XML.Interfaces;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
	public interface IXMLBusAuxiliariesWriter : IXMLComponentWriter
	{

	}

	public abstract class XMLBusAuxiliariesWriter : IXMLBusAuxiliariesWriter
	{
		protected readonly IBusAuxiliariesDeclarationData _inputData;
		protected XElement _xElement;

		protected XMLBusAuxiliariesWriter(IBusAuxiliariesDeclarationData inputData)
		{
			_inputData = inputData;
		}

		public XElement GetElement()
		{
			if (_xElement == null)
			{
				Initialize();
				CreateElements();
			}

			return _xElement;
		}

		public XElement GetElement(XNamespace wrapperNamespace)
		{
			throw new System.NotImplementedException();
		}

		public abstract void Initialize();

		public abstract void CreateElements();
	}



	public class XMLBusAuxiliariesWriterMultistage : XMLBusAuxiliariesWriter
	{


		private XNamespace _defaultNamespace;
		public XMLBusAuxiliariesWriterMultistage(IBusAuxiliariesDeclarationData inputData) : base(inputData) { }

		#region Overrides of XMLBusAuxiliariesWriter

		public override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.V28; 
			_xElement = new XElement(_defaultNamespace + XMLNames.Component_Auxiliaries);
		}

		public override void CreateElements()
		{
	
			var dataElement = new XElement(_defaultNamespace + XMLNames.ComponentDataWrapper,
				new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, "CompletedVehicleAuxiliaryDataDeclarationType"));
			_xElement.Add(dataElement);


			if (_inputData.ElectricConsumers != null) {
				var electricSystemElement = new XElement(_defaultNamespace + XMLNames.BusAux_ElectricSystem);
				var ledLightsElement = new XElement(_defaultNamespace + "LEDLights");
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Interiorlights, _inputData.ElectricConsumers.InteriorLightsLED));
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Dayrunninglights, _inputData.ElectricConsumers.DayrunninglightsLED));
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Positionlights, _inputData.ElectricConsumers.PositionlightsLED));
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Brakelights, _inputData.ElectricConsumers.BrakelightsLED));
				ledLightsElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_Headlights, _inputData.ElectricConsumers.HeadlightsLED));
				electricSystemElement.Add(ledLightsElement);
				dataElement.Add(electricSystemElement);
			}

			if (_inputData.HVACAux != null) {
				var hvacElement = new XElement(_defaultNamespace + "HVAC");
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_SystemConfiguration, _inputData.HVACAux.SystemConfiguration.GetXmlFormat()));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_HeatPumpTypeDriver, _inputData.HVACAux.HeatPumpTypeDriverCompartment.GetLabel()));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_HeatPumpModeDriver, _inputData.HVACAux.HeatPumpModeDriverCompartment?.GetLabel()));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_HeatPumpTypePassenger, _inputData.HVACAux.HeatPumpTypePassengerCompartment?.GetLabel()));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_HeatPumpModePassenger, _inputData.HVACAux.HeatPumpModePassengerCompartment?.GetLabel()));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_AuxiliaryHeaterPower, _inputData.HVACAux.AuxHeaterPower?.ToXMLFormat(0)));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_DoubleGlazing, _inputData.HVACAux.DoubleGlazing));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_AdjustableAuxiliaryHeater, _inputData.HVACAux.AdjustableAuxiliaryHeater));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_SeparateAirDistributionDucts, _inputData.HVACAux.SeparateAirDistributionDucts));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_WaterElectricHeater, _inputData.HVACAux.WaterElectricHeater));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_AirElectricHeater, _inputData.HVACAux.AirElectricHeater));
				hvacElement.Add(new XElement(_defaultNamespace + XMLNames.Bus_OtherHeatingTechnology, _inputData.HVACAux.OtherHeatingTechnology));
				dataElement.Add(hvacElement);
			}
		}

		#endregion
	}
}