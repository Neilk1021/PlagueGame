/*Script created by Pierre Stempin*/

using UnityEngine;

namespace ComponentsFinder
{
	/// <summary>
	/// Database of most Components Finder strings.
	/// </summary>
	public class ComponentsFinderStrings 
	{
		#region Generic Strings
		public const string Window = "Window";
		public const string Tools = "Tools";
		public const string Options = "Options";
		public const string _2D = "2D";
		public const string _3DInParenthesis = "(3D)";
		public const string _Physics = "Physics";
		public const string _Collider = "Collider";
		const string unity = "Unity";
		const string editor = "Editor";
		#endregion

		#region Keyboard Shortcut Names
		public const string ShiftName = "#";
		public const string AltName = "&";
		public const string ControlName = "%";
		public const string ShortcutName = "F";
		#endregion

		#region String Connections
		public const string None = "";
		public const string _Space = " ";
		public const string Underscore = "_";
		public const string Hyphen = "-";
		public const string Dot = ".";
		public const string Slash = "/";
		#endregion

		#region Tool Strings
		/* If you think that the "Find " part of the string in the button is too much",
		 * you can directly change the "Find" string content.*/
		public const string Find_ = Find + _Space;
		public const string _ComponentsFinder = Components + _Space + Finder;
		public const string Finder = "Finder";
		const string Find = "Find";
		const string Components = "Components";
		#endregion

		#region Category Names
		public const string Playables = "Playables";
		public const string Favorites = "Favorites";
		public const string _Mesh = "Mesh";
		public const string AR = "AR";
		public const string Analytics = "Analytics";
		public const string Audio = "Audio";
		public const string Video = "Video";
		public const string Rendering = "Rendering";
		public const string Effects = "Effects";
		public const string Miscellaneous = "Miscellaneous";
		public const string Navigation = "Navigation";
		public const string Network = "Network";
		public const string Scripts = "Scripts";
		public const string Layout = "Layout";
		public const string UI = "UI";
		public const string Event = "Event";
		public const string Others = "Others";
		public const string Physics2D = _Physics + _Space + _2D;
		public const string Physics3DInParenthesis = _Physics + _Space + _3DInParenthesis;
		#endregion

		#region Specific Script Names

		#region Audio
		public const string AudioSpatializer = Audio + _Space + spatializer;
		const string spatializer = "Spatializer";
		#endregion

		#region Effects
		public const string LegacyParticles = legacy + _Space + particles;
		const string legacy = "Legacy";
		const string particles = particle + "s";
		const string particle = "Particle";
		#endregion

		public const string _Halo = "Halo";
		const string world = "World";
		const string input = "Input";

		const string holoLens = "HoloLens";

		public const string WorldParticleCollider = world + particle + _Collider;

		public const string ContentSizeFitter = content + size + fitter;
		public const string AspectRatioFitter = aspect + ratio + fitter;
		const string fitter = "Fitter";

		const string content = "Content";
		const string size = "Size";
		const string aspect = "Aspect";
		const string ratio = "Ratio";

		public const string _Physics3DRaycaster = _Physics + _3DInParenthesis + raycaster;
		const string raycaster = "Raycaster";

		#region Scripts
		public const string UnityEngine_EventSystems = unityEngine + Dot + eventSystems;
		public const string UnityEngine_TestTools_TestRunner_CallBacks = unityEngine + Dot + testTools + Dot + testRunner + Dot + callBacks;
		public const string UnityEngine_Advertisements = unityEngine + Dot + advertisements;
		public const string UnityEngine_Purchasing = unityEngine + Dot + purchasing;
		public const string UnityEngine_PlayModeTestRunner = unityEngine + Dot + play + mode + testRunner;

		const string advertisements = "Advertisements";
		const string purchasing = "Purchasing";
		const string unityEngine = unity + "Engine";
		const string eventSystems = "EventSystems";
		const string test = "Test";
		const string tools = "Tools";
		const string testTools = test + tools;
		const string testRunner = test + runner;
		const string callBacks = "CallBacks";

		const string _base = "Base"; 
		const string play = "Play";
		const string mode = "Mode";
		const string runner = "Runner";
		const string callback = "Callback";

		public const string _BaseInput = _base + _Space + input;
		public const string _HoloLensInput = holoLens + _Space + input;
		public const string PlayModeRunnerCallback = play + mode + runner + callback;
		public const string _PlayModeRunnerCallback = play + _Space + mode + _Space + runner + _Space + callback;

		public const string UnityAdsEditorPlaceholder = unity + "Ads" + editor + "Placeholder";
		public const string PlaymodeTestsController = play + "modeTestsController";
		public const string ResultsRenderer = "ResultsRenderer";
		public const string TestRunnerLoggerCallback = testRunner + "LoggerCallback";
		public const string AsyncUtil = "AsyncUtil";
		#endregion

		public const string _FlareLayer = "FlareLayer";
		public const string _EllipsoidParticleEmitter = "EllipsoidParticleEmitter";
		public const string _MeshParticleEmitter = "MeshParticleEmitter";
		public const string _2DRectMask = "2DRectMask";

		#endregion

		#region Star Button Strings
		public const string Star = "Star";
		public const string Enabled = "Enabled";
		public const string Disabled = "Disabled";
		public const string Icons = "Icons";
		#endregion

		#region Search Strings
		public const string SetSearchFilter = "SetSearchFilter";
		public const string UnityEditorSceneHierarchyWindow = unityEditor + Dot + sceneHierarchyWindow;
		public const string SearchByType = "t:";
		const string unityEditor = unity + editor;
		const string sceneHierarchyWindow = "SceneHierarchyWindow";
		#endregion

		#region Get Scripts Strings
		const string Assembly = "Assembly";
		const string CSharp = "CSharp";
		public const string AssemblyCSharp = Assembly + Hyphen + CSharp;
		#endregion
	}
}

