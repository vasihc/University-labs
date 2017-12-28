import numpy as np
from scipy.integrate import odeint, simps
from scipy.misc import derivative
from scipy.interpolate import interp1d
import matplotlib.pyplot as plt
import math
from scipy.integrate import quad
from scipy.special import erf

def convert_angstrom_to_atomic_units(value):
    return value / 0.53


def convert_electronvolt_to_atomic_units(value):
    return value / 27.212

class Pristrelki_method:
    # initial data (atomic units)
    L = convert_angstrom_to_atomic_units(2.0)
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
#-----------------end class method pristrelki-----------------------------


#------------------data initial---------------------------------------------
V0 = convert_electronvolt_to_atomic_units(20)
L = convert_angstrom_to_atomic_units(2.0)
W = 5.0
A = -L
B = +L
n = 1001
X = np.linspace(A, B, n)  # forward
count_phi = 100
N1 = 5
N2 = 8
N3 = 15
phi_values = []
Temp = []
#--------------------end data initial----------------------------------------

def fun_U(x): #potential function
    if (abs(x) < L):
        return V0 * erf(x)
    else:
        return W

def srednee(psi_m, oper_value, psi_n):
    value = []
    if oper_value is None:
        for ind in range(len(psi_m)):
            value.append(psi_m[ind] * psi_n[ind])
    else:
        for ind in range(len(psi_m)):
            value.append(psi_m[ind] * oper_value[ind] * psi_n[ind])

    result = simps(value, X)#Integrate y(x) using samples along the given axis and the composite
                            #Simpson's rule.  If x is None, spacing of dx is assumed.
    return result

def calcT(funF, X):
    localX = [X[0] - 4 * 1.e-6]
    localX.append(X[0] - 2 * 1.e-6)
    for x in X:
        localX.append(x)
    localX.append(X[len(X) - 1] + 2 * 1.e-6)
    localX.append(X[len(X) - 1] + 4 * 1.e-6)

    der = []
    for x in X:
        der.append(-1/2 * derivative(funF, x, dx=1.e-6, n=2, order=5))
    return der

def phi_even(k):
    return lambda x: 1 / math.sqrt(L) * math.sin(math.pi * k * x / (2 * L))

def phi_odd(k):
    return lambda x: 1 / math.sqrt(L) * math.cos(math.pi * k * x / (2 * L))

#target of functions in point k
def get_phi_k_value(k):
    if k % 2 == 0:
        fun = phi_even(k)
    else:
        fun = phi_odd(k)

    result = []
    for x in X:
        result.append(fun(x))

    return result

#functions
def get_phi_k(k):
    if k % 2 == 0:
        return phi_even(k)
    else:
        return phi_odd(k)

def get_matrix_H(N):
    value_U = np.array([fun_U(X[i]) for i in np.arange(n)])
    matrix = np.zeros((N, N))
    for i in range(N):
        for j in range(N):
            new_value = 0
            if i == j:
                new_value += math.pow(math.pi * (i + 1) / L, 2) / 8
            new_value += srednee(phi_values[i], value_U, phi_values[j])
            matrix[i][j] = new_value
    return matrix

def get_psi(c):
    result = []
    for i in range(len(X)):
        value = 0
        for j in range(len(c)):
            value += c[j] * phi_values[j][i]
        result.append(value)

    coefPsi = srednee(result, None, result)
    for i in range(len(result)):
        result[i] /= math.sqrt(coefPsi)
    return result

def plot(U, psi1, psi2, psi3, psi):
    plt.axis([A, B, -1, W])
    plt.plot(X, U, 'g-', linewidth=5.0, label="U(x)")
    Zero = np.zeros(n, dtype=float)
    plt.plot(X, Zero, 'k-', linewidth=1.0)  # abscissa axis
    plt.plot(X, psi, 'b-', linewidth=8.0, label="'$\psi$'")
    plt.plot(X, psi1, 'y-', linewidth=5.0, label="'$\psi$1'")
    plt.plot(X, psi2, 'm-', linewidth=2.5, label="'$\psi$2'")
    plt.plot(X, psi3, 'g-', linewidth=0.5, label="'$\psi$3'")
    plt.xlabel("X", fontsize=18, color="k")
    plt.ylabel("U(x), Psi(x)", fontsize=18, color="k")
    plt.grid(True)
    plt.legend(fontsize=16, shadow=True, fancybox=True, loc='upper right')
    plt.show()

for i in range(count_phi):
    phi_values.append(get_phi_k_value(i + 1))



#-----------------main-----------


#------------------First compare-----------
H = get_matrix_H(N1)#calc matrix H for N1 count

e, c = np.linalg.eig(H)
E0 = e.min()
for i in range(len(e)):
    if e[i] == E0:
        min_ind = i
        break

coef_c = []
for i in range(len(c)):
    coef_c.append(c[i][min_ind])
if (coef_c[0] < 0):
    coef_c = np.dot(coef_c, -1)

psi1 = get_psi(coef_c)  #calc psi1
#------------------end first----------------

#--------------Second-------------------
H = get_matrix_H(N2)
e, c = np.linalg.eig(H)
E02 = e.min()
for i in range(len(e)):
    if e[i] == E02:
        min_ind = i
        break

coef_c = []
for i in range(len(c)):
    coef_c.append(c[i][min_ind])

if coef_c[0] < 0:
    coef_c = np.dot(coef_c, -1)
psi2 = get_psi(coef_c)
#--------------end second------------------


#-------------third--------------------
H = get_matrix_H(N3)
e, c = np.linalg.eig(H)
E03 = e.min()
for i in range(len(e)):
    if e[i] == E03:
        min_ind = i
        break

coef_c = []
for i in range(len(c)):
    coef_c.append(c[i][min_ind])

if coef_c[0] < 0:
    coef_c = np.dot(coef_c, -1)
psi3 = get_psi(coef_c)
#--------------end third-------------------

method_pristrelki_U = Pristrelki_method(fun_U, U0=-4, ne=101, e2=15, count_e=1)
energy_U, psi_U = method_pristrelki_U.get_energy()

#---------write results--------------------
stroka = " E0 = {:12.5e}"
print("Using pristrelka method ")
print(stroka.format(energy_U[0]))
print()
print("Using variacionnij method")
print( stroka.format(E0), "( N = ", N1, ")")
print( stroka.format(E02), "( N = ", N2, ")")
print(stroka.format(E03), "( N = ", N3, ")")



#----------plot graph----------------------
value_U = np.array([fun_U(X[i]) for i in np.arange(n)])
plot(value_U, psi1, psi2, psi3, psi_U[0])

