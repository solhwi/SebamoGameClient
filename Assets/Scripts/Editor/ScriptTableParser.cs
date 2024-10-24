#if UNITY_EDITOR

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectHelper
{
	public static ScriptableObject GenerateSO(string assetPath, Type assetType)
	{
		string fullAssetPath = $"Assets/Bundles/{assetPath}";
		Directory.CreateDirectory(Path.GetDirectoryName(fullAssetPath));

		var asset = AssetDatabase.LoadAssetAtPath(fullAssetPath, assetType) as ScriptableObject;

		if (asset == null)
		{
			asset = ScriptableObject.CreateInstance(assetType.Name);
			AssetDatabase.CreateAsset(asset, fullAssetPath);
		}

		asset.hideFlags = HideFlags.DontSaveInEditor;
		return asset;
	}
}


public class ScriptParserEditor : AssetPostprocessor
{
	public static string ExcelRootPath = "../SebamoScript/Excels/";
	public static string CSRootPath = "Assets/Scripts/Datas/Parser/";

	public static readonly int FixedDataStartRow = 2;

	[MenuItem("Tools/Parse Excel")]
	public static void ExcelAutoCodeGenerate()
	{
		string excelPath = ExcelRootPath;

		var excelFilePaths = Directory.GetFiles(excelPath, "*.xlsx", SearchOption.AllDirectories)
			.Where(f => f.Contains("~$") == false)
			.ToArray();

		foreach (var excelFilePath in excelFilePaths)
		{
			GenerateExcelToCSFile(excelFilePath);
		}

		OnPostprocessAllAssets(excelFilePaths, null, null, null);
	}

	private static void GenerateExcelToCSFile(string excelFile)
	{
		using (FileStream fs = new FileStream(excelFile, FileMode.Open, FileAccess.Read))
		{
			var workBook = new XSSFWorkbook(fs);
			string tableName = Path.GetFileNameWithoutExtension(excelFile);

			ExcelToCSFile(workBook, tableName);
		}
	}

	private static void ExcelToCSFile(XSSFWorkbook workBook, string tableName)
	{
		StringBuilder builder = new StringBuilder();

		builder.Append("using System;\n");
		builder.Append("using System.Collections.Generic;\n\n\n");

		builder.Append("/// <summary>\n");
		builder.Append("/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.\n");
		builder.Append("/// </summary>\n");

		builder.Append("[Serializable]\n");
		builder.Append($"[ScriptParserAttribute(\"{tableName}.asset\")]\n");
		builder.Append($"public partial class {tableName} : ScriptParser\n");
		builder.Append("{\n");

		builder.Append("\tpublic override void Parser()\n");
		builder.Append("\t{\n");

		for (int i = 0; i < workBook.NumberOfSheets; i++)
		{
			ISheet sheet = workBook[i];
			string className = sheet.SheetName;

			StringBuilder classFieldNameBuilder = new StringBuilder(className);

			var firstChar = className.FirstOrDefault();
			classFieldNameBuilder[0] = Char.ToLower(firstChar);
			var classFieldName = classFieldNameBuilder.ToString();

			builder.Append($"\t\t{classFieldName}Dictionary.Clear();\n");
			builder.Append($"\t\tforeach(var value in {classFieldName}List)\n");
			builder.Append("\t\t{\n");
			builder.Append($"\t\t\t{classFieldName}Dictionary.Add(value.key, value);\n");
			builder.Append("\t\t}\n");
		}

		builder.Append("\t}\n\n");

		for (int i = 0; i < workBook.NumberOfSheets; i++)
		{
			ISheet sheet = workBook[i];

			SheetToClass(sheet.SheetName, sheet.GetRow(0), sheet.GetRow(1), ref builder);
		}

		builder.Append("\n}");

		string csFileDirectory = CSRootPath + $"{tableName}Folder";
		MakeCSFile(ref builder, csFileDirectory, $"{csFileDirectory}/{tableName}.Helper.cs", $"{csFileDirectory}/{tableName}.cs");
	}

