import os
import string

import numpy as np
from unittest import TestCase
from parameterized import parameterized, parameterized_class

from FuelCellClasses import FuelCellSystem
from FuelCellClasses import FuelCellString
from FuelCellClasses import FuelCellMap
from FuelCellClasses import read_fc_map

from PrintMethods import print_fc_data
from PrintMethods import print_fuel_cell_string_fc_map
from PrintMethods import printFuelCellSystem
from PrintMethods import printFuelCellSystemForPower


import shutil



class TestFuelCell(TestCase):
    @parameterized.expand([
        (r'fuel_cell_maps/GenericFuelCellMassflowMap1.vfcm', 3,
         r'fuel_cell_maps/GenericFuelCellMassflowMap2.vfcm', 3,
         r'testoutput/different_fc_same_range/',
         [1, 50e3, 150e3, 200e3, 300e3, 400e3, 550e3]),

        (r'fuel_cell_maps/GenericFuelCellMassflowMap1.vfcm', 3,
         r'fuel_cell_maps/GenericFuelCellMassflowMap1.vfcm', 3,
         r'testoutput/same_fc_same_range/',
         [1, 47700, 50e3, 150e3, 200e3, 300e3, 400e3, 550e3]),

        (r'fuel_cell_maps/GenericFuelCellMassflowMap1.vfcm', 1,
         r'fuel_cell_maps/GenericFuelCellMassflowMap3.vfcm', 1,
         r'testoutput/different_fc_different_range/',
         [1, 47700, 50e3, 150e3, 200e3, 300e3, 400e3, 494e3, 495e3]),

        (r'fuel_cell_maps/GenericFuelCellMassflowMap1.vfcm', 3,
         r'fuel_cell_maps/GenericFuelCellMassflowMap3.vfcm', 3,
         r'testoutput/different_fc_different_range_#3/',
         [1, 47700, 50e3, 150e3, 200e3, 300e3, 400e3, 494e3, 495e3]),



    ])
    def test_multiple_fuelcells(self, fcmap1: string, count1: int, fcmap2: string, count2:int, outpath: string, powersplit):
        if not os.path.isdir(outpath):
            os.makedirs(outpath)

        fc1 = read_fc_map(fcmap1)

        shutil.copy2(fcmap1, outpath)  # target filename is /dst/dir/file.ext
        print_fc_data(fc1, outputPath=outpath)



        fc2 = read_fc_map(fcmap2)
        shutil.copy2(fcmap2, outpath)
        print_fc_data(fc2, outputPath=outpath)

        fcString1 = FuelCellString(fc1, count1)
        fcString2 = FuelCellString(fc2, count2)

        print_fuel_cell_string_fc_map(fcString1, outpath=outpath)
        print_fuel_cell_string_fc_map(fcString2, outpath=outpath)

        fcSystem = FuelCellSystem(fuel_cell_string_1=fcString1, fuel_cell_string_2=fcString2)


        for p in powersplit:
            printFuelCellSystemForPower(fcSystem, p / 1000, outpath=outpath)








