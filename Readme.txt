 ------------------
- Brief on the game -
 ------------------

 This game is picked up from the unity turorial and initially was a multiplayer game with two user controlled players. Now however this game is extended to have the second player as an AI controlled element, each of the players have to strive to survive by exterminating the other.

 ----------------------------
 - Brief on the AI element -
 ----------------------------

The game itself is controlled by a game loop containing a finite state machine which then enable a decision tree that controls the shooting and the movement. The movement however is calculated through A* pathfinding algorithm in conjunction to the use of grid space representation.

----------------
- How to play -
----------------

Well, it is simple, you control the player using the arrow keys on your keyboard i.e. up and down is to move forward and backward, left and right are used to rotate. 

The space button is used to fire the shell onto the enemy, the more you hold it, the more further the shell will go.