	private static void MakeCSFile(ref StringBuilder builder, string directoryPath, string fileHelperPath, string filePath)
	{
		// 클래스 디렉터리 제작
		if (!Directory.Exists(directoryPath))
			Directory.CreateDirectory(directoryPath);

		// 클래스 헬퍼 파일 제작, 단 이미 존재하는 경우 무시
		if (!File.Exists(fileHelperPath))
			File.Create(fileHelperPath);

		// 클래스 파일 제작
		using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
		{
			var writer = new StreamWriter(fs, Encoding.UTF8);

			writer.WriteLine(builder);

			writer.Close();
			writer.Dispose();
		}
	}

	private static void SheetToClass(string className, IRow typeRow, IRow nameRow, ref StringBuilder builder)
	{
		StringBuilder classFieldNameBuilder = new StringBuilder(className);
		
		var firstChar = className.FirstOrDefault();
		classFieldNameBuilder[0] = Char.ToLower(firstChar);
		var classFieldName = classFieldNameBuilder.ToString();

		builder.Append("\t[Serializable]\n");
		builder.Append($"\tpublic class {className}\n");
		builder.Append("\t{\n");

		if (typeRow.LastCellNum != nameRow.LastCellNum)
			return;

		builder.Append($"\t\tpublic {typeRow.GetCell(0)} key;\n");

		for (int i = 1; i < typeRow.LastCellNum; i++)
		{
			var typeCell = typeRow.GetCell(i);
			var nameCell = nameRow.GetCell(i);

			builder.Append($"\t\tpublic {typeCell} {nameCell};\n");
		}

		builder.Append("\t}\n\n");

		builder.Append($"\tpublic List<{className}> {classFieldName}List = new List<{className}>();\n");
		builder.Append("\t[System.Serializable]\n");
		builder.Append($"\tpublic class {className}Dictionary : SerializableDictionary<{typeRow.GetCell(0)}, {className}> " + "{}\n");
		builder.Append($"\tpublic {className}Dictionary {classFieldName}Dictionary = new {className}Dictionary();\n\n");
	}

	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		bool imported = false;

		foreach (string path in importedAssets)
		{
			if (Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx")
			{
				var excelName = Path.GetFileNameWithoutExtension(path);
				if (excelName.StartsWith("~$")) continue;

				var info = FindAssetTypeByExcelName(excelName);

				if (info == null)
				{
					Debug.LogWarning("ExcelAssetScript is not found. Select " + path + " and excute 'Create/ExcelAssetScript' from create menu.");
					continue;
				}

				ImportExcel(path, info);
				imported = true;
			}
		}

		if (imported)
		{
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}

	private static ScriptAssetInfo FindAssetTypeByExcelName(string excelName)
	{
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			var type = assembly.GetType(excelName);
			if (type == null) continue;

			var attributes = type.GetCustomAttributes(typeof(ScriptParserAttribute), false);
			if (attributes.Length == 0) continue;

			var attribute = (ScriptParserAttribute)attributes[0];

			return new ScriptAssetInfo()
			{
				AssetType = type,
				Attribute = attribute
			};
		}

		return null;
	}

