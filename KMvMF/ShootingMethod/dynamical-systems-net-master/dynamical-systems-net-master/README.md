Dynamical Systems Net
=====================

A set of base classes and solver for simulating a distributed, graphical, dynamical system 
of differential equations. Includes a sparse, variable-indexed vector class, and one 4th Runga-Kutta solver. 
An abstract interface for solvers exists to enable other numerical implementations. Variable dependencies
are expressed as weighted connections in the graph, making this suitable for dynamical machine learning models
as well as simpler ODE models. Written in C# and targeting .NET Framework 4.0 (using System.Numerics).

Base Classes
------------
The base class design is composed principally of 
<code>DynamicalNode</code>, 
<code>DynamicalSystem</code>, 
<code>IIntegrator</code>, 
<code>NodeLink</code>, 
and <code>VectorOI</code>. 

To define your derivative <code>dy_i/dt</code> you simply subclass and implement
virtual method <code>DynamicalNode.F(t, y)</code>, and instantiate it with name e.g. "y_i". 
Dependencies on other variables are explicitly definable 
via access to the mutable list of incoming node edges.

This convention assumes your system of equations is of the form 
<code>{ dy_i/dt = F(t, y, ext) } | i:[1,n]</code> 
where <code>dy_i/dt</code> is
the rate of change of one variable <code>y_i</code>, 
<code>y</code> is the state of all variables in the system, 
<code>n</code> is the number of variables in the system,
<code>t</code> is the current time,
and <code>ext</code> is some external state.

Example Project
---------------
The examples project includes:
+ A sine-wave generator system.
+ A linear-coupled two variable oscillating system.
+ Two (2) gradient-frequency neural network (GFNN) systems.
+ A Hebbian-learning example, forming connection weights atop of a GFNN system.

Original Motivation
-------------------
This project comes from efforts toward recreating a cognitive model of musical perception 
and tonality learning, for course CSE 258A at U.C. San Diego. The final report on this effort
is available below.

[Exploring a Neurological Model for Cross-Cultural Consonance and Dissonance in Music Perception: CSE 258A Project Final Report]
(http://www.scribd.com/doc/148918615/Exploring-a-Neurological-Model-for-Cross-Cultural-Consonance-and-Dissonance-in-Music-Perception-CSE-258A-Project-Final-Report)
