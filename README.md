this version contains:

​	1.physics engine

​	2.8-ball game logic

​	3.AI

​	4.UI

****Note that some Debug.Log shows Chinese that is for my own convenience. Just ignore it and I'll change that later.

Details:

| Script               | Method                      | Original? | Brief  description of functions                              |
| -------------------- | --------------------------- | --------- | ------------------------------------------------------------ |
| Balls.cs             | putBalls                    | Yes       | Place  balls into a triangle shape                           |
|                      | iniBalls                    | Yes       | Initialize  balls                                            |
|                      | randomPickStrip             | Yes       | Randomly  select a strip ball                                |
|                      | randomPickSolid             | Yes       | Randomly  select a solid ball                                |
|                      | random0or1                  | Yes       | Randomly  generate 0 or 1                                    |
|                      | findNearestBall             | Yes       | Find  the ball closest to the cue ball                       |
|                      | findNextPossibleBall        | Yes       | Find  the next target according to the assigned color of rules |
|                      | LocateWhiteAndPlaceCue      | Yes       | Locate  cue ball and place the cue next to it                |
|                      | moveTarget                  | Yes       | Adjust  shooting angle to left or right                      |
|                      | helperLineAndBall           | Yes       | Calculate  the auxiliary line vertices and place the auxiliary ball |
|                      | HelperBallCenter            | Yes       | Calculate  the center of the auxiliary ball                  |
|                      | HitBall                     | Yes       | Perform  the shoot action                                    |
|                      | redDotisDistance            | Yes       | Limit  the position of the red dot to the UI                 |
|                      | spinAdjust                  | Yes       | Adjust  spin angle                                           |
|                      | noBallIsMoving              | Yes       | Check  if every ball is stopped                              |
|                      | whiteHittedRightColor       | Yes       | Check  violation: if the cue ball hit the ball of correct color first |
|                      | isTimeForBlackBall          | Yes       | Check  violation: if the cue ball should hit black ball now  |
|                      | collideWtihAnotherBallOnPos | Yes       | if  the variable "Vector3 pos" is within the collider bound of another  ball on table |
|                      | freeBall                    | Yes       | place  ball in hand                                          |
|                      | ballNumToColor              | Yes       | translate  ball number to color i.e ball[1] --> solid        |
|                      | win                         | Yes       | win  the game                                                |
|                      | lose                        | Yes       | lost  the game                                               |
|                      | hasValidColor               | Yes       | Check  if                                                    |
|                      | changePlayer                | Yes       | Check  if there is a ball of assigned color among the balls just dropped |
|                      | keepGoing                   | Yes       | Game  logic: the previous player keeps shooting              |
|                      | gameLogic                   | Yes       | Perform  game logic check                                    |
|                      | wait                        | Yes       | wait  for specific seconds                                   |
| ball0Script.cs       | iniCollisonHandeler         | Yes       | Initialize  collisonHandelers                                |
|                      | iniWalls                    | Yes       | Initialize  walls                                            |
|                      | hitWall                     | Yes       | Calculate  normal and exit angle.                            |
|                      | wallSpinSimulate            | Yes       | Simulate  wallSpin. This is a helper method for helperLineAndBall in Balls.cs |
|                      | wallSpin                    | Yes       | Perform  the acceleration and deceleration effect when a ball hits a wall with  non-zero spin speed |
|                      | getHittedSimulate           | Yes       | Simulate  getHitted. This is a helper method for helperLineAndBall in Balls.cs |
|                      | getHitted                   | Yes       | Calculate  velocity/angular velocity/spin speed and perform the collide action of two  balls. This method is abandoned since the colliding check of multiple  fast-moving objects are so bad in one thread. Balls will pass through other  balls. |
|                      | getHittedBeta               | Yes       | A  better version of getHitted. This version fixed every bug in the colliding  check of multiple fast-moving objects by using multiple threads. Each thread  only handles the colliding of two specific balls. |
|                      | hittedByCue                 | Yes       | Perform  the action of shooting the cue ball                 |
|                      | hittedByCueSimulate         | Yes       | Simulate  hittedByCue. This is a helper method for helperLineAndBall in Balls.cs |
|                      | isStopped                   | Yes       | check  if the ball that this script is attached to stops moving |
| CollisionHandeler.cs | getHitted                   | Yes       | Responding  to getHittedBeta. Calculate velocity/angular velocity/spin speed and perform  the collide action of two balls. |
| AI.cs                | ballNumToColor              | Yes       | translate  ball number to color i.e ball[13] --> strip       |
|                      | findABallStraightToBag      | Yes       | Find  a ball that no other objects are on the path toward bags. i.e find a easy  target. |
|                      | ballExistOnPath             | Yes       | Check  if no other objects are on the path of the ball that this script is attached  to toward bags |
|                      | collideWtihAnotherBallOnPos | Yes       | if  the variable "Vector3 pos" is within the collider bound of another  ball on table |
|                      | isOnTable                   | Yes       | if  the variable "Vector3 pos" is within the area of the table |
|                      | placeFreeBall               | Yes       | Perform  "ball in hand"                                      |
|                      | NextGaussian                | No        | Generate  a gaussian distributed random number This method is quoted from   https://blog.csdn.net/georgeandgeorge/article/details/89480224 |
|                      | findAngleToWall             | Yes       | Find  a angle to shoot. By shooting at this angle, the target ball can be reached  after reflection through the wall |
|                      | findAngleToshoot            | Yes       | Find  a angle to shoot.                                      |
|                      | RandomShoot                 | Yes       | Perform  a shoot with random angle/power/spin                |
|                      | backShoot                   | Yes       | Perform  a draw shot i.e hit at the bottom of the cue ball   |
|                      | Shoot                       | Yes       | Perform  a normal shot                                       |
|                      | getWhiteOutputToBag         | Yes       | Check  the exit angle of the cue ball is toward a bag i.e check the potential of  pocketing of the cue ball |
|                      | AIThinking                  | Yes       | Performe  AI thinking logic                                  |
|                      | AIShoot                     | Yes       | Performe  AI shooting logic                                  |