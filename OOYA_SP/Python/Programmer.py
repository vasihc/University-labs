from Position import Position
class Programmer: #Базовый класс
    def __init__(self, nm, srnm, pos, skls):
        self._name = nm
        self._surname = srnm
        self._position = pos
        self._skills = skls

    def getName(self):  #иммитация публичного свойства, на самом деле все свойства в питоне паблик
        return self._name

    def setName(self, val):# set метод может быть вызван классом наследником
         self._name = val

    def __print__(self):
        print("Programmer", self._name + ' has skills:' + self._skills)




