"""
MTOP LAB
"""
import numpy as np
import matplotlib.pyplot as plt
import math 

f = lambda x, y: 2 * x * y
h = 0.001

ylist = [-4]
xlist = np.arange(2, 3, h)

for x in xlist:
    y = ylist[len(ylist) - 1]
    k1 = f(x, y)
    k2 = f(x + h / 2, y + h / 2 * k1)
    k3 = f(x + h / 2, y + h / 2 * k2)
    k4 = f(x + h, y + h * k3)
    ylist.append(y + h / 6 * (k1 + 2 * k2 + 2 * k3 + k4))

ylist.pop()

plt.plot(xlist, ylist, 'r-', linewidth=2.0, label="y(x)")
plt.xlabel("X", fontsize=16, color="b")
plt.ylabel("Y", fontsize=16, color="b")
plt.grid(True)
plt.legend(fontsize=14, shadow=True, fancybox=True)

plt.show()
