# RPThreadTrackerV3.BackEnd
> Web service API used by RPThreadTracker.com V3
>
> If you are looking for the front-end Javascript code, please see [http://www.github.com/blackjackkent/RPThreadTrackerV3.FrontEnd](http://www.github.com/blackjackkent/RPThreadTrackerV3.FrontEnd).
>
> If you are looking for the Tumblr thread status microservice API, please see [http://www.github.com/blackjackkent/RPThreadTracker.BackEnd.TumblrClient](http://www.github.com/blackjackkent/RPThreadTracker.BackEnd.TumblrClient).

[![Build Status](https://travis-ci.org/blackjackkent/RPThreadTrackerV3.BackEnd.svg?branch=development)](https://travis-ci.org/blackjackkent/RPThreadTrackerV3.BackEnd)
[![codecov](https://codecov.io/gh/blackjackkent/RPThreadTrackerV3.BackEnd/branch/development/graph/badge.svg)](https://codecov.io/gh/blackjackkent/RPThreadTracker.BackEnd.TumblrClient)


This is a web service API called by [RPThreadTrackerV3.FrontEnd](https://github.com/blackjackkent/RPThreadTrackerV3.FrontEnd) to retrieve information about a user's account, tracked characters, tracked threads, and other account tools.

## Documentation

Documentation for this API's endpoints can be found at [https://rpthreadtrackerv3-backend-docker.azurewebsites.net/docs](https://rpthreadtrackerv3-backend-docker/docs)

## Running the Application Locally

You will need to have the .NET Core 2.1 SDK installed on your local machine to develop this application.

1. Create a fork of this repository to your own GitHub account (<https://github.com/blackjackkent/RPThreadTrackerV3.BackEnd/fork>).
2. Clone the forked repository to your local machine.
3. Check out a new feature branch in your local copy of the code.
4. Duplicate the file at `./RPThreadTrackerV3.BackEnd/appsettings.secure.example.json` and name it `appsettings.secure.json`.

Once running, the application will be available at `http://localhost:29564`.

## Running Unit Tests

The application uses [XUnit](https://xunit.github.io/) and associated libraries for unit testing across all parts of the application. Any changes to the code should be appropriately unit tested to maintain code coverage. Test files should be added to the `RPThreadTrackerV3.BackEnd.Test` project following existing patterns.

You can run all unit tests using your preferred C# test runner. To generate a code coverage report, run `./coverage.sh` from the project root.

## External Dependencies

This application communicates with the following external services:

1. A SQL Server database for Tracker-specific account information. The connection string for this database is set in `./RPThreadTrackerV3.BackEnd/appsettings.json`. You can use the scripts in `./SQL/InitDatabase` to set up a local copy of the database tables for development purposes.
2. A DocumentDB NoSQL database for maintaining information about customized public views. You can run a local DocumentDB server using the [Azure CosmosDB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator), and set the connection information for this server in the `./RPThreadTrackerV3.BackEnd/appsettings.secure.json` file that you created earlier.

	Unfortunately, this portion of the service can only be run on Windows machines at this time.
3. The SendGrid API for managing email transmission. If you wish to develop/modify features involving emails, you will need to set up a SendGrid account and generate an API key. The application SendGrid API key is set in the `./RPThreadTrackerV3.BackEnd/appsettings.secure.json` file that you created earlier.

Of the three services above, only the SQL Server database is absolutely required to run the application.

## Submitting a Change

1. Commit your changes to your feature branch and push it to your forked repository.
2. Open a pull request to the repository at https://github.com/blackjackkent/RPThreadTrackerV3.BackEnd.


## Meta

Rosalind Wills - [@blackjackkent](https://twitter.com/blackjackkent) – rosalind@blackjack-software.com

[https://github.com/blackjackkent/RPThreadTrackerV3.BackEnd](https://github.com/blackjackkent/RPThreadTrackerV3.BackEnd/)
