# LavenderChessClock1

This is a standalone Blazor WebAssembly application that serves as a customizable chess clock. The app is designed to support various types of time controls.
## Features
- **Play Modes**
    - **No Increment:** Play with a fixed countdown timer for each player, with no time added per turn.
    - **Increment Mode:** Allows time to be added at the beginning of each turn, offering a more flexible time control style.
    - **Delay Increment:** Adds a delay before the main countdown starts, providing more options for time control.
- **Game Controls**
    - **Pause and Resume:** The game can be paused at any moment and continued later, preserving current times and increment states.
    - **Game Reset or New Game:** When a game ends, you have the option to reset the game using the same settings or create a new game with fresh time control settings.
- **Customizable Time Settings**
    - **Separate or Shared Time Controls:** Select identical or unique time settings for each player.
    - **Customizable Increments:** Players do not need to use the same increment type. You can specify two different increment types for the game's players.

## Getting Started
- **Installation:** Clone the repository and open the solution in Visual Studio 2022 or later.
- **Run:** Build and run the app in your local browser to start using the chess clock.

## Usage
- **Select Player Time Controls:** Choose the preferred time settings for each player, adjusting individual increments if desired.
- **Ending Turn:** Players can either use the space bar, or click on their respective timer area, to end their turn.
- **Reset or New Game:** After each game, you can choose to reset the game according to the way you intially set it up, or create a new game from scratch.

## Stack
- **Front-End:** Blazor WebAssembly.
- **Backend:** No server backend is needed for this standalone application.
- **.NET Version:** .NET 8.0.

## Planned Improvements
- **Increments:** Allow user to determine if increment should be added at the beginning of the turn or the end.
- **Saved Games:** Save history of time controls and allow user to select from a previously configured game.
- **Themes:** Allow the user to define a color scheme, save and load custom themes.
- **Key Bindings:** Add support for custom key bindings, allowing the user to determine which keys perform which actions.
