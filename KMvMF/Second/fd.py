"""
Schrodinger's Equation - PROBLEM #1
"""
import numpy
import matplotlib.pyplot
import scipy.integrate as spint
import math

def convert_angstrom_to_atomic_units(value):
    return value / 0.53


def convert_electronvolt_to_atomic_units(value):
    return value / 27.212

L = convert_angstrom_to_atomic_units(2.0)
V0 = convert_electronvolt_to_atomic_units(5.0)
A = -L
B = +L
A1 = 1.0
B1 = 1.25
# number of mesh node
n = 1000
# step of mesh
h = (B - A) / (n - 1)
# constant for Numerov's method
c = h ** 2 / 12.0
# minimum of potensial (atomic units)
U0 = -1.0
# maximum of potensial (atomic units) - for visualization only!
W = 3.0
# node of sewing
r = (n - 1) / 2 + 15
d1 = d2 = 1.e-9
dE = 0.09234
dU = 1.e-2

E_0 = E = U0 + dU

psi = numpy.zeros(n)
phi = numpy.zeros(n)
F = numpy.zeros(n)
X = numpy.linspace(A, B, n)
norm = numpy.zeros(n)
V_array = numpy.zeros(n)
E_array = numpy.zeros(6)
E_array_1 = numpy.zeros(6)
psi_array = numpy.zeros(shape=(6, n))

# potential function
def U(x):
    return V0 * (-(numpy.sin(numpy.pi * x / 2))) if abs(x) < L else W


# potential function vozmushenie
def U1(x):
    if (x >= A1) and (x <= B1):
        return 0.45
    else:
        return U(x)


def V(x):
    return U1(x) - U(x)


# function (0x)
def q(e, x):
    return 2.0 * (e - U(x))


# function (0x)
def q1(e, x):
    return 2.0 * (e - U1(x))


# function (0xx)
def deriv(Y, h, m):
    return (Y[m - 2] - Y[m + 2] + 8.0 * (Y[m + 1] - Y[m - 1])) / (12.0 * h)


def num(e, r, n, q):
    F = numpy.array([c * q(e, X[i]) for i in numpy.arange(n)])
    psi[0] = 0.0
    phi[n - 1] = 0.0
    psi[1] = d1
    phi[n - 2] = d2
    # Cauchy problem ("forward")
    for i in numpy.arange(1, n - 1, 1):
        p1 = 2.0 * (1.0 - 5.0 * F[i]) * psi[i]
        p2 = (1.0 + F[i - 1]) * psi[i - 1]
        psi[i + 1] = (p1 - p2) / (1.0 + F[i + 1]) # (16)
    for i in numpy.arange(n - 2, 0, -1):
        f1 = 2.0 * (1.0 - 5.0 * F[i]) * phi[i]
        f2 = (1.0 + F[i + 1]) * phi[i + 1]
        phi[i - 1] = (f1 - f2) / (1.0 + F[i - 1]) # (17)
    # search of maximum value of psi
    p1 = numpy.abs(psi).max()
    p2 = numpy.abs(psi).min()
    big = p1 if p1 > p2 else p2
    # scaling of psi
    for i in numpy.arange(0, n - 1, 1):
        psi[i] = psi[i] / big
    # mathematical scaling of phi for F[r]=psi[r]
    #print(psi[r], phi[r])
    coef = psi[r] / phi[r]
    for i in numpy.arange(0, n - 1, 1):
        phi[i] = coef * phi[i]
    # calculation of f(E) in node of sewing
    f = deriv(psi, h, r) - deriv(phi, h, r)
    return f


def bisection(a, b, func):
    eps = 1.e-3
    x = (a + b) / 2
    while numpy.abs(func(x)) >= eps:
        x = (a + b) / 2
        a, b = (a, x) if func(a) * func(x) < 0 else (x, b)
    return (a + b) / 2


