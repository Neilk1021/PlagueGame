/*Script created by Pierre Stempin*/

using UnityEngine;

namespace ComponentsFinder
{
	/// <summary>
	/// Values used for spacing for the Components Finder editor window
	/// </summary>
	public class WindowValues 
	{
		public const int ShortSpacingHeight = 10;
		public const int MediumSpacingHeight = 20;
		public static int EndCategorySpacingHeight = MediumSpacingHeight;
		public const int MinHeight = 310;
		public const int MinLength = 277;

		public static float CurrentPositionY;

		public const float LeftMargin = 4f;

		public const float ButtonHeight = 21f;
		public const float ButtonSpaceHeight = 2f;
		public static float FullButtonHeight = ButtonHeight + ButtonSpaceHeight;

		public const float FoldoutHeight = 21f;
		public const float FoldoutSpaceHeight = 2f;
		public static float FullFoldoutHeight = FoldoutHeight + FoldoutSpaceHeight;

		public const float InitialPositionY = 21f;
		public const float StarButtonLength = 21f;
	}
}
