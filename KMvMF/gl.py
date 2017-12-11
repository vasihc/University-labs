import numpy as np
from scipy.integrate import odeint
from scipy.misc import derivative
from scipy.interpolate import interp1d
import matplotlib.pyplot as plt
from scipy.integrate import quad
from numpy.specify import erf
import math

class Method_bis:
    # initial data (atomic units)
    L = 2.0 / 0.5292
    A = -L
    B = +L
    # number of mesh node
    n = 1001  # odd integer number

    def __init__(self, fun_U, U0, ne, e2, count_e):
        self.U = fun_U
        self.U0 = U0
        self.ne = ne
        self.e2 = e2

        self.X = np.linspace(self.A, self.B, self.n)  # forward
        self.XX = np.linspace(self.B, self.A, self.n)  # backwards
        self.r = (self.n - 1) * 3 // 4  # forward
        self.rr = self.n - self.r - 1
        self.e1 = self.U0 + 0.05
        self.count_e = count_e


        # function (13)
    def q(self, e, x):
        return 2.0 * (e - self.U(x))

    def system1(self, cond1, X):
        Y0, Y1 = cond1[0], cond1[1]
        dY0dX = Y1
        dY1dX = - self.q(self.eee, X) * Y0
        return [dY0dX, dY1dX]

    def system2(self, cond2, XX):
        Z0, Z1 = cond2[0], cond2[1]
        dZ0dX = Z1
        dZ1dX = - self.q(self.eee, XX) * Z0
        return [dZ0dX, dZ1dX]

    def average_value(self, psi, oper_value):
        value = []
        for ind in range(len(psi)):
            value.append(psi[ind] * oper_value[ind])

        fun = interp1d(self.X, value, kind='cubic')
        result = quad(fun, self.A, self.B)
        return result

    # calculation of f (eq. 18; difference of derivatives)
    def f_fun(self, e):
        self.eee = e
        """
        Cauchy problem ("forward")
        dPsi1(x)/dx = - q(e, x)*Psi(x);
        dPsi(x)/dx = Psi1(x);
        Psi(A) = 0.0
        Psi1(A)= 1.0
        """
        cond1 = [0.0, 1.0]
        sol1 = odeint(self.system1, cond1, self.X)
        self.Psi = sol1[:, 0]
        """
        Cauchy problem ("backwards")
        dPsi1(x)/dx = - q(e, x)*Psi(x);
        dPsi(x)/dx = Psi1(x);
        Psi(B) = 0.0
        Psi1(B)= 1.0
        """
        cond2 = [0.0, 1.0]
        sol2 = odeint(self.system2, cond2, self.XX)
        self.Fi = sol2[:, 0]
        # search of maximum value of Psi
        p1 = np.abs(self.Psi).max()
        p2 = np.abs(self.Psi).min()
        big = p1 if p1 > p2 else p2
        # scaling of Psi
        self.Psi[:] = self.Psi[:] / big
        # mathematical scaling of Fi for F[rr]=Psi[r]
        coef = self.Psi[self.r] / self.Fi[self.rr]
        self.Fi[:] = coef * self.Fi[:]
        # calculation of f(E) in node of sewing
        curve1 = interp1d(self.X, self.Psi, kind='cubic')
        curve2 = interp1d(self.XX, self.Fi, kind='cubic')
        der1 = derivative(curve1, self.X[self.r], dx=1.e-6)
        der2 = derivative(curve2, self.XX[self.rr], dx=1.e-6)
        f = der1 - der2
        return f

    def m_bis(self, x1, x2, tol):
        if self.f_fun(e=x2) * self.f_fun(e=x1) > 0.0:
            print("ERROR: f_fun(e=x2, r, n)*f_fun(e=x1, r, n) > 0")
            print("x1=", x1)
            print("x2=", x2)
            print("f_fun(e=x1, r=r, n=n)=", self.f_fun(e=x1))
            print("f_fun(e=x2, r=r, n=n)=", self.f_fun(e=x2))
            exit()
        while abs(x2 - x1) > tol:
            xr = (x1 + x2) / 2.0
            if self.f_fun(e=x2) * self.f_fun(e=xr) < 0.0:
                x1 = xr
            else:
                x2 = xr
            if self.f_fun(e=x1) * self.f_fun(e=xr) < 0.0:
                x2 = xr
            else:
                x1 = xr
        return (x1 + x2) / 2.0

    def get_energy(self):
        ee = np.linspace(self.e1, self.e2, self.ne)
        af = np.zeros(self.ne, dtype=float)
        porog = 5.0
        tol = 1.0e-7
        energy = []
        fun_psi = []
        ngr = 0
        for i in np.arange(self.ne):
            e = ee[i]
            af[i] = self.f_fun(e)
            if i > 0:
                Log1 = af[i] * af[i - 1] < 0.0
                Log2 = np.abs(af[i] - af[i - 1]) < porog
                if Log1 and Log2:
                    energy1 = ee[i - 1]
                    energy2 = ee[i]
                    eval = self.m_bis(energy1, energy2, tol)
                    energy.append(eval)
                    coefPsi = self.average_value(self.Psi, self.Psi)
                    self.Psi[:] = self.Psi[:] / math.sqrt(coefPsi[0])
                    normPsi = self.average_value(self.Psi, self.Psi)
                    if (normPsi[0] - 1 > 0.000001):
                        print("Error! integrate Psi = ", normPsi[0])
                        return None
                    fun_psi.append(self.Psi)
                    ngr += 1
                    if ngr == self.count_e:
                        break
        return energy, fun_psi



