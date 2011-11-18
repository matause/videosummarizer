using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/*  SHOT DETECTION ALGORITHM
    1. A threshold is used. 
    2. We compute a 256-bin Y channel histogram over the entire frame.
    3. The difference measure is the sum of the absolute bin-wise histogram differences.
    4. A shot boundary is declared if the histogram difference between consecutive frames exceeds a threshold.
*/
namespace VideoPlayer
{
    class Histogram
    {
        public void GenerateHistogram(ref Frame frame)  // STEP 2
        {
            frame.values = new Int32[256];

            for (int i = 0; i < frame.width; ++i)
            {
                for (int j = 0; j < frame.height; ++j)
                {
                    byte y = frame.pixels[i][j].y;
                    frame.values[y]++;
                }
            }
        }

        public int SumOfBinWiseDiff(ref Frame frameA, ref Frame frameB)
        {
            int sum = 0;

            GenerateHistogram(ref frameA);
            GenerateHistogram(ref frameB);

            for (int i = 0; i < frameA.values.Length; ++i)
            {
                sum += Math.Abs(frameA.values[i] - frameB.values[i]);
            }

            // GC values
            //frameA.values = null;
            //frameB.values = null;

            return sum;
        }

        public void GenerateCSVFile(List<int> framesMinWiseDifferences)
        {
            string filePath = @"C:\ShotDetection.csv";  
	        string delimiter = ",";

            //file = new FileStream(filePath, FileMode.Create);

            int length = framesMinWiseDifferences.Count;

	        StringBuilder sb = new StringBuilder();  
	        for (int index = 0; index < length; index++)
                sb.AppendLine(string.Join(delimiter, framesMinWiseDifferences[index]));  
	 
	        File.WriteAllText(filePath, sb.ToString()); 
        }
    }
}
