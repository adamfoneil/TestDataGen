# TestDataGen

Nuget package: **AoTestDataGen**

CodeProject article: [A-Random-Test-Data-Generator-Library](https://www.codeproject.com/Articles/1192440/AoTestDataGen-A-Random-Test-Data-Generator-Library)

There are several test data generator apps for SQL Server -- Red Gate, ApexSQL, and DevArt have tools that come to mind. I actually bought the Red Gate app. I did not try the ApexSQL app if I recall, but my problem with Red Gate's tool was that although you could do random foreign key lookups, you couldn't put a filter or predicate on those lookups. This is important in multi-tenant systems where foreign key values must be consistent within a certain customer "partition." In other words, you can't have foreign key values that cross customer boundaries. I have not seen that as a feature in any of the apps I looked at, so that's something I wanted to address.

This repo is just my baby steps towards a test data generator app for SQL Server. There is no user interface yet, just some low-level randomization and data access methods. I am not positive that I want to to build a GUI at all -- right now I'm just exploring feasibility.

Here's an example I used today (6/8/17) that generated 100 random "customer" records via an MVC action. Although my example uses the Postulate [SaveMultiple](https://github.com/adamosoftware/Postulate.Orm/blob/master/PostulateV1/Abstract/SqlDb_SaveMultiple.cs#L38) method to save the data, this library has no dependence on Postulate.

Here's a link to the [Generate](https://github.com/adamosoftware/TestDataGen/blob/master/TestDataGen2/TestDataGenerator.cs#L60) method in this library that does the actual work. Also, note the [Random](/TestDataGen2/TestDataGenerator.cs#L140) method. There are a couple overloads of it (as well as a [RandomWeighted](/TestDataGen2/TestDataGenerator.cs#L104) method). This is what supplies individual random values to a property when generating a random object.

![img](/tdg_sample.png)

Here's a couple more examples. Here is a simple use of the Random method to select any character from an array. There's no weighing in effect, so the 'M' and 'F' values will be more or less equally used:

![img](/tdg_random_sex1.png)

But you can also weigh items in source array differently so that certain selections come up more often. Here's an example that uses the RandomWeighted method to make 'M' come up 3x as often as 'F'. It relies on the [IWeighted](/TestDataGen2/IWeighted.cs) interface.

![img](/tdg_random_sex2.png)
