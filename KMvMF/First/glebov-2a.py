"""
One-dimensional Schrödinger steady-state equation.
Square potential well.
"""
import numpy as np
from scipy.integrate import odeint
from scipy.misc import derivative
from scipy.interpolate import interp1d
import matplotlib.pyplot as plt
import math
from scipy.integrate import quad
from scipy.special import erf

global r, n, Psi, Fi, X, XX
T = open("glebov-2b.txt", "w")


# potential function
def U(x):
    if (abs(x) < L):
        return V0 * erf(x)
    else:
        return W


# function (13)
def q(e, x):
    return 2.0*(e-U(x))


def operT(funF, X):
    localX = [X[0] - 2 * 1.e-6]
    for x in X:
        localX.append(x)
    localX.append(X[len(X) - 1] + 2 * 1.e-6)

    localFunF = [0]
    for val in funF:
        localFunF.append(val)
    localFunF.append(0)

    fun = interp1d(localX, localFunF, kind='cubic')
    der = []
    for x in X:
        der.append(-1/2 * derivative(fun, x, dx=1.e-6, n=2, order=5))
    return der

def system1(cond1, X):
    global eee
    Y0, Y1 = cond1[0], cond1[1]
    dY0dX = Y1
    dY1dX = - q(eee, X)*Y0
    return [dY0dX, dY1dX]


def system2(cond2, XX):
    global eee
    Z0, Z1 = cond2[0], cond2[1]
    dZ0dX = Z1
    dZ1dX = - q(eee, XX)*Z0
    return [dZ0dX, dZ1dX]


# calculation of f (eq. 18; difference of derivatives)
def f_fun(e):
    global r, n, Psi, Fi, X, XX, eee
    eee = e
    """
    Cauchy problem ("forward")
    dPsi1(x)/dx = - q(e, x)*Psi(x);
    dPsi(x)/dx = Psi1(x);
    Psi(A) = 0.0
    Psi1(A)= 1.0
    """
    cond1 = [0.0, 1.0]
    sol1 = odeint(system1, cond1, X)
    Psi, Psi1 = sol1[:, 0], sol1[:, 1]
    """
    Cauchy problem ("backwards")
    dPsi1(x)/dx = - q(e, x)*Psi(x);
    dPsi(x)/dx = Psi1(x);
    Psi(B) = 0.0
    Psi1(B)= 1.0
    """
    cond2 = [0.0, 1.0]
    sol2 = odeint(system2, cond2, XX)
    Fi, Fi1 = sol2[:, 0], sol2[:, 1]
    # search of maximum value of Psi
    p1 = np.abs(Psi).max()
    p2 = np.abs(Psi).min()
    big = p1 if p1 > p2 else p2
    # scaling of Psi
    Psi[:] = Psi[:]/big
    # mathematical scaling of Fi for F[rr]=Psi[r]
    coef = Psi[r]/Fi[rr]
    Fi[:] = coef * Fi[:]
    # calculation of f(E) in node of sewing
    curve1 = interp1d(X, Psi, kind='cubic')
    curve2 = interp1d(XX, Fi, kind='cubic')
    der1 = derivative(curve1, X[r], dx=1.e-6)
    der2 = derivative(curve2, XX[rr], dx=1.e-6)
    f = der1-der2
    return f

def average_value(psi, oper_value):
    global X
    value = []
    for ind in range(len(psi)):
        value.append(psi[ind] * oper_value[ind])

    fun = interp1d(X, value, kind='cubic')
    result = quad(fun, A, B)
    return result


def plotting_wf0(e):
    global r, n, Psi, Fi, X, XX
    ff = f_fun(e)
    print("f=", ff)
    plt.axis([A, B, -1, W])
    Upot = np.array([U(X[i]) for i in np.arange(n)])

    coefPsi = average_value(Psi, Psi)
    Psi[:] = Psi[:]/math.sqrt(coefPsi[0])
    normPsi = average_value(Psi, Psi)
    print("integrate Psi = ", normPsi[0])

    coefFi = average_value(Fi, Fi)
    Fi[:] = Fi[:] / math.sqrt(coefFi[0])
    normFi = average_value(Fi, Fi)
    print("integrate Fi = ", normFi[0])

    averageT = average_value(Psi, operT(Psi, X))
    print("<T> = ", averageT[0])

    UPsiValue = []
    for ind in range(len(X)):
        UPsiValue.append(Psi[ind] * U(X[ind]))
    averageU = average_value(Psi, UPsiValue)
    print("<U> = ", averageU[0])




    plt.plot(X, Upot, 'g-', linewidth=6.0, label="U(x)")
    Zero = np.zeros(n, dtype=float)
    plt.plot(X, Zero, 'k-', linewidth=1.0)  # abscissa axis
    plt.plot(X, Psi, 'r-', linewidth=2.0, label="Psi(x)")
    plt.plot(XX, Fi, 'b-', linewidth=2.0, label="Fi(x)")
    plt.xlabel("X", fontsize=18, color="k")
    plt.ylabel("Psi(x), Fi(x), U(x)", fontsize=18, color="k")
    plt.grid(True)
    plt.legend(fontsize=16, shadow=True, fancybox=True, loc='upper right')
    plt.plot([X[r]], [Psi[r]], color='red', marker='o', markersize=7)
    string1 = "E    = " + format(e, "10.7f")
    string2 = "f(E) = " + format(ff, "10.3e")
    plt.text(-1.5, 2.7, string1, fontsize=14, color='black')
    plt.text(-1.5, 2.3, string2, fontsize=14, color="black")
    # save to file
    name = "glebov-2a.pdf"
    plt.savefig(name, dpi=300)
    plt.show()


# initial data (atomic units)
V0 = 20 / 27.212
L = 2.0 / 0.5292
A = -L
B = +L
# number of mesh node
n = 1001  # odd integer number
print("n=", n)
print("n=", n, file=T)
# minimum of potential (atomic units)
U0 = 0
# maximum of potential (atomic units) - for visualization only!
W = 5.0
# x-coordinates of the nodes
X  = np.linspace(A, B, n)  # forward
XX = np.linspace(B, A, n)  # backwards
# node of sewing
r = (n-1)*3//4      # forward
rr = n-r-1          # backwards
print("r=", r)
print("r=", r, file=T)
print("rr=", rr)
print("rr=", rr, file=T)
print("X[r]=", X[r])
print("X[r]=", X[r], file=T)
print("XX[rr]=", XX[rr])
print("XX[rr]=", XX[rr], file=T)
# input of energy
e = float(input("Energy = "))
print("e =", e)
# plot
dummy = plotting_wf0(e)