	private static IWorkbook LoadBook(string excelPath)
	{
		using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			if (Path.GetExtension(excelPath) == ".xls") return new HSSFWorkbook(stream);
			else return new XSSFWorkbook(stream);
		}
	}

	private static List<string> GetFieldNamesFromSheetHeader(ISheet sheet)
	{
		IRow headerRow = sheet.GetRow(1);

		var fieldNames = new List<string>() { "key" };

		for (int i = 1; i < headerRow.LastCellNum; i++)
		{
			var cell = headerRow.GetCell(i);
			if (cell == null || cell.CellType == CellType.Blank) break;
			fieldNames.Add(cell.StringCellValue);
		}

		return fieldNames;
	}

	private static object CellToFieldObject(ICell cell, FieldInfo fieldInfo, bool isFormulaEvalute = false)
	{
		var type = isFormulaEvalute ? cell.CachedFormulaResultType : cell.CellType;

		switch (type)
		{
			case CellType.String:
				if (fieldInfo.FieldType.IsEnum) return Enum.Parse(fieldInfo.FieldType, cell.StringCellValue);
				else return cell.StringCellValue;
			case CellType.Boolean:
				return cell.BooleanCellValue;
			case CellType.Numeric:
				return Convert.ChangeType(cell.NumericCellValue, fieldInfo.FieldType);
			case CellType.Formula:
				if (isFormulaEvalute) return null;
				return CellToFieldObject(cell, fieldInfo, true);
			default:
				return null;
		}
	}

	private static object CreateEntityFromRow(IRow row, List<string> columnNames, Type entityType)
	{
		var entity = Activator.CreateInstance(entityType);

		for (int i = 0; i < columnNames.Count; i++)
		{
			FieldInfo entityField = entityType.GetField(
				columnNames[i],
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
			);
			if (entityField == null) continue;
			if (!entityField.IsPublic && entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;

			ICell cell = row.GetCell(i);

			try
			{
				object fieldValue = CellToFieldObject(cell, entityField);
				entityField.SetValue(entity, fieldValue);
			}
			catch
			{
				throw new Exception(string.Format("Invalid excel cell type at row {0}, column {1} : 앤티티 타입 {2}, 컬럼 네임 {3}.", row.RowNum, cell.ColumnIndex, entityType.ToString(), columnNames[i]));
			}
		}
		return entity;
	}

	private static object GetEntityCollectionFromSheet(ISheet sheet, Type entityType)
	{
		List<string> excelColumnNames = GetFieldNamesFromSheetHeader(sheet);

		Type collectionType = typeof(List<>).MakeGenericType(entityType);
		MethodInfo collectionAddMethod = collectionType.GetMethod("Add", new Type[] { entityType });

		object collection = Activator.CreateInstance(collectionType);

		// row of index 0 is header
		for (int i = 2; i <= sheet.LastRowNum; i++)
		{
			IRow row = sheet.GetRow(i);
			if (row == null) break;

			ICell entryCell = row.GetCell(0);
			if (entryCell == null || entryCell.CellType == CellType.Blank) break;

			// skip comment row
			if (entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;

			var entity = CreateEntityFromRow(row, excelColumnNames, entityType);

			collectionAddMethod.Invoke(collection, new object[] { entity });
		}

		return collection;
	}

	private static void ImportExcel(string excelPath, ScriptAssetInfo info)
	{
		string assetPath = info.Attribute.resourcePath;

		UnityEngine.Object asset = ScriptableObjectHelper.GenerateSO(assetPath, info.AssetType);

		IWorkbook book = LoadBook(excelPath);

		var assetFields = info.AssetType.GetFields();

		foreach (var assetField in assetFields)
		{
			Type fieldType = assetField.FieldType;
			if (!fieldType.IsGenericType) continue;

			string fieldName = string.Empty;

			if (fieldType.GetGenericTypeDefinition() == typeof(List<>))
				fieldName = assetField.Name.Replace("List", "");

			ISheet sheet = book.GetSheet(fieldName); // 엑셀의 시트의 이름과 필드 네임이 같아야 합니다.
			if (sheet == null) continue;

			Type[] types = fieldType.GetGenericArguments();
			Type entityType = types[0];

			object entities = GetEntityCollectionFromSheet(sheet, entityType);
			assetField.SetValue(asset, entities);
		}

		ScriptParser parser = asset as ScriptParser;
		parser?.Parser();
		parser?.RuntimeParser();

		EditorUtility.SetDirty(parser);
	}
}
#endif