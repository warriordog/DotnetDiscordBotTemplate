# DotnetDiscordBotTemplate
Template project for creating Discord bots with .NET, using the DSharpPlus library.
Sample code is completely cross-platform and modular.
Use of Jetbrains Rider is recommended but not required.
This repository should also work with with Visual Studio, VS Code, or the dotnet CLI.

### Requirements
* .NET 5

### Setup
A few basic setup steps are required in order to run the sample bot. These are:
1. Register a bot with the [Discord Developer Portal](https://discord.com/developers/docs/intro), and get an auth / access token.
2. Open a terminal / command prompt to the `Main` folder.
3. Run `dotnet user-secrets init`.
4. Run `dotnet user-secrets set "DiscordAuth:DiscordToken" "INSERT TOKEN HERE"`, using the token from step 1.

### Sample bot
The sample bot responds to `bot!hello` with `Hello, world!`. It will not join any servers on its own, so you will need to invite it to a server in the typical way. Make sure to assign the `bot` scope and request permission to send messages.
