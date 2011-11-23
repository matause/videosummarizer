//#define RATIO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


/*  SHOT DETECTION ALGORITHM
    1. Quantize 2^24 colors and build a histogram of 64 bins for each frame 
    2. Compute Color Histogram difference for every consecutive frames
       The difference measure is the sum of the absolute bin-wise histogram differences.
    3. Adaptive threshold value is computed for each frame based on the neighbouring frames on either side
    4. If the frame bin-wise difference is above threshold, compute the ratio Ds/Di to eliminate flash frames
    3. A shot boundary is declared if the ratio is less than the predefined flash frame ratio
*/
namespace VideoPlayer
{
    class Histogram
    {
        public List<int> framesMinWiseDifferences;
        public List<int> shots;
        public int MIN_WISE_DIFF = 1;
        public int SHOTS = 2;
        public int threshold = 5000;
        public float Tflash = 0.55f;
        public const int colorBinWidth = 64;
        public const int colorQuantGroups = 4;
        public Video videoRef;

        private const int MINIMUM_FRAMES_IN_SHOT = 150;    // 6 Seconds

        public void OnInitialize(Video video)
        {
            framesMinWiseDifferences = new List<int>();
            shots = new List<int>();
            videoRef = video;
        }

        public void GenerateColorHistogram(ref Frame frame)  // STEP 2
        {
            frame.values = new Int32[colorQuantGroups, colorQuantGroups, colorQuantGroups];

            for (int i = 0; i < frame.width; ++i)
            {
                for (int j = 0; j < frame.height; ++j)
                {
                    frame.values[frame.pixels[i][j].r / colorBinWidth, frame.pixels[i][j].g / colorBinWidth, frame.pixels[i][j].b / colorBinWidth]++;
                }
            }
        }

        public int SumOfBinWiseDiff(ref Frame frameCur, ref Frame frameLeft)
        {
            int sum = 0;

            GenerateColorHistogram(ref frameCur);
            GenerateColorHistogram(ref frameLeft);

            for (int i = 0; i < colorQuantGroups; ++i)
            {
                for (int j = 0; j < colorQuantGroups; ++j)
                {
                    for (int k = 0; k < colorQuantGroups; ++k)
                    {
                        sum += Math.Abs(frameCur.values[i, j, k] - frameLeft.values[i, j, k]);
                    }
                }
            }

            framesMinWiseDifferences.Add(sum);

            return sum;
        }

        // AUTOMATIC VIDEO CUT DETECTION USING ADAPTIVE THRESHOLDS ALGORITHM

        // Adaptive threshold value
        public int AdaptiveThreshold(int frameIndex)
        {
            int W = 72;     // Number of adjacent frames
            int sum = framesMinWiseDifferences[frameIndex];
            int result = 1;
            int gainFactor = 2;

            if (frameIndex > (W - 1))
            {
                for (int i = 1; i <= W; i++)
                    sum += framesMinWiseDifferences[frameIndex - i];

                result += W;
            }

            if (frameIndex < framesMinWiseDifferences.Count - W)
            {
                for (int i = 1; i <= W; i++)
                    sum += framesMinWiseDifferences[frameIndex + i];

                result += W;
            }

            if (result == 1)
                result = threshold;
            else
                result = gainFactor * (sum / result);

            return result;
        }

        // Too Lengthy computational time - need to optimize later (Skipping this step of the algorithm) 

#if RATIO
        // Compute ratio to decide between Flash frame or Shot boundary frame
        public float ComputeRatio(int frameIndex)
        {
            float ratio = 0f;
            int W = 2;
            float Ds = 0;
            Int32[, ,] hLeft = new Int32[colorQuantGroups, colorQuantGroups, colorQuantGroups];
            Int32[, ,] hRight = new Int32[colorQuantGroups, colorQuantGroups, colorQuantGroups];
            Frame frameLeft, frameRight;

            frameLeft = new Frame(1, videoRef.frameWidth, videoRef.frameHeight);
            frameRight = new Frame(2, videoRef.frameWidth, videoRef.frameHeight);

            // HLeft & HRight
            if (frameIndex > (W - 1) && frameIndex < (framesMinWiseDifferences.Count - W))
            {
                for (int i = 0; i < colorQuantGroups; ++i)
                {
                    for (int j = 0; j < colorQuantGroups; ++j)
                    {
                        for (int k = 0; k < colorQuantGroups; ++k)
                        {
                            for (int l = 1; l <= W; l++)
                            {
                                // Read the current shot selected
                                videoRef.videoReader.ReadFrame((frameIndex - l), ref frameLeft);
                                videoRef.videoReader.ReadFrame((frameIndex + l), ref frameRight);

                                GenerateColorHistogram(ref frameLeft);
                                GenerateColorHistogram(ref frameRight);

                                hLeft[i, j, k] += frameLeft.values[i, j, k];
                                hRight[i, j, k] += frameRight.values[i, j, k];
                            }

                            hLeft[i, j, k] /= W;
                            hRight[i, j, k] /= W;

                            Ds += Math.Abs(hLeft[i, j, k] - hRight[i, j, k]);
                        }
                    }
                }

                Ds *= 0.5f;

                ratio = Ds / framesMinWiseDifferences[frameIndex];
            }

            // GC
            frameLeft = frameRight = null;
            hLeft = hRight = null;

            return ratio;
        }
#endif

        // Detect the frame numbers at which new shots begin
        public void FindShotTransitions()
        {
            // First frame is the start of the first shot
            shots.Add(0);
                       
            for (int i = 1; i < framesMinWiseDifferences.Count; ++i)
            {
                threshold = AdaptiveThreshold(i);

                // Check for Abrupt Transition
                if (framesMinWiseDifferences[i] > threshold)
                {

#if RATIO
                    // Compute ratio to find if this is a shot boundary or flash
                    float ratio = ComputeRatio(i);

                    if (ratio < Tflash && ratio > 0)
                    {
#endif
                        if (shots[shots.Count - 1] < (i - MINIMUM_FRAMES_IN_SHOT))
                        {
                            // Check for shot similarity

                            // Read the current shot selected
                            Frame frameA;
                            frameA = new Frame(1, videoRef.frameWidth, videoRef.frameHeight);
                            videoRef.videoReader.ReadFrame(i, ref frameA);

                            // Read the Last shot that was selected
                            Frame frameB;
                            frameB = new Frame(2, videoRef.frameWidth, videoRef.frameHeight);
                            videoRef.videoReader.ReadFrame(shots[shots.Count - 1], ref frameB);

                            int sum = SumOfBinWiseDiff(ref frameB, ref frameA);

                            // Mark new shot if there exists a major difference between the previous and the current shot
                            if (sum > 40000)
                                shots.Add(i);

                            frameA = frameB = null;
                        }
                        else
                        {
                            // store Max(previous transition, current transition)
                            if (framesMinWiseDifferences[shots[shots.Count - 1]] < framesMinWiseDifferences[i])
                                shots[shots.Count - 1] = i;
                        }
#if RATIO
                    }
#endif
                }
            }
        }

        // Write the Min-wise differences into CSV file for analysis
        public void GenerateCSVFile(int analyse)
        {
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
                        if (shots.Contains(index))
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
