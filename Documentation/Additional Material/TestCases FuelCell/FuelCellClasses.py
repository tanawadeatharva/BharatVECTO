from typing import List, Union, Tuple, Any

import numpy as np
import pandas as pd
from numpy import ndarray, dtype, bool_, unsignedinteger, signedinteger, floating, complexfloating, number, timedelta64
from scipy.optimize import minimize

power_col = 'P_el_out [kW]'
cons_col = '\tm_H2 [g/h]'


def read_fc_map(vfcm_path):
    fc = FuelCellMap()
    fc.sourceFile = vfcm_path

    fc_df = pd.read_csv(vfcm_path)
    fc_df = fc_df.sort_values(by=power_col)

    fc_of_P = lambda p: np.interp(p, fc_df[power_col], fc_df[cons_col])
    fc.minPower = fc_df[power_col].min()
    fc.maxPower = fc_df[power_col].max()
    fc.fc_of_p = fc_of_P
    fc.eff = lambda x: x / fc.fc_of_p(x)
    fc.inv_eff = lambda x: fc.fc_of_p(x) / x
    fc.measured_points = fc_df[power_col]

    # cons = ({'type': 'ineq', 'fun': lambda x: x[0] - 2 * x[1] + 2},
    #
    #         {'type': 'ineq', 'fun': lambda x: -x[0] - 2 * x[1] + 6},
    #
    #         {'type': 'ineq', 'fun': lambda x: -x[0] + 2 * x[1] + 2})

    bnds = [(fc.minPower, fc.maxPower)]
    result = minimize(fun=fc.inv_eff, x0=((fc.minPower + fc.maxPower) / 2), bounds=bnds)
    fc.min_eff_power = result.x[0]
    return fc


class FuelCellMap:
    minPower = None
    maxPower = None
    fc_of_p = None
    sourceFile = ''
    eff = None
    inv_eff = None
    # rename
    min_eff_power = None

    measured_points = None

    def __getFuelConsumptionForPower(self, power, use_time_slicing: bool = True):
        if power < 0:
            raise Exception("power must not be < 0")

        if power < self.min_eff_power and use_time_slicing:
            return self.fc_of_p(self.min_eff_power) * power / self.min_eff_power

        return self.fc_of_p(power)

    def getFuelConsumption(self, power, use_time_slicing: bool = True):
        if hasattr(power, "__iter__"):
            return np.array([self.__getFuelConsumptionForPower(p, use_time_slicing) for p in power])
        else:
            return self.__getFuelConsumptionForPower(power, use_time_slicing)


    def __init__(self) -> None:
        super().__init__()
        self.sourceFile = None


class FuelCellString:
    fuel_cell: FuelCellMap = None

    def stringFuelConsumption(self, p):
        fcCount = np.floor(p / (self.fuel_cell.min_eff_power))
        fcCount = np.where(fcCount < 1, 1, fcCount)
        fcCount = np.where(fcCount > self.count, self.count, fcCount)
        p_values = p / fcCount

        return fcCount * (self.fuel_cell.getFuelConsumption(p / fcCount))

    def maxPower(self):
        return self.fuel_cell.maxPower * self.count

    def measuredPoints(self):
        mp = self.fuel_cell.measured_points
        min_eff_power_round = self.fuel_cell.min_eff_power.round()


        for i, measuredPoint in enumerate(mp[np.where(mp < 2 * min_eff_power_round)[0]]):
            yield measuredPoint, self.stringFuelConsumption(measuredPoint)

        for i, measuredPoint in enumerate(
                mp[np.where((mp >= min_eff_power_round) & (2 * mp < 3 * min_eff_power_round))[0]]):
            power = measuredPoint * 2
            yield power, self.stringFuelConsumption(power)

        for i, measuredPoint in enumerate(
                mp[np.where((mp >= min_eff_power_round))[0]]):
            power = measuredPoint * 3
            yield power, self.stringFuelConsumption(power)

    def minPower(self):
        return self.fuel_cell.minPower

    def __init__(self, fuel_cell, count):
        super().__init__()
        self.count = count
        self.fuel_cell = fuel_cell


