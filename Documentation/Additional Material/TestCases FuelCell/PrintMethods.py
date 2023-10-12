import string
from pathlib import Path
import numpy as np
from matplotlib import pyplot as plt

from FuelCellClasses import FuelCellString, FuelCellMap, FuelCellSystem


def print_fuel_cell_string_fc_map(fcstring: FuelCellString, outpath: string = None, close=True):
    fig, ax1 = plt.subplots(figsize=(8, 6))
    fc_ = fcstring.fuel_cell
    p_string = np.linspace(fc_.minPower, fc_.maxPower * 3, 10000)
    total_fc = fcstring.stringFuelConsumption(p_string)
    fig, ax1 = plt.subplots(figsize=(11.69,8.27))
    ax1.plot(p_string, total_fc, label='fc_total_string1', color='blue')
    ax1.legend()
    plt.axvline(x=fc_.min_eff_power, color='r', label="1 min eff power")
    plt.axvline(x=fc_.min_eff_power * 2, color='r', label="2 min eff power")
    plt.axvline(x=fc_.min_eff_power * 3, color='r', label="3 min eff power")
    measuredPoints = [x for x in fcstring.measuredPoints()]
    plt.scatter(x = [x[0] for x in fcstring.measuredPoints()], y = [x[1] for x in fcstring.measuredPoints()], label="measured points")
    plt.title("Fuel Cell String " + fcstring.fuel_cell.sourceFile + "-" + str(fcstring.count))
    plt.xlabel('p [kW]')
    plt.ylabel('total fc [g/H]')
    plt.legend()
    plt.grid(True)

    if outpath is None:
        plt.savefig(r'' + fcstring.fuel_cell.sourceFile.replace(".vfcm", "") + "-" + str(fcstring.count) + '.png')
    else:
        plt.savefig(outpath + Path(fcstring.fuel_cell.sourceFile).stem + "-" + str(fcstring.count) + '.png')
    if close:
        plt.close()


def print_fc_data(fc: FuelCellMap, close=True, outputPath: string=None):
    min_fc_power = fc.minPower
    max_fc_power = fc.maxPower
    fc1_eff = lambda x: x / fc.fc_of_p(x)
    x_values = np.arange(min_fc_power, max_fc_power, 0.1)
    time_slice_x_values = np.arange(0, max_fc_power, 0.1)
    y_values = fc.fc_of_p(x_values)
    y_time_sliced = fc.getFuelConsumption(time_slice_x_values)

    y_2_values = fc1_eff(x_values)

    y_3_values = 1 / y_2_values

    fig, ax1 = plt.subplots(figsize=(11.69,8.27))
    fig.subplots_adjust(right=0.75)

    ax1.plot(x_values, y_values, label='fc', color='blue')
    ax1.plot(time_slice_x_values, y_time_sliced, label='fc with time slicing', color='cyan', linestyle='--')
    ax1.scatter(x=[fc.min_eff_power], y=[fc.fc_of_p(fc.min_eff_power)], color="red")

    # plt.plot(x_values, y_2_values, label='Interpolated Function ^(-1)', color='green')
    # ax1.scatter(x_values, y_values, color='red', marker='o')

    twin1 = ax1.twinx()
    twin1.plot(x_values, y_2_values, label='efficiency', color='green')

    twin2 = ax1.twinx()
    twin2.plot(x_values, y_3_values, label='1/efficiency', color='pink')
    twin2.spines.right.set_position(("axes", 1.2))
    # plt.plot(x_values, y_2_values, label='Interpolated Function ^(-1)', color='green')
    # twin1.scatter(x_values, y_2_values, color='cyan', marker='o')

    plt.xlabel('p [kW]')
    ax1.set_ylabel('fc [g/h]')

    plt.title('Fuel cell map ' + fc.sourceFile)
    fig.legend()

    plt.grid(True)
    if outputPath is not None:
        plt.savefig(outputPath + Path(fc.sourceFile).stem + ".png")
    else:
        plt.savefig(fc.sourceFile.replace(".vfcm", ".png"))

    if close:
        plt.close()


def printFuelCellSystem(fcs: FuelCellSystem, close=True):
    # Create a grid of x_power and y_a values using meshgrid

    x_power = np.linspace(fcs.minPower(), fcs.maxPower(), 3000)
    y_a = np.linspace(0, 1, 100)
    X, Y = np.meshgrid(x_power, y_a)

    # Calculate the corresponding z values using the provided function
    Z = np.empty(X.shape)

    # Iterate over each element in the meshgrid and calculate the value using the function
    for i in range(X.shape[0]):
        for j in range(X.shape[1]):
            Z[i, j] = fcs.fc_consumption(X[i, j], Y[i, j])[0]

    # Create a 3D plot
    fig = plt.figure(figsize=(11.69,8.27))
    ax = fig.add_subplot(111, projection='3d')

    # Plot the surface
    surf = ax.plot_surface(X, Y, Z, cmap='viridis', alpha=0.9)



    # Add labels
    ax.set_xlabel('power')
    ax.set_ylabel('a')
    ax.set_zlabel('fc')
    ax.view_init(elev=48, azim=-143, roll=0)

    # Add a color bar which maps values to colors
    fig.colorbar(surf)

    add_eff_scatter(Y, Z, ax, x_power)


    #Show the plot

    plt.savefig(r'fuel_cell_maps/string.png')
    if close:
        plt.close()


def add_eff_scatter(Y, Z, ax, x_power):
    for i in range(0, x_power.shape[0]):
        # requested power
        x = x_power[i]

        minfc = np.nanmin(Z.T[i])
        indices = np.where(Z.T[i] == minfc)
        argmins = indices

        for j in range(argmins[0].shape[0]):
            min_a = Y[argmins[0][j], i]
            min_fc = Z[argmins[0][j], i]
            if np.isnan(min_fc):
                continue
            # Plot points to highlight the minimum values

            ax.scatter(x, min_a, min_fc, color='red' if (j == 0) else 'blue', marker='o', s=1, label='Minimum')


def printFuelCellSystemForPower(fcs: FuelCellSystem, power, outpath=None, close=True):
    get_fcs = lambda a, p: fcs.fc_consumption(power=p, a=a)
    a_array = np.linspace(0, 100, 5000) / 100
    y_values = np.array([get_fcs(a, power) for a in a_array])


    relevant_points = fcs.splits(power)
    relevant_fc = np.array([get_fcs(a, power) for a in relevant_points])


    fig = plt.figure(figsize=(11.69,8.27))
    ax = fig.add_subplot(111)
    plt.locator_params(axis='x', nbins=20)
    ax.plot(a_array, y_values)
    ax.scatter(relevant_points, relevant_fc[:,0])
    ax.scatter(relevant_points, relevant_fc[:,1])
    ax.scatter(relevant_points, relevant_fc[:,2])
    ax2 = ax.twiny()
    ax2.set_xticks(ax.get_xticks())
    ax2.set_xbound(ax.get_xbound())
    ax2.set_xticklabels([round(1 - x, 3) for x in ax.get_xticks()])
    ax.set_xticklabels([round(x, 3) for x in ax.get_xticks()])


    plt.figlegend(['fc_total', 'fc_string1(a*p)', 'fc_string2(b*p)'])
    plt.title("Fuel consumption " + str(power) + " kW")


    if outpath is None:
        plt.savefig(r'fuel_cell_maps/string' + str(power) + '_kW.png')
    else:
        plt.savefig(outpath + 'fuel_cell_system' + str(power) + "_kW.png")


    if close:
        plt.close()
