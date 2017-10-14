from Position import Position
class Programmer:
    def __init__(self, nm, srnm, pos, skls):
        self._name = nm
        self._surname = srnm
        self._position = pos
        self._skills = skls

    def getName(self):
        return self._name

    def setName(self, val):
         self._name = val

    def __print__(self):
        print("Programmer", self._name + ' has skills:' + self._skills)




