import Programmer
class Developer(Programmer):
    def __init__(self, developer_name, developer_surname, knowledgeArea, currentProject):
        super.__init__(self, developer_name, developer_surname)
        self._knowledgeArea = knowledgeArea
        self._currentProject = currentProject

    _knowledgeArea = ""
    _currentProject = ""

    def printConsole(self):
        print(self._currentProject,)
