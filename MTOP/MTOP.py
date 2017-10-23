"""
MTOP LAB
"""
import numpy as np
import matplotlib.pyplot as plt
import math

f = lambda x, y: 2 * x * y
h = 0.001

ylist = [1]
xlist = np.arange(0, 2, h)

for x in xlist:
    y = ylist[len(ylist) - 1]
    k1 = f(x, y)
    k2 = f(x + h / 2, y + h / 2 * k1)
    k3 = f(x + h / 2, y + h / 2 * k2)
    k4 = f(x + h, y + h * k3)
    ylist.append(y + h / 6 * (k1 + 2 * k2 + 2 * k3 + k4))

ylist.pop()

plt.plot(xlist, ylist, 'r-', linewidth=2.0, label="y(x)")
plt.xlabel("X", fontsize=26, color="k")
plt.ylabel("Y", fontsize=26, color="k")
plt.grid(True)
plt.legend(fontsize=16, shadow=True, fancybox=True)

plt.show()
