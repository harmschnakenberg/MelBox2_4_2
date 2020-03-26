using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MelBox2_4
{
    public partial class MainWindow : Window
    {

        #region Felder
        private static int _InBoxCount;
		private static int _OutBoxCount;
		private static int _InMsgsSinceStartup;
		private static int _OutMsgsSinceStartup;
		private static int _ErrorCount;
		private static int _MessageLogCount;
		#endregion

		#region Properties
		public static int InBoxCount
		{
			get { return _InBoxCount; }
			set { 
				_InBoxCount = value;
				NotifyStaticPropertyChanged();
			}
		}

		public static int OutBoxCount
		{
			get { return _OutBoxCount; }
			set
			{
				_OutBoxCount = value;
				NotifyStaticPropertyChanged();
			}
		}

		public static int InMsgsSinceStartup
		{
			get { return _InMsgsSinceStartup; }
			set
			{
				_InMsgsSinceStartup = value;
				NotifyStaticPropertyChanged();
			}
		}

		public static int OutMsgsSinceStartup
		{
			get { return _OutMsgsSinceStartup; }
			set
			{
				_OutMsgsSinceStartup = value;
				NotifyStaticPropertyChanged();
			}
		}

		public static int ErrorCount
		{
			get { return _ErrorCount; }
			set
			{
				_ErrorCount = value;
				NotifyStaticPropertyChanged();
			}
		}

		public static int MessageLogCount
		{
			get { return _MessageLogCount; }
			set
			{
				_MessageLogCount = value;
				NotifyStaticPropertyChanged();
			}
		}
		#endregion

		#region Methoden


		#endregion
	}
}
