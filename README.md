Arcmage
=======

Arcmage is a browser-based implementation of a CCG being developed as part 
of the [Arcmage project](https://arcmage.org). It offers the player a virtual 
table top for playing the game, without forcing any rules on the actual 
game being played.

No artificial intelligence was implemented in the platform nor are there any 
plans to implement game rules.

Furthermore, arcmage offers an online card generation tool to create new cards 
and decks.

Repository Structure
--------------------

The respository is used for both the card db/generation as for the online game

Card Database folders

* WTacticsDAL : The database orm layer for use with Entity Framework
* WTacticsLibrary : The buisiness logic for the card database/generation tool 
* WTacticsService : 
  * Holds the Rest API for Authentication, Card Creation, Deck Creation, Card Searching, ...
  * Holds the single page angular webapp for the card database/generation tool
  * Holds the single page vue webapp for the online game*

Online game demo folders

* WTacticsGameService: The owin/singalR hub backend for the browser to browser communication
* WTacticsWindowsService: A windows service to host the WTacticsGameService
* WTacticsGameConsole: A console program to host the WTacticsGameService (usefull for debugging)

Others
* ArtworkIndexer : simple tool to generate a single json file with metadata on the artwork repo
* SvgToCMYKPdf : example tool to use imagemagick for svg -> pdf conversion

Resources
---------

If you want to know more about the project please visit the following links or 
read the suggested documents:
- https://aminduna.arcmage.org/#/cards [Arcmage cards]
- https://aminduna.arcmage.org/#/games [Arcmage online game]
