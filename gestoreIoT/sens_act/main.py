import time

def lettura_file():
    reading = open("/Users/robertmelente/Desktop/aa22-23-gruppo13/gestoreIoT/sens_act/datiSensori", "r")
    return reading.readline()


def sensor():


def actuator(command, type):
    if command == 1:
        writing = open("/Users/robertmelente/Desktop/aa22-23-gruppo13/gestoreIoT/sens_act/actHistory", "a")
        writing.write(time.time, type, "Activated")

    if command == 0:
        writing = open("/Users/robertmelente/Desktop/aa22-23-gruppo13/gestoreIoT/sens_act/actHistory", "a")
        writing.write(time.time, type, "Deactivated")
