import Position

class Programmer:
    def __init__(self, programmer_name, programmer_surname):
        self.name = programmer_name
        self.surname = programmer_surname

    _name = ""
    _surname = ""
    _position = Position.Position.JUNIOR
    _skills = []
    def getName(self):
        return self._name

    def setName(self, val):
         self._name = val