# metod pristrelki + primenenie bisekcii dlya nevozmushennoi zadachi
def main_func(curr_E, dE):
    f_curr = num(curr_E, r, n, q)
    curr_E += dE
    f_next = num(curr_E, r, n, q)
    while f_curr * f_next > 0:
        curr_E += dE
        f_curr = f_next
        f_next = num(curr_E, r, n, q)

    funct = lambda e: num(e, r, n, q)

    E_f = bisection(curr_E - dE, curr_E, funct)
    return E_f


# metod pristrelki + primenenie bisekcii dlya vozmushennoi zadachi
def main_func_vozm(curr_E, dE):
    f_curr = num(curr_E, r, n, q1)
    curr_E += dE
    f_next = num(curr_E, r, n, q1)
    while f_curr * f_next > 0:
        curr_E += dE
        f_curr = f_next
        f_next = num(curr_E, r, n, q1)

    funct = lambda e: num(e, r, n, q1)

    E_f = bisection(curr_E - dE, curr_E, funct)
    return E_f


# Vychislenie pervogo priblizhenia volnovoi funkcii osnovnogo sostoyania vozmushennoi sistemy
def get_psi_vozm(e_array, phi_matrix, v_array):
    l = 0
    summm = numpy.zeros(n)
    for m in numpy.arange(0, 5, 1):
        if m != l:
            v_ml = spint.simps(numpy.multiply(numpy.multiply(phi_matrix[m], phi_matrix[l]), v_array), X)
            e = e_array[l] - e_array[m]
            tmp = v_ml / e
            summm = numpy.add(summm, tmp * phi_matrix[m])
    psi = numpy.add(phi_matrix[l], summm)
    return psi


# Vychislenie vtorogo priblizhenia energii osnovnogo sostoyania vozmush sistemy
def get_second_priblizh(e_first, e_array, phi_matrix, v_array):
    l = 0
    summm = 0
    for n in numpy.arange(0, 5, 1):
        if n != l:
            v_ln = spint.simps(numpy.multiply(numpy.multiply(phi_matrix[l], phi_matrix[n]), v_array), X)
            v_ln = numpy.power(v_ln, 2)
            e = e_array[l] - e_array[n]
            summm += v_ln / e
    return e_first + summm


def check_shodymost(e_array, phi_matrix, v_array):
    print("Check 47.12 (shodymost ryada posledovatelnych priblizhenii)")
    l = 0
    for m in numpy.arange(0, 6, 1):
        if m != l:
            v_lm = numpy.abs(spint.simps(numpy.multiply(numpy.multiply(phi_matrix[l], phi_matrix[m]), v_array), X))
            e = numpy.abs(e_array[l] - e_array[m])
            print(str(v_lm) + " << " + str(e))


k = 0
while k < 6:
    E_final = E_array[k] = main_func(E, dE)

    normcost = spint.simps(numpy.power(psi, 2), X)
    coef = 1.0/math.sqrt(normcost)
    for m in range(1, n-1):
        norm[m] = psi[m] * coef

    psi_array[k] = norm

    E = E_final + dE
    k += 1


for i in numpy.arange(0, n - 1, 1):
    V_array[i] = V(X[i])

E_1 = E_array[0] + spint.simps(numpy.multiply(numpy.power(psi_array[0], 2), V_array), X)
psi_vozm = get_psi_vozm(E_array, psi_array, V_array)

print("E[p1] = " + str(E_1))
print("E[p2] = " + str(get_second_priblizh(E_1, E_array, psi_array, V_array)))
print("E[sh] = " + str(main_func_vozm(E_0, dE)))
print
check_shodymost(E_array, psi_array, V_array)

matplotlib.pyplot.subplot(111)
matplotlib.pyplot.plot(X[1:n - 1], psi_vozm[1:n - 1], 'b', label="psi(x)")
matplotlib.pyplot.plot(X, [U1(X[i]) for i in numpy.arange(n)], 'k', linewidth=2.0, label="U(x)")
matplotlib.pyplot.grid(True)
ax = matplotlib.pyplot.subplot(111)
box = ax.get_position()
ax.set_position([box.x0, box.y0, box.width * 0.8, box.height])
matplotlib.pyplot.legend(loc='center left', bbox_to_anchor=(1, 0.5))

matplotlib.pyplot.show()
