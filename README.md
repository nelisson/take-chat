# The Amazing TakeChat!

**The TakeChat came to turn down your world!**

You will never talk to your friends in the same way again!

TakeChat transforms a boring console chat application into a new awesome experience.

With TakeChat you can:

* Choose your own nickname (in the welcome dialog)
* Get the list of registered users (/users)
* Send messages to #general channel (just type and send a message for all users)
* Send messages to a specific user (/u {userName} {message})
* Send messages privately to a specific user (/u {userName} {message})
* Left the general channel and close the application (/exit)
* Get the list of commands (/help)

After clone this repository, navigate to root directory and run
* *dotnet build*

Now you are ready to run the Server, this server handle multiple client connections
* *dotnet run --project .\TakeChat.Server*

To run the client application is the same way
* *dotnet run --project .\TakeChat.Client*

## YES, we have Tests!!!
This project has Unit Tests in the main classes with a almost fully code coverage (tsc tsc tsc)
*dotnet test* will run the tests for this solution and integrate do Coverlet to find the code coverage

![tests](https://user-images.githubusercontent.com/291539/130472633-33095dee-39a1-4369-afa7-38c7ab8716f9.png)

![image](https://user-images.githubusercontent.com/291539/130473370-f78fafcc-8c5d-4c66-841c-5a76fff2bd10.png)

TakeServer uses the RegisteredUser to interact, so the basic flow is tested in some way =D
