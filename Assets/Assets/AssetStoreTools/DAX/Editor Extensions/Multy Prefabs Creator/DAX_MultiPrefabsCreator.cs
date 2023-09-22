// DAzBjax (2015), if you have same questions contact me at DAzBjax.Unity@mail.ru 

// DAzBjax(2015) - base release
// DAzBjax(2016) - added renaming functionality
// DAh Right[16] - added new icon for renaming functionality
// rt_soft(2016) - not forces errors if you compile you project with this script

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
    using UnityEditor;
    using System.IO;
    using System.Reflection;

public class DAX_MultiPrefabsCreator : EditorWindow 
{ 
	const string MenuItemSTR = "Tools/Multi prefabs creation";
	const string TabCaptionSTR = "MP Creation";
	
	string Path, fullName;

	Object[] findedObjects;
	bool[] exportedObjects;
	bool[] exportedErrors;
    bool[] exportedWarnings;
    bool[] exportedWarningsUpdate;

	bool ClearLOG;
//**************************************
	bool EXEnabled;
//**************************************
	bool overwritePosition = true;
	Vector3 positionBaseVector = Vector3.zero;
//**************************************
	bool overwriteRotation = false;
	Vector3 rotationBaseVector = Vector3.zero;
//**************************************
	bool overwriteScale = false;
	Vector3 scaleBaseVector = Vector3.one;
//**************************************

	int lastErrorsCount;
    int lastWarningsCount;
    int lastWarningsUpdateCount;
	
	
	bool overwriteExists = false;
    bool replaceByName = false;
    bool RenameExists = false;

	Vector2 scrollVector;
	
