using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BasicStats
{
    class OneDimBase
    {
        public float Value
        {
            get { CalculateValues(); return _Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public OneDimBase()
        {
            Init();
        }

        /// <summary>
        /// Used when string data fed programmatically
        /// </summary>
        /// <param name="Input"></param>
        public OneDimBase(String Input)
        {
            Init();
            if (String.IsNullOrEmpty(Input))
                throw new InvalidOperationException(ERR_WrongDataInput);
            mRawStrInput = Input;
        }

        /// <summary>
        /// Used when integer array is inputted
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="bSortedInput">if true, no validation is done</param>
        public OneDimBase(float[] Input, Boolean bSortedInput = false)
        {
            Init();
            if (null == Input || 0 == Input.Length)
                throw new InvalidOperationException(ERR_WrongDataInput);
            if (MinDataLength > Input.Length)
                throw new InvalidOperationException(new StringBuilder("").AppendFormat(ERR_FORM_MinLength, MinDataLength).ToString());

            if (!bSortedInput)
            {
                mRawIntInput = new float[Input.Length];
                Array.Copy(Input, mRawIntInput, Input.Length);
                Array.Sort(mRawIntInput);
            }

            mSortedData = new float[Input.Length];
            Array.Copy((bSortedInput?Input:mRawIntInput), mSortedData, Input.Length);

            bValidInput = true;
        }

        ////////////////////////////////////
        // Virtual interface
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool DriveConsole()
        {
            Console.WriteLine();
            showConsoleMenuOptions();
            String Input = Console.ReadLine().Trim();
            if (Input.Length == 0) return false;
            mRawStrInput = Input;

            if (!ProcessAndSortStrInput())
            {
                Console.WriteLine(ERR_WrongDataEntry);
                return false;
            }
            
            CheckDataValidity();
            Console.WriteLine(Environment.NewLine + "Unnumbered Sorted List:" + ToString());
            Console.WriteLine("Numbered Sorted List:" + ToString(true));

            Console.Write("Calculate {0}? (Y/N): ", Operation);
            Input = Console.ReadLine().Trim();
            if (0 == Input.Length || !Input.ToLower().StartsWith("y")) return true;

            CalculateValues();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void showConsoleMenuOptions()
        {
            Console.WriteLine("Calculate {0}. Enter comma separated list. CTL+C will exit." +
                                " Entering other than comma or numbers will exit." +
                                " Entering End\\Exit\\E\\Quit\\Q will take you up." +
                                " Wrong data will exit.", Operation);
            Console.Write(" Enter Data : ");
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool ProcessAndSortStrInput()
        {
            char[] delimiter = InputDelimiter.ToCharArray();
            String[] strTokens = mRawStrInput.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (strTokens.Length == 0) return false;

            mSortedData = new float[strTokens.Length];
            for (int count = 0; count < strTokens.Length; count++)
            {
                if (!float.TryParse(strTokens[count], out mSortedData[count]))
                {
                    Array.Clear(mSortedData, 0, mSortedData.Length);
                    return false;
                }
            }
            Array.Sort(mSortedData);

            return (bValidInput = true);
        }

        /// <summary>
        /// Displays the list in string format
        /// </summary>
        /// <param name="NumberedList">Numbered list preference</param>
        /// <returns></returns>
        public virtual String ToString(Boolean NumberedList = false)
        {
            if (!bValidInput) return null;

            String Temp = "";
            for (int count = 0; count < (mSortedData.Length - 1); count++)
            {
                if (NumberedList)
                    Temp += (count + 1).ToString() + "." + mSortedData[count].ToString() + " ";
                else
                    Temp += mSortedData[count].ToString() + ", ";
            }
            if (NumberedList)
                Temp += mSortedData.Length.ToString() + ".";

            return (Temp + mSortedData[mSortedData.Length - 1].ToString());
        }

        protected virtual void CalculateValues() { }

        /// <summary>
        /// Not to be used for partial validity check.
        /// </summary>
        protected virtual void CheckDataValidity()
        {
            if (!bValidInput)
                throw new InvalidOperationException(ERR_BadObjectState);

            if (MinDataLength > mSortedData.Length)
                throw new InvalidOperationException(new StringBuilder("").AppendFormat(ERR_FORM_MinLength, MinDataLength).ToString());

            for (int count=0; count < mSortedData.Length - 1; count++)
            {
                if (mSortedData[count] > mSortedData[count + 1])
                    throw new InvalidOperationException(ERR_UnsortedObject);
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Init()
        {
            bTraceData = Tracing.dataSwitch;
            bTraceApp = Tracing.appSwitch;

            bValidInput = false;
            bConsoleOperation = false;

            mRawStrInput = null;
            mRawIntInput = null;
            mSortedData = null;

            MinDataLength = 1;
            Operation = "Unknown";
        }
        
        ////////////////////////////////////
        // Protected Members
        protected float _Value;

        protected String Operation = "";
        protected String mRawStrInput;
        protected float[] mRawIntInput;
        protected float[] mSortedData;
        protected Int32 MinDataLength;

        protected Boolean bValidInput;
        protected Boolean bConsoleOperation;

        protected BooleanSwitch bTraceData;
        protected TraceSwitch bTraceApp;

        protected readonly String InputDelimiter = " ,;:_";

        protected readonly String ERR_WrongDataInput = "Invalid data input.";
        protected readonly String ERR_WrongDataEntry = "Zero tolerance for any wrong or no data entry";
        protected readonly String ERR_BadObjectState = "Bad Object state";
        protected readonly String ERR_UnsortedObject = "Data is not sorted";

        protected readonly string ERR_FORM_MinLength = "Must provide at least {0} members";
    }
}
