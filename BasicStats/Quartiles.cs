using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BasicStats
{
    class Quartiles : OneDimBase
    {
        //http://en.wikipedia.org/wiki/Quartile

        //////////////////////////////////////
        // Public properties
        public float[] Method1Quartiles
        {
            get { CalculateValues(); return _Method1Quartiles; }
        }
        public float[] Method2Quartiles
        {
            get { CalculateValues(); return _Method2Quartiles; }
        }
        public float[] Method3Quartiles
        {
            get { CalculateValues(); return _Method3Quartiles; }
        }

        /// <summary>
        /// Use when console operation is to be done
        /// </summary>
        public Quartiles()
        {
        }
        /// <summary>
        /// Use when only result is needed
        /// </summary>
        public Quartiles(String Input): 
            base(Input)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="SortedInput"></param>
        public Quartiles(float[] Input, Boolean bSortedInput = false) :
            base(Input, bSortedInput)
        {
        }

        /// <summary>
        /// Take input from user and work on that
        /// </summary>
        /// <returns></returns>
        public override bool DriveConsole()
        {
            if (!base.DriveConsole())
                return false;

            Console.WriteLine();
            Console.WriteLine("Method 1: " + ToStringQuartiles(_Method1Quartiles));
            Console.WriteLine("Method 2: " + ToStringQuartiles(_Method2Quartiles));
            Console.WriteLine("Method 3: " + ToStringQuartiles(_Method3Quartiles));

            return true;
        }

        /// <summary>
        /// Converts to string format assuming  valid data is passed.
        /// </summary>
        /// <param name="QuartileArray">Non null & valid 3 element array. No error check</param>
        /// <returns></returns>
        public String ToStringQuartiles(float[] QuartileArray)
        {
            String temp = new StringBuilder("").AppendFormat("Q1. {0} Q2. {1} Q3. {2}",
                QuartileArray[0], QuartileArray[1], QuartileArray[2]).ToString();
             
            return temp;
        }

        ///////////////////////////////////////////////////
        // Override methods
        protected override void Init()
        {
            base.Init();
            MinDataLength = 4;
            Operation = "Quartiles";
        }
        protected override void showConsoleMenuOptions()
        {
            base.showConsoleMenuOptions();
        }

        
        protected override void CalculateValues()
        {
            CheckDataValidity();

            Int32 datalength = mSortedData.Length;
            Int32 halfwaymark = datalength / 2;
            float[] lowerhalf = new float[halfwaymark];
            float[] upperhalf = new float[halfwaymark];

            //Method 1 : employed by the TI-83 calculator boxplot and "1-Var Stats" functions
            // Use the median to divide the ordered data set into two halves. Do not include the median in either half.
            // The lower quartile value is the median of the lower half of the data. 
            // The upper quartile value is the median of the upper half of the data.
            Array.Copy(mSortedData, 0, lowerhalf, 0, halfwaymark);
            Array.Copy(mSortedData, (0 == (datalength % 2) ? halfwaymark : halfwaymark + 1), upperhalf, 0, halfwaymark);
            updateQuartileValue(ref _Method1Quartiles, lowerhalf, upperhalf);

            //Method 2 : "Tukey's hinges"
            // Use the median to divide the ordered data set into two halves. 
            // If the median is a datum (as opposed to being the mean of the middle two data), include the median in both halves.
            // The lower quartile value is the median of the lower half of the data. 
            // The upper quartile value is the median of the upper half of the data.
            int size = (0 == (datalength % 2)) ? halfwaymark : (halfwaymark + 1);
            Array.Copy(mSortedData, 0, (lowerhalf = new float[size]), 0, size);
            Array.Copy(mSortedData, halfwaymark, (upperhalf = new float[size]), 0, size);
            updateQuartileValue(ref _Method2Quartiles, lowerhalf, upperhalf);

            //Method 3 : 
            // If there are an even number of data points, then the method is the same as above.
            // (4n+1) data points, 
            //         lower quartile is 25% of the nth data value plus 75% of the (n+1)th data value; 
            //         upper quartile is 75% of the (3n+1)th data point plus 25% of the (3n+2)th data point.
            // (4n+3) data points, 
            //         lower quartile is 75% of the (n+1)th data value plus 25% of the (n+2)th data value; 
            //         upper quartile is 25% of the (3n+2)th data point plus 75% of the (3n+3)th data point.
            // This always gives the arithmetic mean of Methods 1 and 2; it ensures that the median value is given its correct weight, 
            //  and thus quartile values change as smoothly as possible as additional data points are added.
            _Method3Quartiles[1] = new Median(mSortedData).Value;
            if (1 == (datalength % 4))
            {
                int baseunit = (datalength - 1) / 4;
                _Method3Quartiles[0] = ((float)mSortedData[baseunit-1] * 0.25f) + ((float)mSortedData[baseunit] * 0.75f);
                _Method3Quartiles[2] = ((float)mSortedData[3 * baseunit] * 0.75f) + ((float)mSortedData[(3 * baseunit) + 1] * 0.25f);
            }
            else if (3 == (datalength % 4))
            {
                int baseunit = (datalength - 3) / 4;
                _Method3Quartiles[0] = ((float)mSortedData[baseunit] * 0.75f) + ((float)mSortedData[baseunit + 1] * 0.25f);
                _Method3Quartiles[2] = ((float)mSortedData[(3 * baseunit) + 1] * 0.25f) + ((float)mSortedData[(3 * baseunit) + 2] * 0.75f);
            } 
            else
            {
                Array.Copy(_Method3Quartiles, 0, _Method1Quartiles, 0, datalength);
            }
            return;
        }

        ////////////////////////////////////////////////////
        // Private Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="QuartileArray"></param>
        /// <param name="LowerHalf"></param>
        /// <param name="UpperHalf"></param>
        private void updateQuartileValue(ref float[] QuartileArray, float[] LowerHalf, float[] UpperHalf)
        {
            QuartileArray[0] = new Median(LowerHalf, true).Value;
            QuartileArray[1] = new Median(mSortedData, true).Value;
            QuartileArray[2] = new Median(UpperHalf, true).Value;
        }

        //////////////////////////////////////////////////
        // Private Members
        private float[] _Method1Quartiles = new float[3];
        private float[] _Method2Quartiles = new float[3];
        private float[] _Method3Quartiles = new float[3];
    }
}
