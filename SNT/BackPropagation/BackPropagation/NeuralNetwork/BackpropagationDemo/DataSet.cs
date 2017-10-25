using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;


namespace BackpropagationDemo
{
    public class DataSet
    {
        #region -- Properties --
        public double[] Values { get; set; }
        public double[] Targets { get; set; }
        #endregion

        #region -- Constructor --
        public DataSet(double[] values, double[] targets)
        {
            Values = values;
            Targets = targets;
        }
        #endregion

        public static List<DataSet> ImportDatasets(string path)
        {
            using (var file = File.OpenText(path))
            {
                return JsonConvert.DeserializeObject<List<DataSet>>(file.ReadToEnd());
            }
        }
    }
}
