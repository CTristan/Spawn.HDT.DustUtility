# Dust Utility v2.0
Enter the amout of dust you are aiming for and the plugin searches through your collection for cards that aren't being used in any deck in order for you to see which can be disenchanted.

## Features
#### Offline Mode
- The plugin saves your collection and decks locally, so you are still able to use it while Hearthstone isn't running.
#### Support for multiple accounts and regions
- With `Offline Mode` enabled, you are able to switch between multiple accounts and regions, if their collection has been saved once beforehand.
#### List of disenchanted cards
- If `Offline Mode` is enabled, the plugin is able to create and display a history of cards that have been disenchanted.
#### Search
- Search through your whole collection, instead of just cards that aren't used in a deck.
#### Advanced Search
- Advanced search allows you to search for specific cards.
#### Customizable Sort Order
- Order the result for your needs. Sortable properties: Mana Cost, Name, Dust, Class, Set, etc...
#### Card Image Tooltips
- Displays the actual card image while hovering over the the row.

## Settings
* Offline Mode: After opening the main window the plugin is going to try to store collection and decks locally every 10 sec while Hearthstone is running (Decks can only be saved after visiting the "Play" menu once). If successful, it will try to store collection and decks and check for disenchanted cards every minute while Hearthstone is running.
* Check For Updates: Checks if there is new release available on the GitHub page after opening the main window.
* Card Image Tooltips: Downloads card images and shows them when hovering over a row.
* Local Image Cache: Stores the downloaded images for faster access times and the plugin is still able to display them while being not connected to the internet.
* Clear Local Cache: Deletes all downloaded images.
