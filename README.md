This version of Pool Room contains:

​	1. Custom physics engine

​	2. Modified eight-ball game logic

​	3. AI Opponent

​	4. UI and HUD

****Note that some Debug.Log messages show Chinese, that is for personal convenience.

Details:

| Script               | Method                      | Original? | Brief  description of functions                              |
| -------------------- | --------------------------- | --------- | ------------------------------------------------------------ |
| Balls.cs             | putBalls                    | Yes       | Place  balls into a triangle shape                           |
| Balls.cs             | iniBalls                    | Yes       | Initialize  balls                                            |
| Balls.cs             | randomPickStrip             | Yes       | Randomly  select a strip ball                                |
| Balls.cs             | randomPickSolid             | Yes       | Randomly  select a solid ball                                |
| Balls.cs             | random0or1                  | Yes       | Randomly  generate 0 or 1                                    |
| Balls.cs             | findNearestBall             | Yes       | Find  the ball closest to the cue ball                       |
| Balls.cs             | findNextPossibleBall        | Yes       | Find  the next target according to the assigned color of rules |
| Balls.cs             | LocateWhiteAndPlaceCue      | Yes       | Locate  cue ball and place the cue next to it                |
| Balls.cs             | moveTarget                  | Yes       | Adjust  shooting angle to left or right                      |
| Balls.cs             | helperLineAndBall           | Yes       | Calculate  the auxiliary line vertices and place the auxiliary ball |
| Balls.cs             | HelperBallCenter            | Yes       | Calculate  the center of the auxiliary ball                  |
| Balls.cs             | HitBall                     | Yes       | Perform  the shoot action                                    |
| Balls.cs             | redDotisDistance            | Yes       | Limit  the position of the red dot to the UI                 |
| Balls.cs             | spinAdjust                  | Yes       | Adjust  spin angle                                           |
| Balls.cs             | noBallIsMoving              | Yes       | Check  if every ball is stopped                              |
| Balls.cs             | whiteHittedRightColor       | Yes       | Check  violation: if the cue ball hit the ball of correct color first |
| Balls.cs             | isTimeForBlackBall          | Yes       | Check  violation: if the cue ball should hit black ball now  |
| Balls.cs             | collideWtihAnotherBallOnPos | Yes       | if  the variable "Vector3 pos" is within the collider bound of another  ball on table |
| Balls.cs             | freeBall                    | Yes       | place  ball in hand                                          |
| Balls.cs             | ballNumToColor              | Yes       | translate  ball number to color i.e ball[1] --> solid        |
| Balls.cs             | win                         | Yes       | win  the game                                                |
| Balls.cs             | lose                        | Yes       | lost  the game                                               |
| Balls.cs             | hasValidColor               | Yes       | Check  if                                                    |
| Balls.cs             | changePlayer                | Yes       | Check  if there is a ball of assigned color among the balls just dropped |
| Balls.cs             | keepGoing                   | Yes       | Game  logic: the previous player keeps shooting              |
| Balls.cs             | gameLogic                   | Yes       | Perform  game logic check                                    |
| Balls.cs             | wait                        | Yes       | wait  for specific seconds                                   |
| ball0Script.cs       | iniCollisonHandeler         | Yes       | Initialize  collisonHandelers                                |
| ball0Script.cs       | iniWalls                    | Yes       | Initialize  walls                                            |
| ball0Script.cs       | hitWall                     | Yes       | Calculate  normal and exit angle.                            |
| ball0Script.cs       | wallSpinSimulate            | Yes       | Simulate  wallSpin. This is a helper method for helperLineAndBall in Balls.cs |
| ball0Script.cs       | wallSpin                    | Yes       | Perform  the acceleration and deceleration effect when a ball hits a wall with  non-zero spin speed |
| ball0Script.cs       | getHittedSimulate           | Yes       | Simulate  getHitted. This is a helper method for helperLineAndBall in Balls.cs |
| ball0Script.cs       | getHitted                   | Yes       | Calculate  velocity/angular velocity/spin speed and perform the collide action of two  balls. This method is abandoned since the colliding check of multiple  fast-moving objects are so bad in one thread. Balls will pass through other  balls. |
| ball0Script.cs       | getHittedBeta               | Yes       | A  better version of getHitted. This version fixed every bug in the colliding  check of multiple fast-moving objects by using multiple threads. Each thread  only handles the colliding of two specific balls. |
| ball0Script.cs       | hittedByCue                 | Yes       | Perform  the action of shooting the cue ball                 |
| ball0Script.cs       | hittedByCueSimulate         | Yes       | Simulate  hittedByCue. This is a helper method for helperLineAndBall in Balls.cs |
| ball0Script.cs       | isStopped                   | Yes       | check  if the ball that this script is attached to stops moving |
| CollisionHandeler.cs | getHitted                   | Yes       | Responding  to getHittedBeta. Calculate velocity/angular velocity/spin speed and perform  the collide action of two balls. |
| AI.cs                | ballNumToColor              | Yes       | translate  ball number to color i.e ball[13] --> strip       |
| AI.cs                | findABallStraightToBag      | Yes       | Find  a ball that no other objects are on the path toward bags. i.e find a easy  target. |
| AI.cs                | ballExistOnPath             | Yes       | Check  if no other objects are on the path of the ball that this script is attached  to toward bags |
| AI.cs                | collideWtihAnotherBallOnPos | Yes       | if  the variable "Vector3 pos" is within the collider bound of another  ball on table |
| AI.cs                | isOnTable                   | Yes       | if  the variable "Vector3 pos" is within the area of the table |
| AI.cs                | placeFreeBall               | Yes       | Perform  "ball in hand"                                      |
| AI.cs                | NextGaussian                | No        | Generate  a gaussian distributed random number This method is quoted from   https://blog.csdn.net/georgeandgeorge/article/details/89480224 |
| AI.cs                | findAngleToWall             | Yes       | Find  a angle to shoot. By shooting at this angle, the target ball can be reached  after reflection through the wall |
| AI.cs                | findAngleToshoot            | Yes       | Find  a angle to shoot.                                      |
| AI.cs                | RandomShoot                 | Yes       | Perform  a shoot with random angle/power/spin                |
| AI.cs                | backShoot                   | Yes       | Perform  a draw shot i.e hit at the bottom of the cue ball   |
| AI.cs                | Shoot                       | Yes       | Perform  a normal shot                                       |
| AI.cs                | getWhiteOutputToBag         | Yes       | Check  the exit angle of the cue ball is toward a bag i.e check the potential of  pocketing of the cue ball |
| AI.cs                | AIThinking                  | Yes       | Performe  AI thinking logic                                  |
| AI.cs                | AIShoot                     | Yes       | Performe  AI shooting logic                                  |

Other:

* Powerbar Adapted from Free Asset Store Healthbar
* Pool Table and Balls Purchased on Asset Store
