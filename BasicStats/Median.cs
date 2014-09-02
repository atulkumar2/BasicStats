using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BasicStats
{
    class Median: OneDimBase
    {
        /// <summary>
        /// 
        /// </summary>
        public Median()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Input"></param>
        public Median(String Input) :
            base(Input)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Input">Sorted or unsorted array</param>
        /// <param name="Sorted">No validation for input for sorting if this is true</param>
        public Median(float[] Input, Boolean bSortedInput=false):
            base(Input, bSortedInput)
        {
        }

        /// <summary>
        /// Take Data from user and show information
        /// </summary>
        /// <returns></returns>
        public override bool DriveConsole()
        {
            if (!base.DriveConsole())
                return false;

            Console.WriteLine();
            Console.WriteLine("Median: " + _Value.ToString());

            return true;
        }
        
        ///////////////////////////////////////////////////
        // Override methods
        protected override void Init()
        {
            base.Init();
            MinDataLength = 1;
            Operation = "Median";
        }
        protected override void showConsoleMenuOptions()
        {
            base.showConsoleMenuOptions();
        }
        protected override void CalculateValues()
        {
            CheckDataValidity();

            if (1 == mSortedData.Length % 2)
                _Value = mSortedData[(mSortedData.Length - 1) / 2];
            else
                _Value = (mSortedData[(mSortedData.Length - 1) / 2] + mSortedData[mSortedData.Length / 2]) / 2f;
        }

        ////////////////////////////////
        // Private Methods

        //////////////////////////////////////////////////
        // Private Members
    }
}
