using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using TUGraz.VectoCore.Utils;

namespace HashingTool.Util
{
	public class XMLValidator
	{
		private readonly Action<XmlSeverityType, ValidationEvent> _validationErrorAction;
		private readonly Action<bool> _resultAction;
		private bool _valid;

		public XMLValidator(Action<bool> resultaction, Action<XmlSeverityType, ValidationEvent> validationErrorAction)
		{
			_validationErrorAction = validationErrorAction ?? ((x, y) => { });
			_resultAction = resultaction ?? (x => { });
			_valid = false;
		}

		public Task<bool> ValidateXML(XmlReader hashedComponent)
		{
			var task = new Task<bool>(() => DoValidation(hashedComponent));
			task.Start();
			return task;
		}

		private bool DoValidation(XmlReader hashedComponent)
		{
			_valid = true;
			try {
				var settings = new XmlReaderSettings {
					ValidationType = ValidationType.Schema,
					ValidationFlags = //XmlSchemaValidationFlags.ProcessInlineSchema |
						//XmlSchemaValidationFlags.ProcessSchemaLocation |
						XmlSchemaValidationFlags.ReportValidationWarnings
				};
				settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
				settings.Schemas.Add(GetXMLSchema(""));

				var vreader = XmlReader.Create(hashedComponent, settings);
				var doc = new XmlDocument();
				doc.Load(vreader);
				doc.Validate(ValidationCallBack);
			} catch (Exception e) {
				_validationErrorAction(XmlSeverityType.Error, new ValidationEvent() { Exception = e });
			}
			return _valid;
		}

		private void ValidationCallBack(object sender, ValidationEventArgs args)
		{
			_resultAction(false);
			_valid = false;
			_validationErrorAction(args.Severity, new ValidationEvent { ValidationEventArgs = args });
		}

		private static XmlSchemaSet GetXMLSchema(string version)
		{
			var xset = new XmlSchemaSet() { XmlResolver = new XmlResourceResolver() };
			foreach (var schema in new[] {"VectoComponent.xsd", "VectoInput.xsd", "VectoOutputManufacturer.xsd", "VectoOutputCustomer.xsd"}) {
				var resource = RessourceHelper.LoadResourceAsStream(RessourceHelper.ResourceType.XMLSchema, schema);

				var reader = XmlReader.Create(resource, new XmlReaderSettings(), "schema://");
				xset.Add(XmlSchema.Read(reader, null));				
			}
			xset.Compile();
			return xset;
		}
	}

	public class ValidationEvent
	{
		public Exception Exception;
		public ValidationEventArgs ValidationEventArgs;
	}
}
