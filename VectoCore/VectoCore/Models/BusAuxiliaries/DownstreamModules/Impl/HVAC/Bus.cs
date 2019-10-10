using System;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class Bus : IBus
	{

		// Private Property Backing
		private int _id;
		private string _model;
		private string _floorType;
		private string _engineType;
		private double _lengthInMetres;
		private double _widthInMetres;
		private double _heightInMetres;
		private int _registeredPassengers;
		private bool _isDoubleDecker;

		public int Id
		{
			get {
				return _id;
			}
		}

		public string Model
		{
			get {
				return _model;
			}
			set {
				if (!ModelOK(value))
					throw new ArgumentException("Model argument is invalid");
				_model = value;
			}
		}

		public string FloorType
		{
			get {
				return _floorType;
			}
			set {
				if (!FloorTypeOK(value))
					throw new ArgumentException("Model argument is invalid");
				_floorType = value;
			}
		}

		public string EngineType
		{
			get {
				return _engineType;
			}
			set {
				if (!EngineOK(value))
					throw new ArgumentException("EngineType argument is invalid");
				_engineType = value;
			}
		}

		public double LengthInMetres
		{
			get {
				return _lengthInMetres;
			}
			set {
				if (!DimensionOK(value))
					throw new ArgumentException("Invalid Length");
				_lengthInMetres = value;
			}
		}

		public double WidthInMetres
		{
			get {
				return _widthInMetres;
			}
			set {
				if (!DimensionOK(value))
					throw new ArgumentException("Invalid Width");
				_widthInMetres = value;
			}
		}

		public double HeightInMetres
		{
			get {
				return _heightInMetres;
			}
			set {
				if (!DimensionOK(value))
					throw new ArgumentException("Invalid Height");
				_heightInMetres = value;
			}
		}

		public int RegisteredPassengers
		{
			get {
				return _registeredPassengers;
			}
			set {
				if (!PassengersOK(value))
					throw new ArgumentException("Invalid Number Of Passengers");
				_registeredPassengers = value;
			}
		}

		public bool IsDoubleDecker
		{
			get {
				return _isDoubleDecker;
			}
			set {
				_isDoubleDecker = value;
			}
		}

		// Constructors
		public Bus(int _id, string _model, string _floorType, string _engineType, double _lengthInMetres, double _widthInMetres, double _heightInMetres, int _registeredPassengers, bool _doubleDecker)
		{


			// Validity checks.
			if (!ModelOK(_model))
				throw new ArgumentException("Model argument is invalid");
			if (!FloorTypeOK(_floorType))
				throw new ArgumentException("Model argument is invalid");
			if (!EngineOK(_engineType))
				throw new ArgumentException("EngineType argument is invalid");
			if (!DimensionOK(_lengthInMetres))
				throw new ArgumentException("Invalid Length");
			if (!DimensionOK(_widthInMetres))
				throw new ArgumentException("Invalid Width");
			if (!DimensionOK(_heightInMetres))
				throw new ArgumentException("Invalid Height");
			if (!PassengersOK(_registeredPassengers))
				throw new ArgumentException("Invalid Number Of Passengers");


			// Set Private Members
			this._id = _id;
			this._model = _model;
			this._floorType = _floorType;
			this._engineType = _engineType;
			this._lengthInMetres = _lengthInMetres;
			this._widthInMetres = _widthInMetres;
			this._heightInMetres = _heightInMetres;
			this._registeredPassengers = _registeredPassengers;
			this._isDoubleDecker = _doubleDecker;
		}

		// Construction Validators Helpers                                     
		private bool ModelOK(string model)
		{
			model = model.ToLower();

			if (model == null || model.Trim().Length == 0)
				return false;

			return true;
		}

		private bool FloorTypeOK(string floorType)
		{
			floorType = floorType.ToLower();

			if (floorType == null || floorType.Trim().Length == 0)
				return false;

			if (floorType != "raised floor" && floorType != "low floor" && floorType != "semi low floor")
				return false;

			return true;
		}

		private bool EngineOK(string engine)
		{
			engine = engine.ToLower();

			if (engine == null || engine.Trim().Length == 0)
				return false;

			if (engine != "diesel" && engine != "gas" && engine != "hybrid")
				return false;

			return true;
		}

		private bool DimensionOK(double dimension)
		{
			return dimension > 0.5;
		}

		private bool PassengersOK(int registeredPassengers)
		{
			return registeredPassengers > 1;
		}

		// To String function
		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", _model, _floorType, _engineType, _lengthInMetres, _widthInMetres, _heightInMetres, _registeredPassengers, _isDoubleDecker);
		}
	}
}
