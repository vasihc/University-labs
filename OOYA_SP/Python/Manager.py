import Programmer

class Manager(Programmer):
    def __init__(self, developer_name, developer_surname, subordinatesQuantity, projects):
        super.__init__(self, developer_name, developer_surname)
        self._subordinatesQuantity = subordinatesQuantity
        self._projects = projects

    _subordinatesQuantity = 0
    _projects = []

    