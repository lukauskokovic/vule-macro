# vule-macro
Application that lets you write and simulate input commands (key press, mouse move click etc...)<br>
First you must create the script file,in it you write the one of the commands:(all can be done via the app)<br>

1. <b>MOUSE</b> <b><i>BUTTON</i> <i>Direction</i>(optional)</b><br>
MOUSE = used for simulating mouse press/down/up
BUTTON = LEFT/RIGHT<br>
DIRECTION = UP/DOWN/EMPTY (up - release the button, down - hold the button, empty - press)<br>
2. <b>SLEEP</b> timeUNIT<i><br>
SLEEP = sleeps/waits the duration you entered<br>
time = the time you want to sleep<br>
UNIT = the unit (MS-milisecond/S-second/M-minute)<br>
3. <b>JUMP</b> <i>Instruction</i><br>
JUMP = jumps/goes to the given instruction NOT LINE!<br>
INSTRUCTION = number of the instruction to go to<br>
4. <b>KEY</b> <b><i>BUTTON</i> <i>Direction</i>(optional)</b><br>
KEY = used for simulating key press todo:down/up<br>
BUTTON = the name of the button you want to be pressed<br>
DIRECTION = HOLD/RELEASE/EMPTY<br>
5. <b>CURSOR</b> <b><i>POS/MOVE</i> <i>X Y</i></b><br>
CURSOR = used for simulating cursor movement
POS/MOVE = pos stands for position wich is used to set the cursor to specified position on screen, move is used for moving the cursor from its current position
X Y = x y axis

  <h2>This shows the main menu with the 'script 1' selected</h2>
<img src="https://user-images.githubusercontent.com/49611825/142741084-28c09200-26e2-4445-8151-e8396d47f138.PNG" />
  <h2>This shows the menu in wich you can change the button on wich you want to run the script</h2>
<img src="https://user-images.githubusercontent.com/49611825/142741194-053ac962-b0d9-4197-af39-cd540fa0a8f4.PNG" />
<img src="https://user-images.githubusercontent.com/49611825/142741210-eb066458-2557-4443-8a72-58440f6827d4.PNG" />
