namespace RPThreadTrackerV3.Infrastructure.Mappers.Resolvers
{
	using AutoMapper;
	using Enums;
	using Models.DomainModels;
	using Models.ViewModels;

	public class ThreadHomeUrlResolver : IValueResolver<Thread, ThreadDto, string>
	{
		public string Resolve(Thread source, ThreadDto destination, string destMember, ResolutionContext context)
		{
			switch (source.Character.PlatformId)
			{
				case Platform.Tumblr:
					return GetTumblrHomeUrl(source);
				default:
					return null;
			}
		}

		private string GetTumblrHomeUrl(Thread source)
		{
			return $"http://{source.Character.UrlIdentifier}.tumblr.com/post/{source.PostId}";
		}
	}
}
