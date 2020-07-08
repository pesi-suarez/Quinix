The aim of this project is to create a [feature matrix](https://en.wikipedia.org/wiki/Feature_(machine_learning)) containing data from the Spanish Professional Football League historical record. This matrix can be used afterwards to train a classification model for predicting the Quiniela results (a certain kind of sports bet available in Spain).

The raw data is collected from the internet using a web scrapper. This data consists of football game results. For each game, the season, division, matchday, home team namne, away team, home team goals and away team goals are obtained. This data is then stored in a database.

Another tool is provided to generate the feature matrix from the game results in the database. The produced feature matrix has the following columns:
* season
* division
* home team name
* away team name
* matchday
* considering all previous matches in the season before the given matchday:
  * the point ratio of the home team
  * the winning ratio of the home team
  * the tieing ratio of the home team
  * the losing ratio of the home team
  * goals in favor per game of the home team
  * goals against per game of the home team
  * winning streak of the home team
  * tieing streak of the home team
  * losing streak of the home team
  * undefeated streak of the home team
  * non-winning steak of the home team
  * scoring streak of the home team
  * non-scoring streak of the home team
  * clean sheet streak of the home team
  * conceding streak of the home team.
 * the same last 15 variables but now only considering the home games played by the home team.
 * the same 15 variables but now only considering the last 3 games played by the home team.
 * the same 15 variables but now only considering the home games of the home team from the last 3 games played.
 * the same 30 + 30 variables (all games / home games) considering the last 5 and 10 games played.
 * the same 120 variables for the away team (and away games when appliable).
 * the result class (1 = home team victory, 2 = away team victory, x = tie).

 The structure of the repository is as follows:
 * A .Dot NET class library that does all the actual work.
 * A Xunit unit test project to test the library classes.
 * A console application for scrapping data and filling the database.
 * A console application for generating the feature matrix.
 * An auxiliar console application for generating just the classification table, useful for validating the scrapped data.