	byte[] pngBytesFail = new byte[] {137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 18, 0, 0, 0, 18, 8, 6, 0, 0, 0, 86, 206, 142, 87, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 11, 19, 0, 0, 11, 19, 1, 0, 154, 156, 24, 0, 0, 1, 54, 105, 67, 67, 80, 80, 104, 111, 116, 111, 115, 104, 111, 112, 32, 73, 67, 67, 32, 112, 114, 111, 102, 105, 108, 101, 0, 0, 120, 218, 173, 142, 177, 74, 195, 80, 20, 64, 207, 139, 162, 226, 80, 43, 4, 113, 112, 120, 147, 40, 40, 182, 234, 96, 198, 164, 45, 69, 16, 172, 213, 33, 201, 214, 164, 161, 74, 105, 18, 94, 94, 213, 126, 132, 163, 91, 7, 23, 119, 191, 192, 201, 81, 112, 80, 252, 2, 255, 64, 113, 234, 224, 16, 33, 131, 131, 8, 158, 233, 220, 195, 229, 114, 193, 168, 216, 117, 167, 97, 148, 97, 16, 107, 213, 110, 58, 210, 245, 124, 57, 251, 196, 12, 83, 0, 208, 9, 179, 212, 110, 181, 14, 0, 226, 36, 142, 248, 193, 231, 43, 2, 224, 121, 211, 174, 59, 13, 254, 198, 124, 152, 42, 13, 76, 128, 237, 110, 148, 133, 32, 42, 64, 255, 66, 167, 26, 196, 24, 48, 131, 126, 170, 65, 220, 1, 166, 58, 105, 215, 64, 60, 0, 165, 94, 238, 47, 64, 41, 200, 253, 13, 40, 41, 215, 243, 65, 124, 0, 102, 207, 245, 124, 48, 230, 0, 51, 200, 125, 5, 48, 117, 116, 169, 1, 106, 73, 58, 82, 103, 189, 83, 45, 171, 150, 101, 73, 187, 155, 4, 145, 60, 30, 101, 58, 26, 100, 114, 63, 14, 19, 149, 38, 170, 163, 163, 46, 144, 255, 7, 192, 98, 190, 216, 110, 58, 114, 173, 106, 89, 123, 235, 252, 51, 174, 231, 203, 220, 222, 143, 16, 128, 88, 122, 44, 90, 65, 56, 84, 231, 223, 42, 140, 157, 223, 231, 226, 198, 120, 25, 14, 111, 97, 122, 82, 180, 221, 43, 184, 217, 128, 133, 235, 162, 173, 86, 161, 188, 5, 247, 227, 47, 192, 198, 79, 253, 232, 90, 79, 98, 0, 0, 0, 32, 99, 72, 82, 77, 0, 0, 122, 37, 0, 0, 128, 131, 0, 0, 249, 255, 0, 0, 128, 232, 0, 0, 82, 8, 0, 1, 21, 88, 0, 0, 58, 151, 0, 0, 23, 111, 215, 90, 31, 144, 0, 0, 3, 57, 73, 68, 65, 84, 120, 218, 108, 148, 207, 75, 92, 87, 20, 199, 63, 247, 206, 123, 142, 239, 141, 63, 138, 58, 234, 216, 209, 144, 54, 212, 82, 66, 23, 249, 177, 203, 166, 133, 182, 139, 138, 221, 37, 141, 26, 8, 76, 54, 249, 43, 242, 23, 164, 139, 172, 36, 37, 129, 152, 85, 87, 214, 80, 19, 136, 66, 17, 53, 139, 68, 2, 129, 128, 177, 37, 86, 34, 51, 234, 75, 37, 204, 100, 230, 205, 140, 111, 238, 61, 93, 248, 102, 58, 41, 61, 112, 185, 112, 190, 135, 207, 57, 231, 158, 195, 85, 11, 11, 11, 136, 8, 77, 235, 240, 125, 54, 86, 86, 134, 88, 95, 239, 235, 25, 31, 79, 12, 77, 78, 166, 26, 149, 138, 32, 146, 82, 90, 39, 6, 179, 217, 93, 202, 229, 173, 70, 20, 1, 32, 34, 100, 50, 25, 156, 38, 64, 172, 197, 75, 165, 40, 212, 235, 39, 179, 175, 95, 255, 214, 87, 44, 126, 161, 159, 62, 197, 62, 121, 130, 210, 26, 172, 37, 18, 97, 243, 236, 217, 114, 122, 102, 102, 34, 211, 217, 185, 18, 86, 171, 173, 2, 156, 38, 196, 239, 234, 34, 47, 242, 57, 119, 239, 46, 254, 56, 63, 255, 137, 199, 255, 91, 97, 115, 179, 235, 23, 145, 135, 42, 151, 251, 118, 216, 243, 214, 43, 149, 10, 0, 26, 17, 58, 83, 41, 118, 173, 253, 178, 49, 59, 251, 184, 9, 57, 2, 234, 255, 57, 71, 192, 136, 49, 92, 158, 155, 243, 243, 119, 238, 44, 21, 224, 43, 207, 247, 17, 17, 180, 147, 76, 178, 127, 116, 116, 94, 207, 206, 62, 186, 60, 63, 63, 218, 132, 0, 168, 116, 26, 247, 254, 125, 220, 155, 55, 81, 222, 113, 141, 17, 48, 12, 76, 207, 205, 121, 193, 189, 123, 243, 251, 198, 124, 221, 145, 76, 194, 220, 194, 194, 216, 207, 151, 46, 5, 33, 136, 128, 212, 226, 83, 5, 169, 95, 184, 32, 34, 34, 82, 171, 73, 253, 212, 41, 169, 198, 254, 90, 28, 91, 80, 74, 126, 202, 229, 138, 191, 63, 127, 254, 153, 35, 149, 74, 207, 208, 139, 23, 105, 15, 8, 1, 213, 124, 124, 128, 98, 17, 26, 13, 164, 80, 192, 132, 33, 18, 235, 2, 84, 129, 140, 8, 131, 27, 27, 61, 249, 221, 221, 113, 71, 39, 18, 136, 235, 2, 208, 0, 18, 109, 32, 57, 56, 64, 130, 0, 27, 4, 152, 195, 195, 227, 118, 99, 221, 54, 95, 95, 41, 172, 49, 56, 128, 54, 74, 181, 64, 182, 13, 164, 74, 37, 236, 222, 30, 230, 224, 128, 168, 94, 255, 0, 212, 220, 60, 171, 53, 74, 41, 235, 0, 174, 141, 1, 13, 64, 183, 7, 214, 106, 216, 55, 111, 176, 59, 59, 68, 109, 90, 171, 98, 192, 42, 133, 3, 29, 14, 34, 142, 21, 33, 2, 76, 91, 166, 102, 224, 209, 203, 151, 152, 124, 254, 131, 182, 105, 75, 44, 214, 130, 72, 167, 35, 34, 73, 226, 117, 55, 237, 189, 199, 160, 112, 113, 17, 27, 134, 216, 182, 36, 237, 173, 25, 107, 233, 72, 36, 28, 221, 159, 205, 238, 189, 61, 125, 250, 240, 29, 224, 199, 89, 76, 124, 171, 145, 17, 122, 111, 221, 162, 247, 198, 13, 196, 247, 91, 154, 1, 186, 129, 63, 128, 119, 231, 206, 133, 163, 99, 99, 123, 122, 192, 243, 254, 244, 174, 92, 249, 238, 215, 137, 137, 183, 85, 160, 51, 94, 186, 8, 48, 142, 131, 30, 30, 70, 165, 211, 24, 215, 109, 129, 82, 192, 14, 176, 56, 61, 93, 254, 248, 226, 197, 169, 254, 100, 114, 89, 173, 174, 174, 82, 42, 151, 217, 51, 230, 124, 245, 246, 237, 165, 137, 7, 15, 122, 93, 224, 125, 60, 33, 119, 124, 28, 137, 34, 162, 237, 109, 20, 208, 11, 228, 129, 199, 51, 51, 245, 19, 87, 175, 78, 12, 137, 44, 247, 15, 12, 160, 149, 82, 212, 42, 21, 178, 174, 251, 172, 251, 250, 245, 111, 30, 78, 78, 190, 15, 129, 62, 160, 7, 112, 183, 182, 72, 110, 111, 211, 27, 251, 118, 19, 9, 150, 166, 167, 27, 39, 115, 185, 31, 50, 176, 92, 173, 213, 80, 199, 147, 3, 165, 53, 97, 185, 204, 80, 87, 215, 179, 253, 107, 215, 190, 127, 228, 186, 139, 31, 189, 122, 213, 109, 149, 66, 197, 59, 166, 68, 48, 192, 223, 103, 206, 148, 78, 76, 77, 77, 14, 26, 179, 82, 169, 86, 255, 213, 215, 214, 214, 8, 130, 160, 229, 112, 146, 73, 240, 253, 79, 139, 65, 208, 45, 214, 66, 236, 71, 4, 180, 54, 195, 163, 163, 101, 83, 42, 253, 213, 252, 216, 0, 178, 217, 44, 255, 12, 0, 195, 58, 152, 216, 150, 194, 58, 32, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130};
	Texture2D failTex;
	
