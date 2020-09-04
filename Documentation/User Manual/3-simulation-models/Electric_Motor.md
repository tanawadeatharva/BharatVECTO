##Electric Motor

The electric motor is modeled by basically 4 map files:
 - Maximum drive torque over motor speed
 - Maximum generation torque over motor speed
 - Drag curve (i.e., the motor is not energized) over motor speed
 - Electric power map

The first two curves are read from a single .vemp file (see [Electric Motor Max Torque File (.vemp)](#electric-motor-max-torque-file-.vemp)). The drag curve is provided in a .vemd file (see [Electric Motor Drag Curve File (.vemd)](#electric-motor-drag-curve-file-.vemd)) and the electric power map in a .vemo file (see [Electric Motor Map (.vemo)](#electric-motor-map-.vemo)).

The convention for all input files is that positive torque values drive the vehicle while negative torque values apply additional drag and generate electric power.
