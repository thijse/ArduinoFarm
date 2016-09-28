# ArduinoFarm

ArduinoFarm is a project in very early stage of development. Its goal is to become a testenvironment for Arduino sketches and libraries. 
The studio will manage USB connections to multiple boards of different types. It will compile sketches for these  boards, upload the compiled code and run this code. The output will be fetched and parsed. The farm will understand Arduino Unit output, and report on any failed tests

current status: 
* The studio is able to resolve multiple USB boards (based on Vendor ID, serial no and some heuristics) and connect these to the virtual serial ports
* The studio is able find and understand all Arduino board descriptions 
