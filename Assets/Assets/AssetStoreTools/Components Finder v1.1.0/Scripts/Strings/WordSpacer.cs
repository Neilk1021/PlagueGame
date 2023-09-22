/*Script created by Pierre Stempin*/

using UnityEngine;

namespace ComponentsFinder
{
	/// <summary>
	/// Add a space between two words in a string in Camel or Pascal Case
	/// Ex : "BoxCollider" becomes "Box Collider"
	/// </summary>
	public class WordSpacer
	{
		static int currentSpaces;

		public static void SpaceWords (ref string stringToSpace)
		{
			currentSpaces = 0;
			char [] charArrayToCheck = stringToSpace.ToCharArray ();
			bool [] isUpper = new bool [charArrayToCheck.Length];

			//Check upper
			for (int i = 0; i < charArrayToCheck.Length; i ++) 
			{
				switch (charArrayToCheck [i]) 
				{
					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
					case 'G':
					case 'H':
					case 'I':
					case 'J':
					case 'K':
					case 'L':
					case 'M':
					case 'N':
					case 'O':
					case 'P':
					case 'Q':
					case 'R':
					case 'S':
					case 'T':
					case 'U':
					case 'V':
					case 'W':
					case 'X':
					case 'Y':
					case 'Z':
					
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':

					case '(' :
					case ')' :
						isUpper [i] = true;
					break;
				}
			}

			for (int i = 0; i < charArrayToCheck.Length; i ++) 
			{
				bool currentCharIsUpper = isUpper [i];

				//Not the first letter
				if (i > 0)
				{
					bool previousCharIsUpper = isUpper [i - 1];

					if (currentCharIsUpper)
					{
						if (i + 1 < charArrayToCheck.Length)
						{
							bool nextCharIsUpper = isUpper [i + 1];

							if (!previousCharIsUpper || !nextCharIsUpper)
							{
								SpaceWord (ref stringToSpace, i);
							}
						}
						//Last letter
						else
						{
							if (!previousCharIsUpper)
							{
								SpaceWord (ref stringToSpace, i);
							}
						}
					}
				}
				//First letter
				else
				{
					if (!currentCharIsUpper)
					{
						string firstLetter = stringToSpace.Substring (0, 1);
						string lastPart = stringToSpace.Substring (1);
						firstLetter = firstLetter.ToUpper ();
						stringToSpace = firstLetter + lastPart;
					}
				}
			}
				
			Replace (ref stringToSpace, ComponentsFinderStrings.Underscore, ComponentsFinderStrings.None);
		}

		public static void Replace (ref string n, string a, string b)
		{
			n = n.Replace (a, b);
		}

		public static void SpaceWord (ref string stringToSpace, int i)
		{
			string firstPart = stringToSpace.Substring (0, i + currentSpaces);
			string lastPart = stringToSpace.Substring (i + currentSpaces);
			stringToSpace = firstPart + ComponentsFinderStrings._Space + lastPart;
			currentSpaces ++;
		}
	}
}