class FuelCellSystem:
    def __init__(self, fuel_cell_string_1: FuelCellString, fuel_cell_string_2: FuelCellString) -> None:
        super().__init__()
        self.fuel_cell_string_1: FuelCellString = fuel_cell_string_1
        self.fuel_cell_string_2: FuelCellString = fuel_cell_string_2

    def minPower(self):
        return min(self.fuel_cell_string_1.minPower(), self.fuel_cell_string_2.minPower())

    def maxPower(self):
        return self.fuel_cell_string_1.maxPower() + self.fuel_cell_string_2.maxPower()

    # a -> refers to the power distribution between the fuel cell strings p = p*(a) + p*(1-a)
    def fc_consumption(self, power: float, a: float) -> (float, float, float):
        # if power < self.fuel_cell_string_1.minPower() and power < self.fuel_cell_string_2.minPower():
        #    return np.nan, np.nan, np.nan
        tol = 1e-6
        if power > (self.fuel_cell_string_1.maxPower() + self.fuel_cell_string_2.maxPower()):
            return np.nan, np.nan, np.nan

        b = 1 - a

        #if (b > (0 - tol) and (((b * power) + tol) < (self.fuel_cell_string_2.minPower()) or
        if((b * power) > (self.fuel_cell_string_2.maxPower()) - tol):
            return np.nan, np.nan, np.nan

    #  if(a > (0 - tol) and (((a * power) + tol) < (self.fuel_cell_string_1.minPower()) or
        if((a * power) > (self.fuel_cell_string_1.maxPower() - tol)):
            return np.nan, np.nan, np.nan


        assert (b + a == 1)
        fc_string1 = self.fuel_cell_string_1.stringFuelConsumption(a * power)
        fc_string2 = self.fuel_cell_string_2.stringFuelConsumption(b * power)

        fc_total = fc_string1 + fc_string2

        return fc_total, fc_string1, fc_string2

    def splits(self, power):
        tolerance = 1e-6
        fc1 = np.array([x for x in self.fuel_cell_string_1.measuredPoints()])[:, 0]
        fc2 = np.array([x for x in self.fuel_cell_string_2.measuredPoints()])[:, 0]

        concat_measured = np.concatenate((fc1, fc2))
        concat_measured = np.pad(concat_measured, (0,1), 'constant')
        concat_measured = np.unique(concat_measured)
        concat_measured.sort()

        # fc1 is lead,  a1 represents the share of fc1 of the total power
        a1 = concat_measured / power


        a1 = a1[(a1 >= 0 - tolerance) & (a1 <= 1 + tolerance)]  # Filter shares that are too high
        a1 = a1[
            #(a1 * power >= fc1.min() - tolerance) &
            (a1 * power <= fc1.max() + tolerance) &
            #& ((1 - a1) * power >= fc2.min() - tolerance)
            ((1 - a1) * power <= fc2.max() + tolerance)]

        # fc2 is lead, b2 represents the share of the total power
        b2 = concat_measured / power
        b2 = b2[(b2 >= 0 - tolerance) & (b2 <= 1 + tolerance)]

        b2 = b2[
            #(b2 * power >= fc2.min() - tolerance) &
            (b2 * power <= fc2.max() + tolerance) &
            #((1 - b2) * power >= fc1.min() - tolerance) &
            ((1 - b2) * power <= fc1.max() + tolerance)]

        a2 = 1 - b2

        a = np.concatenate((a1, a2))
        a = np.unique(a)
        #a = a[(a * power >= fc1.min()) & (a * power <= fc1.max()) & ((1 - a) * power >= fc2.min()) & (
        #        (1 - a) * power <= fc2.max())]
        return a
