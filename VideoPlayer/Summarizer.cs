//#define RATIO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using AForge.Math;
using AForge.Imaging;

/*  COLOR HISTOGRAM BASED SHOT BOUNDARY DETECTION ALGORITHM
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
            // Color Histogram  - Absolute difference between frame A & B
            // Motion Vector  - Minimum motion variation between frame A & B
            sum = 0;

            // Audio Amplitude
            avgAmp = 0;
        }
    }

    public class Shot
    {
        public int startFrame;
        public int weight;
        public List<int> keyFrames;
        public List<int> sortedAudioAmps;

        public Shot()
        {
            startFrame = 0;
            sortedAudioAmps = new List<int>();
            keyFrames = new List<int>();
            weight = 0;
        }
    }


    public class shotSort : IComparable<shotSort>
    {
        public int shotIndex;
        public int maxAudioAmp;

        public shotSort()
        {
            shotIndex = 0;
            maxAudioAmp = 0;
        }

        public int CompareTo(shotSort obj)
        {
            if (maxAudioAmp > obj.maxAudioAmp)
                return -1; //normally greater than = 1
            if (maxAudioAmp < obj.maxAudioAmp)
                return 1; // normally smaller than = -1
            else
                return 0; // equal
        }
    }

    public class SortIntDescending : IComparer<int>
    {
        int IComparer<int>.Compare(int a, int b) //implement Compare
        {
            if (a > b)
                return -1; //normally greater than = 1
            if (a < b)
                return 1; // normally smaller than = -1
            else
                return 0; // equal
        }
    }

    public class Coord
    {
        public int x;
        public int y;

        public Coord()
        {
            x = 0;
            y = 0;
        }
    }

    public class ColorErrorSum
    {
        public int colError;
        public Coord index;
        
        public ColorErrorSum(int err, int x, int y)
        {
            index = new Coord();
            colError = err;
            index.x = x;
            index.y = y;
        }
    }

    public class BestMatchDistance
    {
        public int sum;

        public BestMatchDistance(int value)
        {
            sum = value;
        }
    }

    class Summarizer
    {
        public List<AnalysisData> videoAnalysisData;
        public List<Shot> shots;
        public List<shotSort> sortedShots;
        public List<int> summaryFrames;
        public int threshold = 5000;
        public float Tflash = 0.55f;
        public const int colorBinWidth = 64;
        public const int colorQuantGroups = 4;
        public Video videoRef;

        // CSV IDS
        public int MIN_WISE_DIFF = 1;
        public int AVG_AUDIO_AMPS = 2;
        public int SHOTS = 3;
        public int KEY_FRAMES = 4;
        public int VIDEO_SUMMARY = 5;
        public int MOTION_VECTOR_BEST_MATCH = 6;

        private const int MINIMUM_FRAMES_IN_SHOT = 150;    // 6 Seconds

        // Motion vector
        public int frameStep = 24;
        public int macroBlockSideLen = 32;
        public int scanWindowSideLen = 20;
        public Coord blockCount;
        public Coord macroBlocksStart;
        List<BestMatchDistance> BestMatchdata;

        public void OnInitialize(Video video)
        {
            videoAnalysisData = new List<AnalysisData>();
            shots = new List<Shot>();
            videoRef = video;

            // Motion
            BestMatchdata = new List<BestMatchDistance>();
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

        // Color Difference
        public double ColorDifference(ref Pixel A, ref Pixel B)
        {
            return Math.Sqrt( Math.Pow((double)(B.r - A.r), 2.0) + Math.Pow((double)(B.g - A.g), 2.0) + Math.Pow((double)(B.b - A.b), 2.0) );
        }

        // Motion vector distance between best match and current macroblock
        public double MotionVectorDistance(ref Coord A, ref Coord B)
        {
            return Math.Sqrt( Math.Pow((double)(B.x - A.x), 2.0) + Math.Pow((double)(B.y - A.y), 2.0) );
        }

        // Find best motion vector for the current macroblock using the error values in color
        public Coord BestBlockMatchScan(ref Frame frameCur, ref Frame frameLeft ,ref Coord blockCur)
        {
            List<ColorErrorSum> err = new List<ColorErrorSum>();

            Coord scanWindow = new Coord();
            scanWindow.x = ( macroBlocksStart.x + blockCur.x * macroBlockSideLen ) - scanWindowSideLen;
            scanWindow.y = ( macroBlocksStart.y + blockCur.y * macroBlockSideLen ) - scanWindowSideLen;

            // For all pixels in the current macroBlockSideLen block
            Coord curStart = new Coord();
            curStart.x = (macroBlocksStart.x + blockCur.x * macroBlockSideLen);
            curStart.y = (macroBlocksStart.y + blockCur.y * macroBlockSideLen);

            // Compare with all possible matchings in the scan window around the current block
            for (int windowi = scanWindow.x; windowi < scanWindow.x + ( 2 * scanWindowSideLen ); ++windowi)
            {
                for (int windowj = scanWindow.y; windowj < scanWindow.y + ( 2 * scanWindowSideLen ); ++windowj)
                {
                    // [ windowi, windowj ] - Current scan window macroblock start

                    int row = -1;
                    int col = -1;
                    int sum = 0;
                    for (int i = curStart.x; i < curStart.x + macroBlockSideLen; ++i)
                    {
                        ++col;
                        for (int j = curStart.y; j < curStart.y + macroBlockSideLen; ++j)
                        {
                            // [ i,j ] - Current macroblock pixel to be compared with respective window current macro block pixel
                            ++row;

                            Pixel curP = frameCur.pixels[i][j];
                            Pixel prevP = frameLeft.pixels[windowi + col][windowj + row];

                            // Color error sum for all pixels of macroblock
                            sum += (int)ColorDifference(ref curP, ref prevP);
                        }
                        row = -1;
                    }

                    // Stores error sum values of all the possible matchings
                    err.Add(new ColorErrorSum(sum, windowi, windowj) );

                }
            }

            // Find the minimum error sum and associated index of the window macro block which resulted in the minimum error sum 
            int min = err[0].colError;
            int index = 0;
            for (int i = 1; i < err.Count; i++)
            {
                if (err[i].colError < min)
                {
                    min = err[i].colError;
                    index = i;
                }
            }

            // Start index of the best matching window macro block
            return err[index].index;
        }

        public int FillAnalysisData(ref Frame frameCur, ref Frame frameLeft, bool add)
        {
            int sum = 0;
            int result = 0;
            
            if (videoRef.colorHistogramAlgorithm)
            {
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
            }
            else if (videoRef.motionVectorAlgorithm)
            {
                double bestMatchDistance = 0.0;

                blockCount = new Coord();
                blockCount.x = 8;
                blockCount.y = 6;

                macroBlocksStart = new Coord();
                macroBlocksStart.x = (videoRef.frameWidth - (macroBlockSideLen * blockCount.x)) / 2;
                macroBlocksStart.y = (videoRef.frameHeight - (macroBlockSideLen * blockCount.y)) / 2;
 
                // frame n + 1 Macroblock best match from regions in frame n macroblock scan window
                for (int i = 0; i < blockCount.x; ++i)
                {
                    for (int j = 0; j < blockCount.y; ++j)
                    {
                        Coord blockCur = new Coord();
                        blockCur.x = i;
                        blockCur.y = j;

                        Coord motionVector = new Coord();
                        motionVector = BestBlockMatchScan(ref frameCur, ref frameLeft, ref blockCur);

                        // For all pixels in the current block
                        Coord currentCenter = new Coord();
                        currentCenter.x = (macroBlocksStart.x + blockCur.x * macroBlockSideLen) + macroBlockSideLen / 2;
                        currentCenter.y = (macroBlocksStart.y + blockCur.y * macroBlockSideLen) + macroBlockSideLen / 2;

                        Coord bestMatchCenter = new Coord();
                        bestMatchCenter.x = motionVector.x + macroBlockSideLen / 2;
                        bestMatchCenter.y = motionVector.y + macroBlockSideLen / 2;

                        bestMatchDistance += MotionVectorDistance(ref currentCenter, ref bestMatchCenter);
                    }
                }

                // Sum of the motion vector magnitudes of all macroblocks 
                BestMatchdata.Add(new BestMatchDistance((int)bestMatchDistance));
            }

            // Average amplitude of the audio data associated with the video frame
            float startTime = frameCur.index * Video.secondsPerFrame;
            float endTime = (frameCur.index + 1) * Video.secondsPerFrame;
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

            if (add == true)
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
            Shot shot = new Shot();
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
                            shots.Add(new Shot());
                            shots[shots.Count - 1].startFrame = i;
                        }

                        frameA = frameB = null;
                    }
                    else
                    {
                        // store Max(previous transition, current transition)
                        if (videoAnalysisData[shots[shots.Count - 1].startFrame].sum < videoAnalysisData[i].sum)
                        {
                            Shot s = shots[shots.Count - 1];
                            s.startFrame = i;
                        }
                    }
#if RATIO
                    }
#endif
                }
            }
        }

        public void SortAudioAmpsInShots()
        {
            // N shots boundaries = N-1 shots 
            for (int i = 0; i < shots.Count - 1; ++i)
            {
                // Fetch audio Avg amplitudes for all the frames in shot i
                for (int j = shots[i].startFrame; j < shots[i + 1].startFrame; ++j)
                {
                    shots[i].sortedAudioAmps.Add(videoAnalysisData[j].avgAmp);
                }

                // Sort in descending order
                shots[i].sortedAudioAmps.Sort(new SortIntDescending());
            }
        }

        public void SortMaxAudioAmpOfShots()
        {
            sortedShots = new List<shotSort>();

            // N shots boundaries = N-1 shots 
            for (int i = 0; i < shots.Count - 1; ++i)
            {
                // First value in each sorted list of amplitudes of a shot is the max value 
                shotSort s = new shotSort();
                s.maxAudioAmp = shots[i].sortedAudioAmps[0];
                s.shotIndex = i;
                sortedShots.Add(s);
            }

            // Sort in descending order
            sortedShots.Sort();
        }

        public void AssignWeightToShots()
        {
            int index = 0;

            // N shots boundaries = N-1 shots
            index = 0;
            for (int i = sortedShots.Count - 1; i >= 1; --i)
            {
                shots[sortedShots[index].shotIndex].weight = i;
                index++;
            }
        }

        // Audio excitation based key frame selection within a shot  
        public void FindKeyFrames()
        {
            // Histogram variables
            Int32[] audioAmps;
            AForge.Math.Histogram shotAudio;

            // Sort all the shots in descending order
            SortAudioAmpsInShots();

            // Sort Max amplitude of all the shots
            SortMaxAudioAmpOfShots();

            // Assign weight to shots based on sorted list
            AssignWeightToShots();

            // N shots boundaries = N-1 shots
            for (int i = 0; i < shots.Count - 1; ++i)
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

                if (!shots[i].keyFrames.Contains(MaxKeyFrame))
                    shots[i].keyFrames.Add(MaxKeyFrame);
            }
        }

        public void GenerateSummaryVideo(int sceneTime, int percentage)
        {
            summaryFrames = new List<int>();
            int surrFrames = (int)( sceneTime / 2.0 * 24 );
            int minWeight = (int)( ( 100 - percentage ) / 100.0 * shots.Count );

            // N shots boundaries = N-1 shots
            for (int i = 0; i < shots.Count - 1; ++i)
            {
                if (shots[i].weight < minWeight)
                    continue;

                // Fetch sceneTime/2 seconds of video before the key frame
                for (int j = 0; j < shots[i].keyFrames.Count; ++j)
                {
                    // X seconds of video before key frame
                    for (int k = surrFrames; k >= 1; --k)
                    {
                        if ((shots[i].keyFrames[j] - k) >= 0)
                        {
                            if (!summaryFrames.Contains(shots[i].keyFrames[j] - k))
                                summaryFrames.Add(shots[i].keyFrames[j] - k);
                        }
                        else
                            continue;
                    }

                    // Key frame itself
                    summaryFrames.Add(shots[i].keyFrames[j]);

                    // Fetch sceneTime/2 seconds of video after the key frame
                    for (int k = 1; k < surrFrames; ++k)
                    {
                        if ((shots[i].keyFrames[j] + k) < videoAnalysisData.Count)
                        {
                            if (!summaryFrames.Contains(shots[i].keyFrames[j] + k))
                                summaryFrames.Add(shots[i].keyFrames[j] + k);
                        }
                        else
                            break;
                    }
                }
            }
        }

        // Write the Min-wise differences into CSV file for analysis
        public void GenerateCSVFile(int analyze, string path)
        {
            string filePath;
            string delimiter = ",";
            int length;
            StringBuilder sb;

            switch (analyze)
            {
                case 1:
                    filePath = path + "\\MinWiseDifferences.csv";

                    length = videoAnalysisData.Count;

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                        sb.AppendLine(string.Join(delimiter, videoAnalysisData[index].sum));

                    File.WriteAllText(filePath, sb.ToString());

                    break;

                case 2:
                    filePath = path + "\\AvgAudioAmplitudes.csv";

                    length = videoAnalysisData.Count;

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                        sb.AppendLine(string.Join(delimiter, videoAnalysisData[index].avgAmp));

                    File.WriteAllText(filePath, sb.ToString());

                    break;

                case 3:
                    filePath = path + "\\ShotBoundaries.csv";

                    length = videoAnalysisData.Count;

                    List<int> shotboundaries = new List<int>();
                    for (int i = 0; i < shots.Count; ++i)
                        shotboundaries.Add(shots[i].startFrame);

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                    {
                        if (shotboundaries.Contains(index))
                            sb.AppendLine(string.Join(delimiter, videoAnalysisData[index].sum));
                        else
                            sb.AppendLine(string.Join(delimiter, 0));
                    }
                    File.WriteAllText(filePath, sb.ToString());

                    break;

                case 4:
                    filePath = path + "\\KeyFrames.csv";

                    length = videoAnalysisData.Count;

                    List<int> shotMaxAmps = new List<int>();
                    for (int i = 0; i < shots.Count - 1; ++i)
                        shotMaxAmps.Add(shots[i].keyFrames[0]);

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                    {
                        if (shotMaxAmps.Contains(index))
                            sb.AppendLine(string.Join(delimiter, videoAnalysisData[index].avgAmp));
                        else
                            sb.AppendLine(string.Join(delimiter, 0));
                    }
                    File.WriteAllText(filePath, sb.ToString());

                    break;

                case 5:
                    filePath = path + "\\VideoSummary.csv";

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

                case 6:
                    filePath = path + "\\BestMatchDistMagSum.csv";

                    length = BestMatchdata.Count;

                    sb = new StringBuilder();
                    for (int index = 0; index < length; index++)
                        sb.AppendLine(string.Join(delimiter, BestMatchdata[index].sum));

                    File.WriteAllText(filePath, sb.ToString());

                    break;

            }
        }
    }
}
