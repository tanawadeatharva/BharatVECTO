using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Tests.TestUtils;

public class LinearCurve
{
	private IList<(double x, double y)> _points = new List<(double x, double y)>();
	private IEnumerable<CurveSection> _sections;

	private class CurveSection
	{
		private readonly double _start_x;

		public double StartX => _start_x;

		private readonly double _end_x;

		public double EndX => _end_x;


		private double _k;
		private double _d;

		public CurveSection((double x, double y) start, (double x, double y) end)
		{
			_start_x = start.x;
			_end_x = end.x;

			_k = (start.y - end.y) / (start.x - end.x);
			_d = start.y - _k * start.x;
		}

		public double Lookup(double x)
		{
			return _k * x + _d;
		}
	}

	/// <summary>
	/// Class to lookup values in a linear curve with multiple sections
	/// </summary>
	public LinearCurve()
	{
		
	}


	public void AddPoint(double x, double y)
	{
		_points.Add((x, y));

		_points = _points.OrderBy(p => p.x).ToList();
		if (_points.Count >= 2) {
			_sections = _points.Pairwise((p1, p2) => new CurveSection(p1, p2));
		}
	}

	public double Lookup(double x)
	{
		return _sections.First(s => x.IsBetween(s.StartX, s.EndX)).Lookup(x);
	}

}