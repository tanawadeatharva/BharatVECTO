using System.Collections.Generic;

namespace DownstreamModules.Electrics
{
	public interface ICombinedAlternator
	{
		// Alternators List
		List<IAlternator> Alternators { get; set; }

		// Test Equality
		bool IsEqualTo(ICombinedAlternator other);
	}
}
