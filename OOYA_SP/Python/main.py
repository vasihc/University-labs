from Programmer import Programmer
from Developer import Developer
from Manager import Manager
from ITCompany import ITCompany

company = ITCompany.__new__(ITCompany)
company = company.__read__()
company.__sort__()
company.__print__()
company.__del__()
