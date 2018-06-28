# Getting Started

## Contributing to RPThreadTracker

All contributions to RPThreadTrackerV3.BackEnd can be made via pull requests to https://github.com/blackjackkent/RPThreadTrackerV3.BackEnd. To generate a pull request with your code changes:

1. Fork the repository (https://github.com/blackjackkent/RPThreadTrackerV3.BackEnd/fork)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new pull request on GitHub

## Running the code locally

RPThreadTrackerV3.BackEnd is written in the .NET Core 2.0 framework and uses SQL Server and DocumentDB for data storage. In order to run the project locally on your machine:

1. Fork the repository and clone it to your computer.
2. Install SQL Server if you do not already have it installed.
3. Run `SQL/CreateDatabase.bat` to create the database and build the necessary tables.
4. Update `RPThreadTrackerV3.BackEnd/appSettings.json` with your SQL Server connection string if necessary.
5. Duplicate `RPThreadTrackerV3.BackEnd/appsettings.secure.example.json` and rename the file to `appsettings.secure.json`.
6. Install the [Azure CosmosDB emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator) to run DocumentDB locally.
7. Update your new `appsettings.secure.json` file with connection information for your emulated DocumentDB server.
8. If you are planning to run the [front-end project](https://github.com/blackjackkent/RPThreadTrackerV3.FrontEnd) as well, be sure that the `CorsUrl` key in `appsettings.json` is set to the URL where that application will be running.
9. If you are planning to test email-related functionality, fill in the relevant keys in `appsettings.secure.json`.

## Questions?

Contact the developer at [rosalind@blackjack-software.com](mailto:rosalind@blackjack-software.com) or [submit a GitHub issue](https://github.com/blackjackkent/RPThreadTrackerV3.BackEnd/issues).