	byte[] pngBytesEYE = new byte[] {137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 19, 0, 0, 0, 19, 8, 6, 0, 0, 0, 114, 80, 54, 204, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 11, 19, 0, 0, 11, 19, 1, 0, 154, 156, 24, 0, 0, 0, 32, 99, 72, 82, 77, 0, 0, 122, 37, 0, 0, 128, 131, 0, 0, 249, 255, 0, 0, 128, 232, 0, 0, 82, 8, 0, 1, 21, 88, 0, 0, 58, 151, 0, 0, 23, 111, 215, 90, 31, 144, 0, 0, 2, 76, 73, 68, 65, 84, 120, 218, 228, 148, 203, 79, 19, 97, 20, 197, 207, 204, 180, 196, 118, 58, 51, 116, 35, 76, 37, 33, 64, 89, 72, 210, 198, 134, 146, 144, 82, 22, 117, 129, 16, 68, 23, 60, 130, 91, 82, 170, 136, 11, 18, 12, 113, 217, 53, 143, 45, 255, 129, 134, 242, 90, 104, 13, 77, 109, 40, 144, 144, 52, 145, 202, 163, 6, 215, 26, 211, 84, 87, 246, 49, 44, 128, 249, 174, 11, 165, 97, 120, 24, 22, 221, 121, 183, 223, 239, 158, 197, 119, 206, 185, 28, 17, 161, 82, 195, 163, 130, 83, 81, 49, 78, 177, 59, 175, 123, 107, 3, 48, 12, 192, 3, 64, 2, 80, 4, 176, 7, 224, 53, 128, 244, 141, 196, 24, 35, 23, 17, 91, 240, 122, 239, 181, 12, 244, 247, 193, 237, 186, 11, 155, 36, 162, 84, 212, 144, 249, 252, 5, 145, 197, 183, 216, 217, 217, 59, 52, 153, 132, 97, 142, 227, 50, 134, 101, 197, 238, 132, 98, 119, 66, 174, 110, 178, 112, 66, 205, 98, 71, 231, 67, 138, 197, 214, 233, 228, 228, 148, 174, 26, 198, 24, 37, 18, 91, 228, 243, 247, 18, 39, 212, 44, 201, 213, 77, 150, 51, 13, 40, 118, 39, 108, 114, 131, 200, 155, 213, 200, 104, 104, 146, 24, 99, 229, 197, 100, 114, 155, 6, 135, 130, 164, 222, 113, 211, 224, 80, 144, 146, 201, 109, 131, 112, 232, 217, 75, 18, 204, 106, 196, 38, 55, 136, 138, 221, 9, 72, 74, 163, 96, 170, 114, 76, 143, 4, 39, 12, 96, 42, 149, 166, 177, 241, 41, 90, 89, 141, 146, 166, 105, 180, 178, 26, 165, 177, 241, 41, 74, 165, 210, 6, 110, 36, 56, 65, 166, 42, 199, 180, 164, 52, 10, 16, 204, 106, 115, 187, 175, 135, 242, 249, 130, 1, 10, 135, 103, 104, 118, 110, 158, 116, 93, 39, 34, 34, 93, 215, 105, 118, 110, 158, 194, 225, 25, 3, 151, 207, 23, 168, 221, 215, 67, 130, 89, 109, 230, 69, 209, 26, 120, 208, 21, 128, 44, 75, 134, 191, 76, 239, 30, 192, 225, 168, 5, 207, 255, 73, 15, 207, 243, 112, 56, 106, 145, 222, 61, 48, 112, 178, 44, 161, 187, 235, 62, 68, 209, 26, 224, 53, 237, 104, 35, 30, 223, 64, 161, 80, 52, 64, 173, 30, 55, 178, 217, 28, 24, 99, 127, 93, 102, 200, 102, 115, 104, 245, 184, 13, 92, 161, 80, 68, 44, 190, 14, 77, 59, 74, 10, 54, 233, 246, 175, 175, 223, 190, 91, 114, 63, 126, 118, 60, 126, 212, 93, 134, 44, 150, 91, 72, 110, 110, 227, 248, 248, 4, 245, 245, 117, 136, 190, 255, 128, 244, 167, 125, 12, 244, 247, 161, 174, 78, 45, 115, 207, 95, 188, 194, 218, 218, 250, 140, 213, 106, 89, 46, 187, 41, 152, 213, 72, 232, 233, 205, 221, 100, 140, 209, 104, 104, 146, 248, 243, 110, 94, 204, 153, 207, 223, 75, 137, 196, 150, 65, 244, 114, 206, 54, 201, 223, 217, 71, 188, 169, 118, 241, 124, 206, 46, 53, 128, 136, 92, 167, 167, 250, 66, 91, 155, 167, 101, 112, 224, 172, 1, 54, 148, 138, 37, 236, 103, 14, 177, 180, 252, 14, 59, 31, 175, 110, 192, 191, 186, 233, 5, 240, 228, 66, 55, 119, 1, 188, 185, 182, 155, 255, 199, 61, 251, 61, 0, 216, 84, 140, 133, 246, 8, 144, 48, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 };
	Texture2D eyeTex;

