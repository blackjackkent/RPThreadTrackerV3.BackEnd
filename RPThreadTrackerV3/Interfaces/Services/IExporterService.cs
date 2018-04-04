namespace RPThreadTrackerV3.Interfaces.Services
{
	using System.Collections.Generic;
	using Models.DomainModels;

	public interface IExporterService
    {
	    byte[] GetByteArray(IEnumerable<Character> characters, Dictionary<int, List<Thread>> threads);
    }
}
