import pandas as pd
import matplotlib.pyplot as plt
from scipy.optimize import minimize
import numpy as np

from FuelCellClasses import read_fc_map, FuelCellString, FuelCellSystem
from PrintMethods import print_fuel_cell_string_fc_map, print_fc_data, printFuelCellSystem, printFuelCellSystemForPower

fc1 = read_fc_map(r'fuel_cell_maps/GenericFuelCellMassflowMap1.vfcm')
print_fc_data(fc1)

fc2 = read_fc_map(r'fuel_cell_maps/GenericFuelCellMassflowMap2.vfcm')
print_fc_data(fc2)

string1 = FuelCellString(fc1, 3)
string2 = FuelCellString(fc2, 3)

print_fuel_cell_string_fc_map(string1)
print_fuel_cell_string_fc_map(string2)

fuelCellSystem = FuelCellSystem(string1, string2)
#printFuelCellSystem(fuelCellSystem, False)

printFuelCellSystemForPower(fuelCellSystem, 300)


