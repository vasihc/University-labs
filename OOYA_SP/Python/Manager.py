from Programmer import Programmer

class Manager(Programmer): #наследуемый класс
    def __init__(self, nm, srnm, pos, skls, subordinatesQuantity, projects):
        self._subordinatesQuantity = subordinatesQuantity
        self._projects = projects
        super().__init__(nm, srnm, pos, skls)

    def __print__(self):#Переопределенный метод базового класса
        print(self._name, self._surname, "is ", self._position, "have ", self._subordinatesQuantity, ". Projects: ",  ", ".join([str(x) for x in self._projects] ))