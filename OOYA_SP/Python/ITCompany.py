from Position import Position
from Developer import Developer
from Manager import Manager

class ITCompany:
    def __new__(cls):
        return super(ITCompany, cls).__new__(cls)

    def __init__(self, name, programmers):
        self._name = name
        self._programmers = programmers

    def __del__(self):
        print("Company deleted")

    def __print__(self):
        print(self._name)
        for person in self._programmers :
            person.__print__();

    def getPosition(self, str):
        if str == "JUNIOR" :
            return Position.JUNIOR
        elif  str == "MIDDLE" :
                return Position.MIDDLE
        else : return Position.SENIOR;


    def __read__(self):
        f = open('input.txt', 'r')
        name = f.readline().replace('\n','')
        quant = int(f.readline())
        f.readline()
        programmers = []

        for i  in range(quant) :
            type = str(f.readline()).replace('\n','')
            nm = f.readline().replace('\n','')
            srnm = f.readline().replace('\n','')
            pos = self.getPosition(f.readline())
            skls = f.readline().split()
            if type == "Developer" :
                knwldg = f.readline().replace('\n','')
                prjcts = f.readline().split()
                dev = Developer(nm, srnm, pos, skls, knwldg, prjcts)
                programmers.append(dev)
            else:
                prjcts = f.readline().split()
                quant = int(f.readline())
                manager = Manager(nm, srnm,pos, skls, quant, prjcts)
                programmers.append(manager)

        cm = ITCompany.__new__(ITCompany)
        cm.__init__(name, programmers)
        return cm

    def __sort__(self):
        sorted(self._programmers, key=lambda p : (p._position, p._name, p._surname))

    def __removeNotOdd__(self):
        programmers = []
        for programmer in self._programmers :
             if type(programmer) is Developer :
                 if int(programmer._currentProject[0][len(programmer._currentProject[0]) - 1]) % 2 !=0 :
                     programmers.append(programmer)
             else :
                 programmers.append(programmer)
        self._programmers = programmers








