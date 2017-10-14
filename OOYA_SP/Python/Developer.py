from Programmer import Programmer
class Developer(Programmer):
    def __init__(self, nm, srnm, pos, skls, knowledgeArea, currentProject):
        super().__init__(nm, srnm, pos, skls)
        self._knowledgeArea = knowledgeArea
        self._currentProject = currentProject

    def __print__(self):
        print(self._name, self._surname, "is ", self._position, "of ", self._knowledgeArea, " have skils:", ", ".join([str(x) for x in self._skills] ))

