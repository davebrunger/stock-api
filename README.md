# stock-api

## Requirements
We are the London Stock Exchange and we are creating an API to receive notification of trades from
authorised brokers and expose the updated price to them.

We need to receive every exchange of shares happening in real-time, and we need to know:
- What stock has been exchanged (using the ticker symbol)
- At what price (in pound)
- Number of shares (can be a decimal number)
- ID of broker managing the trade

A relational database is used to store all transactions happening. Consider a minimal schema and
data structure.

We need to expose the current value of a stock, values for all the stocks on the market and values
for a range of them (as a list of ticker symbols).

For simplicity, consider the value of a stock as the average price of the stock through all transactions.

Assume you can use SDKs and middleware for common functionalities.

You task is to define a simple version of the API, including a definition of data model. Describe
shortly the system design for an MVP.

### Enhancements
Is this system scalable? How can it cope with high traffic? Can you identify bottlenecks and suggest
an improved design and architecture? Feel free to suggest a complete different approach, but be
sure you can obtain the same goal.

### Submission
If you can share a GitHub repos, that’s our preferred method. But if you prefer to send over a zip file
of the solution or write in a text document, it’s fine, but be sure it’s clear and understandable.

### Evaluation
This assignment should not take more than 2 hours. We are looking for well structure code and
problem solving focus. The solution does not have to be production ready, but consideration of NFRs
is important.

The problem definition is quite open, we want to see your ability with wide systems.
About enhancements, we do not expect a complete system design, more a high level description.
Prepare your ideas for the upcoming interview!

## Configuration
Before running, The Project needs to be configured either by setting the `DefaultConnection` coonection string
in `appsettings.json` or, preferably, by adding a `appsettings.local.json` file to the project that contains
the connection string. This allows standard configuration to be checked in to source control, and confidential
information to be omitted.

### Creating the Database
To create the database for the project run the following command, in the project directory, once the
connection string has been set:

    dotnet ef database update

## The Solution
The solution is a monolithic ASP.Net Core Web API project. I deemed that this was the quickest and
easiest way for me to meet the stated design goals. I have used EntityFrameworkCore for the database
connections with EFCore migrations to manage the database structure. The Repository pattern was used to
abstract the database context from the controllers to allow unit tests to be written easily. This is the
current recommendation from Microsoft for testing involving EFCore. Please see
[here](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy) for more information.
I have also included Swashbuckle to generate a simple Swagger UI that can be used for testing locally.
Documentation comments are used to supplement the automatically generated UI. Unit tests are provided in NUnit
and make use of FluentAssertions and MockQueryable.Moq.

### API Endpoints
The following API endpoints were created to meet the specification:

#### POST /exchanges
To register an exchange with the system

#### GET /averages
Gets the average prices in pounds for all the ticker symbols, alternatively the caller can specify a number
of ticker symbols using the `tickerSymbols` query parameter to limit the symbols returned. For example

    /averages?tickerSymbols=ABC&tickerSymbols=DEF

#### GET /averages/{tickerSymbol}
Gets the average price for a single ticker symbol.

#### Get /exchanges/{exchangeId}
This was not required by the specification, but was added for convenience. It also allowed me to show off use
of the `CreatedAtAction` action result. Tests are not provided for this action due to time constraints.

### Average Calculation
The average for each ticker symbol is re-calculated every time an exchange is registered. For a simple system
like this it should be sufficient. There is the possibility that an average could be updated by two requests
at the same time leading to a bad average calculation. This could be solved by creating a transaction object and
locking the averages table. There is obviously a trade off here between accuracy and performance.

Another alternative would be to calculate the average completely from scratch each time it was requested, but 
this also has problems. If the caller requests averages for all the ticker symbols and there are many symbols 
and exchanges for each symbol, the API could become very unresponsive.

Perhaps the best approach would be to flag each ticker symbol that has been updated and have a daemon process
regular calculate the averages for the ticker symbols that have been flagged.

### Other Enhancements

#### Security
Obviously this solution has no security in place. Before doing anything else it would probably be best to include
a security layer so that the broker has to prove who they are before creating or viewing individual exchanges.

#### Monolith
Currently the solution exists as a monolith. It could be broken into a number of microservices.

#### Scalability
I assume that this solution would not scale well. breaking the solution into microservices would allow each part of
the application to be scaled separately. Perhaps the registering of exchanges performs well, but displaying averages
is slow, or vice versa.

#### Message Queue
It might be an idea to write incoming exchanges to a message queue rather than straight the database and have a separate
service write them to the database. This could improve performance of the API, and also allow the other service to
calculate averages at write time. One could even dispence of the database altogether and just store the exchanges in the
queue. Although not fully supported in .Nety, using a technology like Kafka for the queues and Flink to query for the
averages.