V0 = 20 / 27.212
L = 2.0 / 0.5292
W = 5.0
L = 2.0 / 0.5292
A = -L
B = +L
n = 1001
X = np.linspace(A, B, n)  # forward
def fun_U_0(x):
    if (abs(x) < L):
        return V0 * erf(f)
    else:
        return W

def fun_U(x):
    if -0.7 < x < -0.5:
        return fun_U_0(x) + 0.7
    else:
        return fun_U_0(x)

def fun_V(x):
    return fun_U(x) - fun_U_0(x)

def get_value_V(psi_l, oper_value, psi_m):
    value = []
    for ind in range(len(oper_value)):
        value.append(psi_l[ind] * oper_value[ind] * psi_m[ind])

    fun = interp1d(X, value, kind='cubic')
    result = quad(fun, A, B)
    return result[0]

def plot(U0, U, ksi1, ksi2):
    plt.axis([A, B, -1, W])
    plt.plot(X, U0, 'g-', linewidth=5.0, label="U0(x)")
    plt.plot(X, U, 'b-', linewidth=1.0, label="U(x)")
    Zero = np.zeros(n, dtype=float)
    plt.plot(X, Zero, 'k-', linewidth=1.0)  # abscissa axis
    plt.plot(X, ksi1, 'y-', linewidth=5.0, label="Ksi1(x)")
    plt.plot(X, ksi2, 'r-', linewidth=1.0, label="Ksi2(x)")
    plt.xlabel("X", fontsize=18, color="k")
    plt.ylabel("U0(x), U(x), Ksi1(x), Ksi2(x)", fontsize=18, color="k")
    plt.grid(True)
    plt.legend(fontsize=16, shadow=True, fancybox=True, loc='upper right')
    plt.show()

method_bis_U0 = Method_bis(fun_U_0, U0=0, ne=101, e2=15, count_e=10)
energy_U0, psi_U0 = method_bis_U0.get_energy()
for i in np.arange(len(energy_U0)):
    stroka = "i = {:1d}    energy[i] = {:12.5e}"
    print(stroka.format(i, energy_U0[i]))

value_fun_V = []
for x in X:
    value_fun_V.append(fun_V(x))

V = []
for i in range(len(energy_U0)):
    V.append([])
    for j in range(len(energy_U0)):
        new_value = get_value_V(psi_U0[i], value_fun_V, psi_U0[j])
        V[i].append(new_value)
        #if i != j:
            #print("Vlm = ", abs(new_value), " El - Em = ", abs(energy_U0[i] - energy_U0[j]))

summa = 0
for i in range(1, len(energy_U0)):
    summa += V[0][i] * V[0][i] / (energy_U0[0] - energy_U0[i])

e0 = energy_U0[0] + V[0][0] + summa
ksi0 = []
for i in range(len(psi_U0[i])):
    sum = 0
    for j in range(1, len(energy_U0)):
        sum += V[0][j] / (energy_U0[0] - energy_U0[j]) * psi_U0[j][i]
    ksi0.append(psi_U0[0][i] + sum)

method_bis_U = Method_bis(fun_U, U0=0, ne=101, e2=15, count_e=1)
energy_U, psi_U = method_bis_U.get_energy()

stroka = "Энергия основного состояния полученная с помощью теории возмущения energy = {:12.5e}"
print(stroka.format(e0))
stroka = "Энергия основного состояния полученная с метода пристрелки energy = {:12.5e}"
print(stroka.format(energy_U[0]))

value_U0 = np.array([fun_U_0(X[i]) for i in np.arange(n)])
value_U = np.array([fun_U(X[i]) for i in np.arange(n)])
plot(value_U0, value_U, psi_U[0], ksi0)


