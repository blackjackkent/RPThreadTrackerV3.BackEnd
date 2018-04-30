namespace RPThreadTrackerV3.Infrastructure.Exceptions.Characters
{
    using System;

    public class CharacterNotFoundException : Exception
    {
		public CharacterNotFoundException() : base("The requested character does not exist for the current user.") { }
    }
}
