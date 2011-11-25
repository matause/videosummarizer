//#define RATIO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using AForge.Math;
using AForge.Imaging;

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
    public class AnalysisData
    {
        public int sum;
        public byte avgAmp;

        public AnalysisData()
        {
            sum = 0;
            avgAmp = 0;
        }
    }

    public class Shots
    {
        public int startFrame;
        public List<int> keyFrames;

        public Shots()
        {
            startFrame = 0;
            keyFrames = new List<int>();
        }
    }

    class Histogram
    {
        public List<AnalysisData> videoAnalysisData;
        public List<Shots> shots;
        public List<int> summaryFrames;
        public int MIN_WISE_DIFF = 1;
        public int AVG_AUDIO_AMPS = 2;
        public int SHOTS = 3;
        public int KEY_FRAMES = 4;
        public int VIDEO_SUMMARY = 5;
        public int threshold = 5000;
        public float Tflash = 0.55f;
        public const int colorBinWidth = 64;
        public const int colorQuantGroups = 4;
        public Video videoRef;
                                
        private const int MINIMUM_FRAMES_IN_SHOT = 150;    // 6 Seconds

        public void OnInitialize(Video video)
        {
            videoAnalysisData = new List<AnalysisData>();
            shots = new List<Shots>();
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

        public int FillAnalysisData(ref Frame frameCur, ref Frame frameLeft, bool add)
        {
            int sum = 0;
            int result = 0;

            GenerateColorHistogram(ref frameCur);
            GenerateColorHistogram(ref frameLeft);

            // Sum of Bin-wise absolute difference
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

            // Average amplitude of the audio data associated with the video frame
            float startTime = frameCur.index * videoRef.secondsPerFrame;
	        float endTime = (frameCur.index + 1) * videoRef.secondsPerFrame;
	        byte[] rawData = videoRef.audioPlayer.GetRawSoundData(startTime, endTime);
	
	        Int64 avgPerFrame = 0;

            for (int i = 0; i < rawData.Length; ++i)
            {
                avgPerFrame += rawData[i];
            }

            // Frame analysis data
            AnalysisData data = new AnalysisData();
            data.sum = sum;
            data.avgAmp = (byte)(avgPerFrame / rawData.Length);

            if(add == true)
                videoAnalysisData.Add(data);

            result = sum;

            return result;
        }

        // AUTOMATIC VIDEO CUT DETECTION USING ADAPTIVE THRESHOLDS ALGORITHM

        // Adaptive threshold value
        public int AdaptiveThreshold(int frameIndex)
        {
            int W = 72;     // Number of adjacent frames
            int sum = videoAnalysisData[frameIndex].sum;
            int result = 1;
            int gainFactor = 2;

            if (frameIndex > (W - 1))
            {
                for (int i = 1; i <= W; i++)
                    sum += videoAnalysisData[frameIndex - i].sum;

                result += W;
            }

            if (frameIndex < videoAnalysisData.Count - W)
            {
                for (int i = 1; i <= W; i++)
                    sum += videoAnalysisData[frameIndex + i].sum;

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
            if (frameIndex > (W - 1) && frameIndex < (videoAnalysisData.Count - W))
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

                ratio = Ds / videoAnalysisData[frameIndex].sum;
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
            Shots shot = new Shots();
            shot.startFrame = 0;
            shots.Add(shot);

            for (int i = 1; i < videoAnalysisData.Count; ++i)
            {
                threshold = AdaptiveThreshold(i);

                // Check for Abrupt Transition
                if (videoAnalysisData[i].sum > threshold)
                {

#if RATIO
                    // Compute ratio to find if this is a shot boundary or flash
                    float ratio = ComputeRatio(i);

                    if (ratio < Tflash && ratio > 0)
                    {
#endif
                        if (shots[shots.Count - 1].startFrame < (i - MINIMUM_FRAMES_IN_SHOT))
                        {
                            // Check for shot similarity

                            // Read the current shot selected
                            Frame frameA;
                            frameA = new Frame(i, videoRef.frameWidth, videoRef.frameHeight);
                            videoRef.videoReader.ReadFrame(i, ref frameA);

                            // Read the Last shot that was selected
                            Frame frameB;
                            frameB = new Frame(shots[shots.Count - 1].startFrame, videoRef.frameWidth, videoRef.frameHeight);
                            videoRef.videoReader.ReadFrame(shots[shots.Count - 1].startFrame, ref frameB);

                            int sum = FillAnalysisData(ref frameB, ref frameA, false);

                            // Mark new shot if there exists a major difference between the previous and the current shot
                            if (sum > 40000)
                            {
                                shots.Add(new Shots());
                                shots[shots.Count - 1].startFrame = i;
                            }

                            frameA = frameB = null;
                        }
                        else
                        {
                            // store Max(previous transition, current transition)
                            if (videoAnalysisData[shots[shots.Count - 1].startFrame].sum < videoAnalysisData[i].sum)
                            {
                                Shots s = shots[shots.Count - 1];
                                s.startFrame = i;
                            }
                        }
#if RATIO
                    }
#endif
                }
            }
        }

        // Audio excitation based key frame selection within a shot  
        public void FindKeyFrames()
        {
            // Histogram variables
    	    Int32[] audioAmps;
            AForge.Math.Histogram shotAudio;

            // N shots boundaries = N-1 shots 
            for (int i = 0; i < shots.Count-1; ++i)
            {
                // Sizeof number of frames in shot i
                audioAmps = new Int32[shots[i + 1].startFrame - shots[i].startFrame];

                int count = 0;
                int maxAmp = 0;
                int MaxKeyFrame = 0;
                for (int j = shots[i].startFrame; j < shots[i + 1].startFrame; ++j)
                {
                    audioAmps[count] = videoAnalysisData[j].avgAmp;

                    if (audioAmps[count] > maxAmp)
                    {
                        MaxKeyFrame = j;
                        maxAmp = audioAmps[count];
                    }

                    count++;
                }

                // Instantiate, used to find Mean, median, SD
                shotAudio = new AForge.Math.Histogram(audioAmps);

                shots[i].keyFrames.Add(MaxKeyFrame);
            }
        }

        public void GenerateSummaryVideo()
        {
            summaryFrames = new List<int>();
            int surrFrames = 72;

            // N shots boundaries = N-1 shots 
            for (int i = 0; i < shots.Count - 1; ++i)
            {
                // Fetch [3 seconds of video - key frame - 3 seconds of video]
                for (int j = 0; j < shots[i].keyFrames.Count; ++j)
                {
                    // X seconds of video before key frame
                    for (int k = surrFrames; k >= 1 ; --k)
                    {
                        if ((shots[i].keyFrames[j] - k) >= 0)
                            summaryFrames.Add(shots[i].keyFrames[j] - k);
                        else
                            continue;
                    }

                    // Key frame itself
                    summaryFrames.Add(shots[i].keyFrames[j]);

                    // X seconds of video after key frame
                    for (int k = 1; k < surrFrames; ++k)
                    {
                        if ((shots[i].keyFrames[j] + k) < videoAnalysisData.Count)
                            summaryFrames.Add(shots[i].keyFrames[j] + k);
                        else
                            break;
                    }
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

                    length = videoAnalysisData.Count;

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                        sb.AppendLine(string.Join(delimiter, videoAnalysisData[index].sum));

                    File.WriteAllText(filePath, sb.ToString());

                    break;

                case 2:
                    /*
                    array = new string[] { "@", System.AppDomain.CurrentDomain.BaseDirectory, "AvgAudioAmplitudes.csv" };
                    filePath = string.Join("", array);
	                
                    */
                    filePath = @"C:\AvgAudioAmplitudes.csv";

                    length = videoAnalysisData.Count;

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                        sb.AppendLine(string.Join(delimiter, videoAnalysisData[index].avgAmp));

                    File.WriteAllText(filePath, sb.ToString());

                    break;

                case 3:
                    /*
                    array = new string[] { "@", System.AppDomain.CurrentDomain.BaseDirectory, "ShotBoundaries.csv" };
                    filePath = string.Join("", array);
	                delimiter = ",";
                    */
                    filePath = @"C:\ShotBoundaries.csv";

                    length = videoAnalysisData.Count;

                    List<int> shotboundaries = new List<int>();
                    for(int i = 0; i < shots.Count; ++i)
                        shotboundaries.Add(shots[i].startFrame);

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                    {
                        if(shotboundaries.Contains(index))    
                            sb.AppendLine(string.Join(delimiter, videoAnalysisData[index].sum));
                        else
                            sb.AppendLine(string.Join(delimiter, 0));
                    }
                    File.WriteAllText(filePath, sb.ToString());

                    break;

                case 4:
                    /*
                    array = new string[] { "@", System.AppDomain.CurrentDomain.BaseDirectory, "KeyFrames.csv" };
                    filePath = string.Join("", array);
	                
                    */
                    filePath = @"C:\KeyFrames.csv";

                    length = videoAnalysisData.Count;

                    List<int> shotMaxAmps = new List<int>();
                    for(int i = 0; i < shots.Count-1; ++i)
                        shotMaxAmps.Add(shots[i].keyFrames[0]);

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                    {
                        if(shotMaxAmps.Contains(index))    
                            sb.AppendLine(string.Join(delimiter, videoAnalysisData[index].avgAmp));
                        else
                            sb.AppendLine(string.Join(delimiter, 0));
                    }
                    File.WriteAllText(filePath, sb.ToString());

                    break;

                case 5:
                    /*
                    array = new string[] { "@", System.AppDomain.CurrentDomain.BaseDirectory, "VideoSummary.csv" };
                    filePath = string.Join("", array);
	                delimiter = ",";
                    */
                    filePath = @"C:\VideoSummary.csv";

                    length = videoAnalysisData.Count;

                    List<int> videoSummary = new List<int>();
                    for (int i = 0; i < summaryFrames.Count; ++i)
                        videoSummary.Add(summaryFrames[i]);

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                    {
                        if (videoSummary.Contains(index))
                            sb.AppendLine(string.Join(delimiter, videoAnalysisData[index].sum));
                        else
                            sb.AppendLine(string.Join(delimiter, 0));
                    }
                    File.WriteAllText(filePath, sb.ToString());

                    break;
            }
        }
    }
}
