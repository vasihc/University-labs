"""
One-dimensional Schrödinger steady-state equation.
Square potential well.
"""
import numpy as np
from scipy.integrate import odeint
from scipy.misc import derivative
from scipy.interpolate import interp1d
import matplotlib.pyplot as plt
from scipy.special import erf

global r, n, Psi, Fi, X, XX

LST = open("glebov-2b.txt", "wt")


# potential function
def U(x):
    if (abs(x) < L):
        return V0 * erf(x)
    else:
        return W


# function (13)
def q(e, x):
    print("e = ", e, ", x = ", x)
    return 2.0*(e-U(x))

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


def m_bis(x1, x2, tol):
    global r, n
    if f_fun(e=x2)*f_fun(e=x1) > 0.0:
        print("ERROR: f_fun(e=x2, r, n)*f_fun(e=x1, r, n) > 0")
        print("x1=", x1)
        print("x2=", x2)
        print("f_fun(e=x1, r=r, n=n)=", f_fun(e=x1))
        print("f_fun(e=x2, r=r, n=n)=", f_fun(e=x2))
        exit()
    while abs(x2-x1) > tol:
        xr = (x1+x2)/2.0
        if f_fun(e=x2)*f_fun(e=xr) < 0.0:
            x1 = xr
        else:
            x2 = xr
        if f_fun(e=x1)*f_fun(e=xr) < 0.0:
            x2 = xr
        else:
            x1 = xr
    return (x1+x2)/2.0


def plotting_f():
    plt.axis([U0, e2, fmin, fmax])
    ZeroE = np.zeros(ne, dtype=float)
    plt.plot(ee, ZeroE, 'k-', linewidth=1.0)  # abscissa axis
    plt.plot(ee, af, 'bo', markersize=2)
    plt.xlabel("E", fontsize=18, color="k")
    plt.ylabel("f(E)", fontsize=18, color="k")
    plt.grid(True)
    # save to file
    plt.savefig('glebov-2b-f.pdf', dpi=300)
    plt.show()


def plotting_wf(e):
    global r, n, Psi, Fi, X, XX
    ff = f_fun(e)
    plt.axis([A, B, -1, W])
    Upot = np.array([U(X[i]) for i in np.arange(n)])
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
    name = "glebov-2b" + "-" + str(ngr) + ".pdf"
    plt.savefig(name, dpi=300)
 #   plt.show()

# initial data (atomic units)
V0 = 20 / 27.212
L = 2.0 / 0.5292
A = -L
B = +L
# number of mesh node
n = 1001  # odd integer number
print("n=", n)
print("n=", n, file=LST)
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
print("r=", r, file=LST)
print("rr=", rr)
print("rr=", rr, file=LST)
print("X[r]=", X[r])
print("X[r]=", X[r], file=LST)
print("XX[rr]=", XX[rr])
print("XX[rr]=", XX[rr], file=LST)
# plot of f(e)
e1 = U0+0.05
e2 = 10.0
print("e1=", e1, "   e2=", e2)
print("e1=", e1, "   e2=", e2, file=LST)
ne = 101
print("ne=", ne)
print("ne=", ne, file=LST)
ee = np.linspace(e1, e2, ne)
af = np.zeros(ne, dtype=float)
porog = 5.0
tol = 1.0e-7
energy = []
ngr = 0
for i in np.arange(ne):
    e = ee[i]
    af[i] = f_fun(e)
    stroka = "i = {:3d}   e = {:8.5f}  f[e] = {:12.5e}"
    print(stroka.format(i, e, af[i]))
    print(stroka.format(i, e, af[i]), file=LST)
    if i > 0:
        Log1 = af[i]*af[i-1] < 0.0
        Log2 = np.abs(af[i]-af[i-1]) < porog
        if Log1 and Log2:
            energy1 = ee[i-1]
            energy2 = ee[i]
            eval = m_bis(energy1, energy2, tol)
            print("eval = {:12.5e}".format(eval))
            dummy = plotting_wf(eval)
            energy.append(eval)
            ngr += 1

fmax = +10.0
fmin = -10.0
# plot
dummy = plotting_f()
# output of roots
nroots = len(energy)
print("nroots =", nroots)
print("nroots =", nroots, file=LST)
for i in np.arange(nroots):
    stroka = "i = {:1d}    energy[i] = {:12.5e}"
    print(stroka.format(i, energy[i]))
    print(stroka.format(i, energy[i]), file=LST)
