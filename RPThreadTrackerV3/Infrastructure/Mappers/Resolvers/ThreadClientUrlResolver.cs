namespace RPThreadTrackerV3.Infrastructure.Mappers.Resolvers
{
	using AutoMapper;
	using Enums;
	using Microsoft.Extensions.Configuration;
	using Models.DomainModels;
	using Models.ViewModels;

	public class ThreadClientUrlResolver : IValueResolver<Thread, ThreadDto, string>
	{
		private readonly IConfiguration _config;

		public ThreadClientUrlResolver(IConfiguration config)
		{
			_config = config;
		}
		public string Resolve(Thread source, ThreadDto destination, string destMember, ResolutionContext context)
		{
			var url = GetUrlForPlatform(source.Character?.PlatformId, source.PostId, source.Character?.UrlIdentifier, source.PartnerUrlIdentifier);
			return url;
		}

		private string GetUrlForPlatform(Platform? platformId, string postId, string characterUrlIdentifier, string partnerIdentifier)
		{
			if (platformId == Platform.Tumblr)
			{
				return GetTumblrClientUrl(postId, characterUrlIdentifier, partnerIdentifier);
			}
			return null;
		}

		private string GetTumblrClientUrl(string postId, string characterUrlIdentifier, string partnerUrlIdentifier)
		{
			var baseUrl = _config["ClientUrls:Tumblr"];
			var url = $"{baseUrl}api/thread?postId={postId}&characterUrlIdentifier={characterUrlIdentifier}";
			if (!string.IsNullOrEmpty(partnerUrlIdentifier))
			{
				url += $"&partnerUrlIdentifier={partnerUrlIdentifier}";
			}
			return url;
		}
	}
}
