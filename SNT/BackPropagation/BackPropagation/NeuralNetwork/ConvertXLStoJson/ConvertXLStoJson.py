import xlrd
import json

arrayData = []

rb = xlrd.open_workbook('echocardiogram.data.xls',formatting_info=True)
sheet = rb.sheet_by_index(0)
for rownum in range(sheet.nrows - 1):
    row = sheet.row_values(rownum + 1)
    data = {}
    values = []
    targets = []
    flag = True
    for i in range(len(row)):
         if i == 1 :
             if type(row[i]) is float : target = row[i]
             else: flag = False
             targets.append(target)
         else:
             if type(row[i]) is float : value = row[i]
             else: flag = False
             values.append(value)
    if flag == True:
     data["Targets"] = targets                    
     data["Values"] = values
     arrayData.append(data)

trainData = []
testData = []

for z in range(int(round(len(arrayData) * 0.7))):
    trainData.append(arrayData[z])

for q   in range(len(arrayData) - z):
    testData.append(arrayData[q + z])

with open("dataTrain.txt", "w", encoding="utf-8") as file:       
   json.dump(trainData, file, ensure_ascii=False)
with open("dataTest.txt", "w", encoding="utf-8") as file:       
   json.dump(testData, file, ensure_ascii=False)

print(len(arrayData))