    byte[] pngshieldHaveChanges = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 16, 0, 0, 0, 16, 8, 6, 0, 0, 0, 31, 243, 255, 97, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 11, 19, 0, 0, 11, 19, 1, 0, 154, 156, 24, 0, 0, 1, 54, 105, 67, 67, 80, 80, 104, 111, 116, 111, 115, 104, 111, 112, 32, 73, 67, 67, 32, 112, 114, 111, 102, 105, 108, 101, 0, 0, 120, 218, 173, 142, 177, 74, 195, 80, 20, 64, 207, 139, 162, 226, 80, 43, 4, 113, 112, 120, 147, 40, 40, 182, 234, 96, 198, 164, 45, 69, 16, 172, 213, 33, 201, 214, 164, 161, 74, 105, 18, 94, 94, 213, 126, 132, 163, 91, 7, 23, 119, 191, 192, 201, 81, 112, 80, 252, 2, 255, 64, 113, 234, 224, 16, 33, 131, 131, 8, 158, 233, 220, 195, 229, 114, 193, 168, 216, 117, 167, 97, 148, 97, 16, 107, 213, 110, 58, 210, 245, 124, 57, 251, 196, 12, 83, 0, 208, 9, 179, 212, 110, 181, 14, 0, 226, 36, 142, 248, 193, 231, 43, 2, 224, 121, 211, 174, 59, 13, 254, 198, 124, 152, 42, 13, 76, 128, 237, 110, 148, 133, 32, 42, 64, 255, 66, 167, 26, 196, 24, 48, 131, 126, 170, 65, 220, 1, 166, 58, 105, 215, 64, 60, 0, 165, 94, 238, 47, 64, 41, 200, 253, 13, 40, 41, 215, 243, 65, 124, 0, 102, 207, 245, 124, 48, 230, 0, 51, 200, 125, 5, 48, 117, 116, 169, 1, 106, 73, 58, 82, 103, 189, 83, 45, 171, 150, 101, 73, 187, 155, 4, 145, 60, 30, 101, 58, 26, 100, 114, 63, 14, 19, 149, 38, 170, 163, 163, 46, 144, 255, 7, 192, 98, 190, 216, 110, 58, 114, 173, 106, 89, 123, 235, 252, 51, 174, 231, 203, 220, 222, 143, 16, 128, 88, 122, 44, 90, 65, 56, 84, 231, 223, 42, 140, 157, 223, 231, 226, 198, 120, 25, 14, 111, 97, 122, 82, 180, 221, 43, 184, 217, 128, 133, 235, 162, 173, 86, 161, 188, 5, 247, 227, 47, 192, 198, 79, 253, 232, 90, 79, 98, 0, 0, 0, 32, 99, 72, 82, 77, 0, 0, 122, 37, 0, 0, 128, 131, 0, 0, 249, 255, 0, 0, 128, 232, 0, 0, 82, 8, 0, 1, 21, 88, 0, 0, 58, 151, 0, 0, 23, 111, 215, 90, 31, 144, 0, 0, 2, 104, 73, 68, 65, 84, 120, 218, 116, 82, 77, 107, 83, 65, 20, 61, 243, 222, 125, 53, 162, 86, 164, 196, 106, 8, 126, 65, 23, 74, 5, 113, 231, 207, 208, 101, 23, 98, 69, 193, 85, 4, 93, 40, 193, 90, 74, 233, 182, 169, 86, 210, 88, 5, 165, 193, 234, 170, 187, 46, 92, 216, 234, 66, 130, 173, 213, 80, 109, 81, 170, 165, 24, 36, 73, 147, 52, 205, 87, 243, 49, 243, 230, 186, 104, 242, 140, 109, 188, 112, 97, 102, 238, 185, 103, 206, 185, 51, 130, 153, 97, 89, 22, 90, 132, 7, 192, 141, 250, 250, 49, 128, 223, 59, 1, 82, 74, 16, 90, 199, 1, 0, 87, 39, 39, 7, 47, 2, 140, 158, 158, 251, 53, 0, 163, 0, 114, 187, 144, 204, 12, 34, 234, 35, 162, 94, 34, 234, 36, 162, 227, 68, 116, 119, 36, 112, 243, 189, 46, 189, 96, 187, 20, 230, 177, 224, 237, 15, 68, 116, 175, 94, 235, 36, 162, 43, 68, 212, 199, 204, 14, 193, 240, 236, 76, 48, 230, 241, 184, 199, 188, 222, 206, 208, 243, 103, 254, 168, 44, 76, 112, 37, 238, 231, 74, 220, 207, 50, 247, 132, 39, 195, 119, 22, 189, 94, 119, 200, 115, 180, 99, 108, 230, 245, 96, 140, 136, 134, 153, 25, 162, 65, 80, 204, 190, 242, 25, 118, 214, 100, 104, 176, 220, 128, 93, 77, 97, 224, 225, 182, 195, 126, 159, 130, 209, 118, 8, 194, 220, 15, 128, 161, 217, 165, 219, 143, 248, 30, 40, 165, 110, 53, 102, 160, 182, 10, 235, 202, 172, 45, 155, 205, 246, 114, 185, 210, 118, 81, 89, 128, 74, 2, 72, 2, 0, 106, 236, 150, 0, 20, 0, 103, 136, 229, 124, 161, 92, 61, 104, 201, 61, 206, 132, 181, 196, 220, 220, 103, 36, 18, 9, 248, 174, 159, 131, 214, 54, 0, 192, 48, 76, 84, 100, 185, 2, 160, 220, 76, 144, 255, 21, 203, 230, 207, 158, 18, 237, 74, 215, 176, 89, 77, 99, 75, 150, 16, 141, 70, 183, 201, 168, 226, 168, 178, 33, 177, 242, 35, 145, 111, 188, 136, 81, 63, 79, 190, 125, 183, 148, 174, 232, 50, 210, 229, 85, 212, 236, 44, 200, 168, 57, 77, 150, 161, 156, 108, 51, 129, 143, 243, 177, 76, 195, 79, 131, 224, 231, 252, 167, 239, 153, 42, 10, 48, 69, 5, 150, 33, 97, 25, 18, 153, 245, 1, 100, 214, 7, 156, 189, 101, 72, 180, 153, 26, 95, 191, 164, 50, 0, 86, 29, 11, 74, 169, 136, 101, 81, 188, 88, 232, 42, 239, 115, 169, 189, 0, 3, 0, 58, 14, 247, 3, 0, 86, 151, 207, 59, 106, 178, 57, 187, 252, 102, 54, 21, 87, 74, 69, 154, 21, 64, 107, 142, 4, 70, 22, 151, 92, 86, 155, 115, 219, 95, 11, 117, 5, 166, 194, 232, 163, 181, 37, 173, 57, 210, 168, 57, 95, 217, 182, 237, 160, 105, 154, 23, 78, 30, 67, 199, 181, 203, 237, 39, 12, 33, 69, 49, 227, 7, 0, 20, 55, 166, 160, 53, 248, 233, 196, 230, 218, 196, 203, 141, 111, 182, 109, 7, 255, 249, 202, 205, 41, 132, 24, 239, 62, 237, 90, 8, 135, 60, 233, 212, 74, 23, 167, 86, 186, 56, 28, 242, 164, 187, 207, 184, 22, 132, 16, 227, 59, 241, 187, 8, 152, 25, 0, 122, 1, 76, 7, 134, 220, 201, 192, 144, 59, 9, 96, 26, 64, 111, 43, 172, 168, 55, 180, 12, 33, 196, 84, 93, 229, 165, 255, 97, 254, 12, 0, 54, 181, 99, 91, 72, 67, 185, 184, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 };
    Texture2D warnTex;
	
	[MenuItem(MenuItemSTR)]
	static void Init () 
	{
		// Get existing open window or if none, make a new one:
		DAX_MultiPrefabsCreator window = (DAX_MultiPrefabsCreator)EditorWindow.GetWindow (typeof (DAX_MultiPrefabsCreator));
		window.titleContent.text = TabCaptionSTR;
		window.Show();
	}

	void updateInfo( bool loadSelection = false )
	{
		findedObjects = new Object[0];
		if (loadSelection) 
		{
			findedObjects = Selection.objects;
		}
		else
		{
			findedObjects = GameObject.FindObjectsOfType (typeof(GameObject));
		}
		
		Object[] tfObj;
		int maxSize = 0;
		for (int i = 0; i < findedObjects.Length; i++) 
		{
			if ((findedObjects[i] as GameObject).transform.parent == null)
			{
				maxSize++;
			}
		}

		tfObj = new Object[maxSize];
		int curItem = 0;
		for (int i = 0; i < findedObjects.Length; i++) 
		{
			if ((findedObjects[i] as GameObject).transform.parent == null)
			{
				tfObj[curItem] = findedObjects[i];
				curItem++;
			}
		}
		findedObjects = tfObj;


		exportedObjects = new bool[findedObjects.Length];
		exportedErrors = new bool[findedObjects.Length];
        exportedWarnings = new bool[findedObjects.Length];
        exportedWarningsUpdate = new bool[findedObjects.Length];
		for(int i = 0; i < findedObjects.Length; i++) 
		{
			exportedObjects[i] = true;
			exportedErrors[i] = false;
            exportedWarnings[i] = false;
            exportedWarningsUpdate[i] = false;
		}
	}
	
	public static void DAXClearLog()
	{
		Assembly  assembly = Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));
		System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
		System.Reflection.MethodInfo method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
	}

	bool DAXcreatePrefab( string location, object gObject)
	{
		try
		{
			GameObject GO = gObject as GameObject;
			
			if (EXEnabled) //creating with overwriting same params
			{
				Vector3 trans, scale;
				Quaternion rotation;
				
				trans = GO.transform.localPosition;
				scale = GO.transform.localScale;
				rotation = GO.transform.localRotation;
				
				if (overwritePosition){GO.transform.localPosition = positionBaseVector; };
				if (overwriteRotation){GO.transform.localRotation = Quaternion.Euler( rotationBaseVector ); };
				if (overwriteScale){GO.transform.localScale = scaleBaseVector; };

                if (replaceByName && System.IO.File.Exists( location ))
                {
                    Object Obj = AssetDatabase.LoadAssetAtPath(location, typeof(Object));
                    PrefabUtility.ReplacePrefab(GO, Obj, ReplacePrefabOptions.ReplaceNameBased);
                    Obj = null;
                }
                else
                {
                    PrefabUtility.CreatePrefab(location, GO, ReplacePrefabOptions.ConnectToPrefab);		//save prefab with overwritted params
                }
                
               //PrefabUtility.ReplacePrefab(GO, Resources.Load(location), ReplacePrefabOptions.ReplaceNameBased);
		
				if (overwriteScale){GO.transform.localScale = scale; };
				if (overwriteRotation){GO.transform.localRotation = rotation; };
				if (overwritePosition){GO.transform.localPosition = trans; };
			}
			else
			{                
                if (replaceByName && System.IO.File.Exists(location))
                {
                    Object Obj = AssetDatabase.LoadAssetAtPath(location, typeof(Object));
                    PrefabUtility.ReplacePrefab(GO, Obj, ReplacePrefabOptions.ReplaceNameBased);
                    Obj = null;
                }
                else
                {
                    PrefabUtility.CreatePrefab(location, GO, ReplacePrefabOptions.ConnectToPrefab);		//save prefab with overwritted params
                }
			}
			
			return true;
		}
		catch
		{
			return false;
		}
	}
	
	void userClickSelectOrCreatePrefabsDetectEmptyPath()
	{
		string baseSTR = EditorUtility.OpenFolderPanel ("Select folder to save prefabs", Application.dataPath, "prefabs");
		if ( baseSTR != "" )
		{
			if ( -1 == baseSTR.IndexOf( Application.dataPath, System.StringComparison.InvariantCultureIgnoreCase ) )
			{
				EditorUtility.DisplayDialog( "Assets folder error", "Target folder must be any child directory to : \"" + Application.dataPath + "\"", "OK" );
				Path = "";
			}
			else
			{
				Path = baseSTR.Replace (Application.dataPath, "Assets");
				updateInfo(Selection.objects.Length>1);
			}
		}
	}

	void OnGUI () 
	{
		try
		{
		
		GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Save folder:", GUILayout.MaxWidth(75.0f));
			EditorGUILayout.LabelField ( Path, EditorStyles.textField );	
		
			if(GUILayout.Button("..."/*Change save folder"*/,GUILayout.MaxWidth(30.0f) ))
			{
				userClickSelectOrCreatePrefabsDetectEmptyPath();
			}
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.Separator();// ("Base Settings", EditorStyles.boldLabel);
		
		
		GUILayout.Label ("Root Objects", EditorStyles.boldLabel );
		
		EditorGUILayout.BeginHorizontal ();
			if(GUILayout.RepeatButton("Load ALL", GUILayout.MaxWidth(70.0f)))
			{
				updateInfo( false );
			}
			if(GUILayout.RepeatButton("Load Selection", GUILayout.MaxWidth(95.0f)))
			{
				updateInfo( true );
			}

		EditorGUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.MaxHeight(400.0f));
		
		if (eyeTex==null) //no eye lodaded
		{
			eyeTex = new Texture2D(2,2);
			eyeTex.LoadImage( pngBytesEYE ); //load eye
		}			
		if (failTex==null) //no asterix lodaded
		{
			failTex = new Texture2D(2,2);
			failTex.LoadImage( pngBytesFail ); //load asterix
		}
        if (warnTex == null) //no asterix lodaded
        {
            warnTex = new Texture2D(2, 2);
            warnTex.LoadImage( pngshieldHaveChanges ); //load shield
        }
			scrollVector = GUILayout.BeginScrollView(scrollVector);
			lastErrorsCount = 0;
            lastWarningsCount = 0;
            lastWarningsUpdateCount = 0;
				if (findedObjects != null) 
				{
					for (int i = 0; i < findedObjects.Length; i++) 
					{
						if (findedObjects [i] != null)
						{		
							try //catch error : cant get 2-d item in two items   o_O
							{			
								EditorGUILayout.BeginHorizontal ();	
																	
									if ( exportedErrors[i] ) //have errors
									{	
										lastErrorsCount++;
										if (GUILayout.Button( failTex, GUI.skin.label, GUILayout.MaxHeight(18.0f), GUILayout.MaxWidth( 20.0f )   )) //draw asterix
										{
											EditorGUIUtility.PingObject( findedObjects [i] );
										}
										//GUILayout.Label( failTex, GUILayout.MaxHeight(16.0f), GUILayout.MaxWidth( 20.0f )  ); //draw asterix									
									}
                                    else if (exportedWarnings[i] )
                                    {
                                        lastWarningsCount++;
                                        if (GUILayout.Button(warnTex, GUI.skin.label, GUILayout.MaxHeight(18.0f), GUILayout.MaxWidth(20.0f))) //draw asterix
                                        {
                                            EditorGUIUtility.PingObject(findedObjects[i]);
                                        }
                                    }
                                    else if ( exportedWarningsUpdate[i])
                                    {
                                        lastWarningsUpdateCount++;
                                        if (GUILayout.Button(warnTex, GUI.skin.label, GUILayout.MaxHeight(18.0f), GUILayout.MaxWidth(20.0f))) //draw asterix
                                        {
                                            EditorGUIUtility.PingObject(findedObjects[i]);
                                        }
                                    }

                                    else
                                    {
                                        if (GUILayout.Button(eyeTex, GUI.skin.label, GUILayout.MaxHeight(18.0f), GUILayout.MaxWidth(20.0f))) //draw asterix
                                        {
                                            EditorGUIUtility.PingObject(findedObjects[i]);
                                        }
                                    }
									try //catch error : cant get 2-d item in two items   o_O
									{				
										exportedObjects [i] = EditorGUILayout.BeginToggleGroup (findedObjects [i].name, exportedObjects [i]); //checkboxes left
										EditorGUILayout.EndToggleGroup ();	
									}catch{};
								EditorGUILayout.EndHorizontal ();
							}catch{};
						}		
					}
				}
			GUILayout.EndScrollView();
		GUILayout.EndHorizontal();


		EXEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", EXEnabled);
		
		overwritePosition = EditorGUILayout.Toggle ("Overwrite position", overwritePosition);
		positionBaseVector = EditorGUILayout.Vector3Field ("Position", positionBaseVector);
		
		overwriteRotation = EditorGUILayout.Toggle ("Overwrite rotation", overwriteRotation);
		rotationBaseVector = EditorGUILayout.Vector3Field ("Rotation", rotationBaseVector);
		
		overwriteScale = EditorGUILayout.Toggle ("Overwrite scale", overwriteScale);
		scaleBaseVector = EditorGUILayout.Vector3Field ("Scale", scaleBaseVector);
		
		EditorGUILayout.EndToggleGroup ();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal ();


		GUILayout.BeginVertical (GUI.skin.box); 
		if (overwriteExists) 
		{
            GUILayout.BeginVertical(GUI.skin.box);
            if (!replaceByName)
            {
                GUIStyle richTextStyle = new GUIStyle();
                richTextStyle.richText = true;
                GUILayout.Label("<b>Making prefabs are overwrite all prefabs with \nsame name in target folder and \nmaking prefabs connection to new created.\n\n <color=red>   All old prefab connections will be lost!</color></b>", richTextStyle);
            }
            else
            {
                GUIStyle richTextStyle = new GUIStyle();
                richTextStyle.richText = true;
                //GUILayout.Label("<b>Making prefabs are overwrite all prefabs with \nsame name in target folder and \nmaking prefabs connection to new created.\n <color=red>   All old prefab connections will be lost!</color></b>", richTextStyle);

                GUILayout.Label("<b>Making prefabs are replace all prefabs with \nsame name in target folder and <color=red>NOT making \nprefabs connection to updated</color>, \nbut making connection to new created.\n\n <color=red>   All old(replaced) prefab connections will be \n      updated by name matching algorithm!</color></b>", richTextStyle);
            }
            GUILayout.EndVertical();
        }
        if (RenameExists)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Making prefabs are NOT overwrite prefabs with \nsame name in target folder but add \nnew prefab with same name and ended name \nwith new number [max 999]", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

		overwriteExists = EditorGUILayout.BeginToggleGroup ("Overwrite existing", overwriteExists);
        if (overwriteExists)
        {
            replaceByName = EditorGUILayout.BeginToggleGroup("Replace prefabs by name (create unexists)", replaceByName);
            EditorGUILayout.EndToggleGroup();
        }
        else
        {
            replaceByName = false;
        }
        EditorGUILayout.EndToggleGroup();

        if (overwriteExists) { RenameExists = false; };

        RenameExists = EditorGUILayout.BeginToggleGroup("Rename if exists", RenameExists);
        EditorGUILayout.EndToggleGroup();
        if (RenameExists) { overwriteExists = false; replaceByName = false; };
	
		ClearLOG = EditorGUILayout.BeginToggleGroup ("Clear log before proceed", ClearLOG);
		EditorGUILayout.EndToggleGroup ();
		
		GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Create prefabs")) 
			{
				if (Path=="")
				{
					userClickSelectOrCreatePrefabsDetectEmptyPath();
				}
				if (Path!="")
				{			
					System.IO.Directory.CreateDirectory( Path );
					
					if (ClearLOG) { DAXClearLog(); };
					if (findedObjects != null) 
					{
						string fName;
						for (int i = 0; i < findedObjects.Length; i++) 
						{
							if (exportedObjects[i])
							{
								try{
								if (findedObjects [i] != null)
								{
									fName = this.Path + "/" + findedObjects [i].name + ".prefab";
                                    exportedErrors[i] = false;
                                    exportedWarnings[i] = false;
                                    exportedWarningsUpdate[i] = false;

                                    if (overwriteExists == false && !(RenameExists))
									{
										if ( System.IO.File.Exists( fName ) )
									    {
         
											Debug.LogError("<size=14><b>MPC</b></size> : " +  this.Path + "/<b><size=14>" + findedObjects [i].name + "</size></b>.prefab <b>already exists</b>", findedObjects [i] as GameObject  );
											exportedErrors[i] = true;
										}
										else
										{									
											exportedErrors[i] = !DAXcreatePrefab ( fName, findedObjects[i] );
										}
                                        
									}
                                    else if (RenameExists)
                                    {
                                        if (System.IO.File.Exists(fName))
                                        {
                                            string nFileName = this.Path + "/" + findedObjects[i].name + "_";
                                            string sFN = "";
                                            for (int fnEnd = 0; fnEnd < 1000; fnEnd++)
                                            {
                                                if (!System.IO.File.Exists(nFileName + fnEnd.ToString() + ".prefab"))
                                                {
                                                    nFileName += (fnEnd.ToString() + ".prefab");
                                                    sFN = findedObjects[i].name + "_" + fnEnd.ToString() + ".prefab";
                                                    break;
                                                }
                                            }


                                            Debug.LogWarning("<size=14><b>MPC</b></size> : " + this.Path + "/<b><size=14>" + findedObjects[i].name + "</size></b>.prefab <b>already exists. Object saved with name : [" + sFN + "]</b>", findedObjects[i] as GameObject);
                                            exportedWarnings[i] = true;
                                            exportedErrors[i] = !DAXcreatePrefab(nFileName, findedObjects[i]);
                                        }
                                        else
                                        {
                                            exportedErrors[i] = !DAXcreatePrefab(fName, findedObjects[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (replaceByName & System.IO.File.Exists(fName))
                                        {
                                            Debug.LogWarning("<size=14><b>MPC</b></size> : " + this.Path + "/<b><size=14>" + findedObjects[i].name + "</size></b>.prefab <b>already exists. Object updated!</b>", findedObjects[i] as GameObject);
                                            exportedWarningsUpdate[i] = true;
                                        }
                                        exportedErrors[i] = !DAXcreatePrefab(fName, findedObjects[i]);
                                    }
								}
								}catch{};
							} else { exportedErrors[i] = false; }
						}
					}
				}
			}

			if (lastErrorsCount>0)
			{
				GUILayout.Label( failTex, GUILayout.MaxHeight(22.0f), GUILayout.MaxWidth( 22.0f )  ); //draw asterix	
				GUIStyle richTextStyle = new GUIStyle ();
				richTextStyle.richText = true;
				GUILayout.Label( "<b><color=red><size=18>" + lastErrorsCount.ToString() + " errors!!!</size></color></b>", richTextStyle, GUILayout.MaxWidth( 40.0f ) );
			}
            else if (lastWarningsCount > 0)
            {
                GUILayout.Label(warnTex, GUILayout.MaxHeight(22.0f), GUILayout.MaxWidth(22.0f)); //draw asterix	
                GUIStyle richTextStyle = new GUIStyle();
                richTextStyle.richText = true;
                GUILayout.Label("<b><color=green><size=18>" + lastWarningsCount.ToString() + " renames!!!</size></color></b>", richTextStyle, GUILayout.MaxWidth(40.0f));
            }
            else if (lastWarningsUpdateCount > 0 )
            {
                GUILayout.Label(warnTex, GUILayout.MaxHeight(22.0f), GUILayout.MaxWidth(22.0f)); //draw asterix	
                GUIStyle richTextStyle = new GUIStyle();
                richTextStyle.richText = true;
                GUILayout.Label("<b><color=green><size=18>" + lastWarningsUpdateCount.ToString() + " updates!!!</size></color></b>", richTextStyle, GUILayout.MaxWidth(40.0f));
            }

			GUILayout.FlexibleSpace();
			if (GUILayout.Button ("Cancel")) 
			{
				DAX_MultiPrefabsCreator window = (DAX_MultiPrefabsCreator)EditorWindow.GetWindow (typeof (DAX_MultiPrefabsCreator));
				window.Close();
			}
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
		
		}catch{};
	}

}

#endif