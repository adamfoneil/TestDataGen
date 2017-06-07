# TestDataGen

There are several test data generator apps for SQL Server -- Red Gate, ApexSQL, and DevArt have tools that come to mind. I actually bought the Red Gate app. I did not try the ApexSQL app if I recall, but my problem with Red Gate's tool was that although you could do random foreign key lookups, you couldn't put a filter or predicate on those lookups. This is important in multi-tenant systems where foreign key values must be consistent within a certain customer "partition." In other words, you can't have foreign key values that cross customer boundaries. I have not seen that as a feature in any of the apps I looked at, so that's something I wanted to address.

This repo is just my baby steps towards a test data generator app for SQL Server. There is no user interface yet, just some low-level randomization and data access methods. I am not positive that I want to to build a GUI at all -- right now I'm just exploring feasibility.
