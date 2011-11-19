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
        public List<int> framesMinWiseDifferences;
        public List<int> shots;
        public int MIN_WISE_DIFF = 1;
        public int SHOTS = 2;

        private const int MINIMUM_FRAMES_IN_SHOT = 100;    // I have used this value without no specific reason, have to tune it later

        public void OnInitialize()
        {
            framesMinWiseDifferences = new List<int>();
            shots = new List<int>();
        }

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

            framesMinWiseDifferences.Add(sum);

            // GC values
            frameA.values = null;
            frameB.values = null;

            return sum;
        }

        // GREEDY TECHNIQUE

        // Detect the frame numbers at which new shots begin
        public void FindShotTransitions()
        {
            // Start first shot with first frame
            shots.Add(0);

            for (int i = 1; i < framesMinWiseDifferences.Count; ++i)
            {
                // Check for Abrupt Transition
                if ((framesMinWiseDifferences[i] - framesMinWiseDifferences[i - 1]) > 25000)
                {
                    // Save New shot if the previous shot detected was MINIMUM_FRAMES_IN_SHOT no. of frames beyond the current frame 
                    if (shots[shots.Count - 1] < (i - MINIMUM_FRAMES_IN_SHOT))
                    {
                        shots.Add(i);
                    }
                    else
                    {
                        // store Max(previous transition, current transition)
                        if( framesMinWiseDifferences[shots[shots.Count - 1]] < framesMinWiseDifferences[i] )
                            shots[shots.Count - 1] = i;
                    }
                }
            }
        }

        // Write the Min-wise differences into CSV file for analysis
        public void GenerateCSVFile(int analyse)
        {
//            string[] array;
            string filePath;
            string delimiter = ",";
            int length;
            StringBuilder sb;

            switch (analyse)
            {
                case 1:
                    /*
                    array = new string[] { "@", System.AppDomain.CurrentDomain.BaseDirectory, "MinWiseDifferences.csv" };
                    filePath = string.Join("", array);
	                
                    */
                    filePath = @"C:\MinWiseDifferences.csv";
                    
                    length = framesMinWiseDifferences.Count;

	                sb = new StringBuilder();  
	                for (int index = 0; index < length; index++)
                        sb.AppendLine(string.Join(delimiter, framesMinWiseDifferences[index]));  
	 
	                File.WriteAllText(filePath, sb.ToString());

                    break;

                case 2:
                    /*
                    array = new string[] { "@", System.AppDomain.CurrentDomain.BaseDirectory, "ShotBoundaries.csv" };
                    filePath = string.Join("", array);
	                delimiter = ",";
                    */
                    filePath = @"C:\ShotBoundaries.csv";

                    length = framesMinWiseDifferences.Count;

	                sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                    {
                        if(shots.Contains(index))
                            sb.AppendLine(string.Join(delimiter, framesMinWiseDifferences[index]));
                        else
                            sb.AppendLine(string.Join(delimiter, 0));
                    }
	                File.WriteAllText(filePath, sb.ToString());

                    break;
            }
        }
    } 
}
