/*Script created by Pierre Stempin*/

using UnityEngine;
using System;

namespace ComponentsFinder
{
	public enum BoolOption
	{
		None,
		Adds3D,
		AddsASpace,
		IsFavorite
	}

	[Serializable]
	public class ComponentInfos 
	{
		public Type T;
		public string ScriptName;
		public string DisplayedName;
		public bool Adds3D;
		public bool AddsASpace;
		public bool IsFavorite;
		public Action onCreateSubCategory;

		public ComponentInfos ()
		{
			
		}

		public ComponentInfos (Action _onCreateSubCategory)
		{
			onCreateSubCategory = _onCreateSubCategory;
		}

		public ComponentInfos (Type t)
		{
			SetType (t);
			SetName (t.Name);
			RemoveAction ();
		}

		public ComponentInfos (Type t, BoolOption boolOption)
		{
			ApplyBoolOption (boolOption);
			SetType (t);
			SetName (t);
			RemoveAction ();
		}

		public ComponentInfos (Type t, BoolOption boolOption1, BoolOption boolOption2)
		{
			BoolOption [] boolOptions = new BoolOption [] {boolOption1, boolOption2};

			foreach (BoolOption boolOption in boolOptions) 
			{
				ApplyBoolOption (boolOption);
			}

			SetType (t);
			SetName (t);
			RemoveAction ();
		}

		public ComponentInfos (Type t, BoolOption boolOption1, BoolOption boolOption2, BoolOption boolOption3)
		{
			BoolOption [] boolOptions = new BoolOption [] {boolOption1, boolOption2, boolOption3};

			foreach (BoolOption boolOption in boolOptions) 
			{
				ApplyBoolOption (boolOption);
			}

			SetType (t);
			SetName (t);
			RemoveAction ();
		}

		public ComponentInfos (Type t, string scriptName)
		{
			SetType (t);
			SetName (scriptName);
			RemoveAction ();
		}

		public ComponentInfos (Type t, string scriptName, BoolOption boolOption1)
		{
			ApplyBoolOption (boolOption1);
			SetType (t);
			SetName (scriptName);
			RemoveAction ();
		}

		public ComponentInfos (string scriptName)
		{
			SetName (scriptName);
			RemoveAction ();
		}

		public ComponentInfos (string scriptName, BoolOption boolOption1)
		{
			ApplyBoolOption (boolOption1);
			SetName (scriptName);
			RemoveAction ();
		}

		private void ApplyBoolOption (BoolOption boolOption)
		{
			switch (boolOption) 
			{
				case BoolOption.Adds3D :
					Adds3D = true;
				break;

				case BoolOption.AddsASpace :
					AddsASpace = true;
				break;

				case BoolOption.IsFavorite :
					IsFavorite = true;
				break;
			}
		}

		private void SetType (Type t)
		{
			T = t;
		}

		private void SetName (string scriptName)
		{
			SetScriptName (scriptName);
			SetDisplayedName (Adds3D);
		}

		private void SetName (Type t)
		{
			SetScriptName (t.Name);
			SetDisplayedName (Adds3D);
		}

		private void SetName (Type t, string displayedName)
		{
			SetScriptName (t.Name);
			SetDisplayedName (displayedName);
		}

		private void SetScriptName (string scriptName)
		{
			ScriptName = scriptName;
		}

		private void SetDisplayedName (bool adds3D)
		{
			DisplayedName = ScriptName;

			if (adds3D)
			{
				DisplayedName = ScriptName + "(3D)";
			}

			SetDisplayedName ();
		}

		private void SetDisplayedName ()
		{
			WordSpacer.SpaceWords (ref DisplayedName);
		}

		private void SetDisplayedName (string displayedName)
		{
			DisplayedName = displayedName;
		}

		private void RemoveAction ()
		{
			onCreateSubCategory = null;
		}
	}
}
