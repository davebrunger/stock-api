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