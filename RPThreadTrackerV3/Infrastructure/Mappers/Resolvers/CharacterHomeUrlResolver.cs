namespace RPThreadTrackerV3.Infrastructure.Mappers.Resolvers
{
	using AutoMapper;
	using Enums;
	using Models.DomainModels;
	using Models.ViewModels;

	public class CharacterHomeUrlResolver : IValueResolver<Character, CharacterDto, string>
	{
		public string Resolve(Character source, CharacterDto destination, string destMember, ResolutionContext context)
		{
			switch (source.PlatformId)
			{
				case Platform.Tumblr:
					return GetTumblrHomeUrl(source);
				default:
					return null;
			}
		}

		private string GetTumblrHomeUrl(Character source)
		{
			return $"http://{source.UrlIdentifier}.tumblr.com";
		}
	}
}
