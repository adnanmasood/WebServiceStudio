using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace WebServiceStudio
{
    internal class Script
    {
        private static string dumpClassCS =
            "\r\npublic class Dumper {\r\n    Hashtable objects = new Hashtable();\r\n    TextWriter writer;\r\n    int indent = 0;\r\n\r\n    public Dumper():this(Console.Out) {\r\n    }\r\n    public Dumper(TextWriter writer) {\r\n        this.writer = writer;\r\n    }\r\n    public static void Dump(string name, object o){\r\n        Dumper d = new Dumper();\r\n        d.DumpInternal(name, o);\r\n    }\r\n\r\n    private void DumpInternal(string name, object o) {\r\n        for (int i1 = 0; i1 < indent; i1++)\r\n            writer.Write(\"--- \");\r\n\r\n        if (name == null) name = string.Empty;\r\n\r\n        if (o == null) {\r\n            writer.WriteLine(name + \" = null\");\r\n            return;\r\n        }\r\n\r\n        Type type = o.GetType();\r\n\r\n        writer.Write(type.Name + \" \" + name);\r\n\r\n        if (objects[o] != null) {\r\n            writer.WriteLine(\" = ...\");\r\n            return;\r\n        }\r\n\r\n        if (!type.IsValueType && !type.Equals(typeof(string)))\r\n            objects.Add(o, o);\r\n\r\n        if (type.IsArray) {\r\n            Array a = (Array)o;\r\n            writer.WriteLine();\r\n            indent++;\r\n            for (int j = 0; j < a.Length; j++)\r\n                DumpInternal(\"[\" + j + \"]\", a.GetValue(j));\r\n            indent--;\r\n            return;\r\n        }\r\n        if (o is XmlQualifiedName) {\r\n            DumpInternal(\"Name\", ((XmlQualifiedName) o).Name);\r\n            DumpInternal(\"Namespace\", ((XmlQualifiedName) o).Namespace);\r\n            return;\r\n        }\r\n        if (o is XmlNode) {\r\n            string xml = ((XmlNode)o).OuterXml;\r\n            writer.WriteLine(\" = \" + xml);\r\n            return;\r\n        }\r\n        if (type.IsEnum) {\r\n            writer.WriteLine(\" = \" + ((Enum)o).ToString());\r\n            return;\r\n        }\r\n        if (type.IsPrimitive) {\r\n            writer.WriteLine(\" = \" + o.ToString());\r\n            return;\r\n        }\r\n        if (typeof(Exception).IsAssignableFrom(type)) {                \r\n            writer.WriteLine(\" = \" + ((Exception)o).Message);\r\n            return;\r\n        }\r\n        if (o is DataSet) {\r\n            writer.WriteLine();\r\n            indent++;\r\n            DumpInternal(\"Tables\", ((DataSet)o).Tables);\r\n            indent--;\r\n            return;\r\n        }\r\n        if (o is DateTime) {\r\n            writer.WriteLine(\" = \" + o.ToString());\r\n            return;\r\n        }\r\n        if (o is DataTable) {\r\n            writer.WriteLine();\r\n            indent++;\r\n            DataTable table = (DataTable)o;\r\n            DumpInternal(\"TableName\", table.TableName);\r\n            DumpInternal(\"Rows\", table.Rows);\r\n            indent--;\r\n            return;\r\n        }\r\n        if (o is DataRow) {\r\n            writer.WriteLine();\r\n            indent++;\r\n            DataRow row = (DataRow)o;\r\n            DumpInternal(\"Values\", row.ItemArray);\r\n            indent--;\r\n            return;\r\n        }\r\n        if (o is string) {\r\n            string s = (string)o;\r\n            if (s.Length > 40) {\r\n                writer.WriteLine(\" = \");\r\n                writer.WriteLine(\"\\\"\" + s + \"\\\"\");\r\n            }\r\n            else {\r\n                writer.WriteLine(\" = \\\"\" + s + \"\\\"\");\r\n            }\r\n            return;\r\n        }\r\n        if (o is IEnumerable) {\r\n            IEnumerator e = ((IEnumerable)o).GetEnumerator();\r\n            if (e == null) {\r\n                writer.WriteLine(\" GetEnumerator() == null\");\r\n                return;\r\n            }\r\n            writer.WriteLine();\r\n            int c = 0;\r\n            indent++;\r\n            while (e.MoveNext()) {\r\n                DumpInternal(\"[\" + c + \"]\", e.Current);\r\n                c++;\r\n            }\r\n            indent--;\r\n            return;\r\n        }\r\n        writer.WriteLine();\r\n        indent++;\r\n        FieldInfo[] fields = type.GetFields();\r\n        for (int i2 = 0; i2 < fields.Length; i2++) {\r\n            FieldInfo f = fields[i2];\r\n            if (!f.IsStatic)\r\n                DumpInternal(f.Name, f.GetValue(o));\r\n        }\r\n        PropertyInfo[] props = type.GetProperties();\r\n        for (int i3 = 0; i3 < props.Length; i3++) {\r\n            PropertyInfo p = props[i3];\r\n            if (p.CanRead &&\r\n                  (typeof(IEnumerable).IsAssignableFrom(p.PropertyType) || p.CanWrite) &&\r\n                  !p.PropertyType.Equals(type)){\r\n                object v;\r\n                try {\r\n                    v = p.GetValue(o, null);\r\n                }\r\n                catch (Exception e) {\r\n                    v = e;\r\n                }\r\n                DumpInternal(p.Name, v);\r\n            }\r\n        }\r\n        indent--;\r\n    }\r\n}\r\n";

        private static string dumpClassVB =
            "Public Class Dumper\r\n   Private objects As New Hashtable()\r\n   Private writer As TextWriter\r\n   Private indent As Integer = 0\r\n   \r\n   \r\n   Public Sub New()\r\n      MyClass.New(Console.Out)\r\n   End Sub 'New\r\n   \r\n   Public Sub New(writer As TextWriter)\r\n      Me.writer = writer\r\n   End Sub 'New\r\n   \r\n   Public Shared Sub Dump(name As String, o As Object)\r\n      Dim d As New Dumper()\r\n      d.DumpInternal(name, o)\r\n   End Sub 'Dump\r\n   \r\n   \r\n   Private Sub DumpInternal(name As String, o As Object)\r\n      Dim i1 As Integer\r\n      For i1 = 0 To indent - 1\r\n         writer.Write(\"--- \")\r\n      Next i1 \r\n      If name Is Nothing Then\r\n         name = String.Empty\r\n      End If \r\n      If o Is Nothing Then\r\n         writer.WriteLine((name + \" = null\"))\r\n         Return\r\n      End If\r\n      \r\n      Dim type As Type = o.GetType()\r\n      \r\n      writer.Write((type.Name + \" \" + name))\r\n      \r\n      If Not (objects(o) Is Nothing) Then\r\n         writer.WriteLine(\" = ...\")\r\n         Return\r\n      End If\r\n      \r\n      If Not type.IsValueType And Not type.Equals(GetType(String)) Then\r\n         objects.Add(o, o)\r\n      End If \r\n      If type.IsArray Then\r\n         Dim a As Array = CType(o, Array)\r\n         writer.WriteLine()\r\n         indent += 1\r\n         Dim j As Integer\r\n         For j = 0 To a.Length - 1\r\n            DumpInternal(\"[\" + j + \"]\", a.GetValue(j))\r\n         Next j\r\n         indent -= 1\r\n         Return\r\n      End If\r\n      If TypeOf o Is XmlQualifiedName Then\r\n         DumpInternal(\"Name\", CType(o, XmlQualifiedName).Name)\r\n         DumpInternal(\"Namespace\", CType(o, XmlQualifiedName).Namespace)\r\n         Return\r\n      End If\r\n      If TypeOf o Is XmlNode Then\r\n         Dim xml As String = CType(o, XmlNode).OuterXml\r\n         writer.WriteLine((\" = \" + xml))\r\n         Return\r\n      End If\r\n      If type.IsEnum Then\r\n         writer.WriteLine((\" = \" + CType(o, [Enum]).ToString()))\r\n         Return\r\n      End If\r\n      If type.IsPrimitive Then\r\n         writer.WriteLine((\" = \" + o.ToString()))\r\n         Return\r\n      End If\r\n      If GetType(Exception).IsAssignableFrom(type) Then\r\n         writer.WriteLine((\" = \" + CType(o, Exception).Message))\r\n         Return\r\n      End If\r\n      If TypeOf o Is DataSet Then\r\n         writer.WriteLine()\r\n         indent += 1\r\n         DumpInternal(\"Tables\", CType(o, DataSet).Tables)\r\n         indent -= 1\r\n         Return\r\n      End If\r\n      If TypeOf o Is DateTime Then\r\n         writer.WriteLine((\" = \" + o.ToString()))\r\n         Return\r\n      End If\r\n      If TypeOf o Is DataTable Then\r\n         writer.WriteLine()\r\n         indent += 1\r\n         Dim table As DataTable = CType(o, DataTable)\r\n         DumpInternal(\"TableName\", table.TableName)\r\n         DumpInternal(\"Rows\", table.Rows)\r\n         indent -= 1\r\n         Return\r\n      End If\r\n      If TypeOf o Is DataRow Then\r\n         writer.WriteLine()\r\n         indent += 1\r\n         Dim row As DataRow = CType(o, DataRow)\r\n         DumpInternal(\"Values\", row.ItemArray)\r\n         indent -= 1\r\n         Return\r\n      End If\r\n      If TypeOf o Is String Then\r\n         Dim s As String = CStr(o)\r\n         If s.Length > 40 Then\r\n            writer.WriteLine(\" = \")\r\n            writer.WriteLine((\"\"\"\" + s + \"\"\"\"))\r\n         Else\r\n            writer.WriteLine((\" = \"\"\" + s + \"\"\"\"))\r\n         End If\r\n         Return\r\n      End If\r\n      If TypeOf o Is IEnumerable Then\r\n         Dim e As IEnumerator = CType(o, IEnumerable).GetEnumerator()\r\n         If e Is Nothing Then\r\n            writer.WriteLine(\" GetEnumerator() == null\")\r\n            Return\r\n         End If\r\n         writer.WriteLine()\r\n         Dim c As Integer = 0\r\n         indent += 1\r\n         While e.MoveNext()\r\n            DumpInternal(\"[\" + c + \"]\", e.Current)\r\n            c += 1\r\n         End While\r\n         indent -= 1\r\n         Return\r\n      End If\r\n      writer.WriteLine()\r\n      indent += 1\r\n      Dim fields As FieldInfo() = type.GetFields()\r\n      Dim i2 As Integer\r\n      For i2 = 0 To fields.Length - 1\r\n         Dim f As FieldInfo = fields(i2)\r\n         If Not f.IsStatic Then\r\n            DumpInternal(f.Name, f.GetValue(o))\r\n         End If\r\n      Next i2\r\n      Dim props As PropertyInfo() = type.GetProperties()\r\n      Dim i3 As Integer\r\n      For i3 = 0 To props.Length - 1\r\n         Dim p As PropertyInfo = props(i3)\r\n         If p.CanRead And(GetType(IEnumerable).IsAssignableFrom(p.PropertyType) Or p.CanWrite) And Not p.PropertyType.Equals(type) Then\r\n            Dim v As Object\r\n            Try\r\n               v = p.GetValue(o, Nothing)\r\n            Catch e As Exception\r\n               v = e\r\n            End Try\r\n            DumpInternal(p.Name, v)\r\n         End If\r\n      Next i3\r\n      indent -= 1\r\n   End Sub 'DumpInternal\r\nEnd Class 'Dumper\r\n";

        private static string usingStatementsCS =
            "\r\nusing System.CodeDom.Compiler;\r\nusing System.CodeDom;\r\nusing System.Collections;\r\nusing System.ComponentModel;\r\nusing System.Data;\r\nusing System.Globalization;\r\nusing System.IO;\r\nusing System.Reflection;\r\nusing System.Web.Services.Protocols;\r\nusing System.Xml.Serialization;\r\nusing System.Xml;\r\nusing System;\r\n";

        private static string usingStatementsVB =
            "\r\nImports System.CodeDom.Compiler\r\nImports System.CodeDom\r\nImports System.Collections\r\nImports System.ComponentModel\r\nImports System.Data\r\nImports System.Globalization\r\nImports System.IO\r\nImports System.Reflection\r\nImports System.Web.Services.Protocols\r\nImports System.Xml.Serialization\r\nImports System.Xml\r\nImports System\r\n";

        private readonly CodeTypeDeclaration codeClass;
        private readonly CodeCompileUnit compileUnit;

        private readonly CodeMethodReferenceExpression dumpMethodRef;
        private readonly CodeEntryPointMethod mainMethod;
        private readonly ProxySettings proxySetting;
        private CodeNamespace codeNamespace;
        private HttpWebClientProtocol proxy;

        public Script() : this("WebServiceStudio", "MainClass")
        {
        }

        public Script(string namespaceToGen, string className)
        {
            compileUnit = new CodeCompileUnit();
            codeNamespace = new CodeNamespace(namespaceToGen);
            compileUnit.Namespaces.Add(codeNamespace);
            codeClass = new CodeTypeDeclaration(className);
            codeNamespace.Types.Add(codeClass);
            proxySetting = ProxySettings.RequiredHeaders;
            mainMethod = new CodeEntryPointMethod();
            mainMethod.Name = "Main";
            mainMethod.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            codeClass.Members.Add(mainMethod);
            dumpMethodRef = BuildDumper();
        }

        public HttpWebClientProtocol Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }

        public void AddMethod(MethodInfo method, object[] parameters)
        {
            BuildMethod(codeClass.Members, method, parameters);
        }

        private CodeExpression BuildArray(CodeStatementCollection statements, string name, object value)
        {
            var array = (Array) value;
            Type type = value.GetType();
            string uniqueVariableName = GetUniqueVariableName(name, statements);
            var statement = new CodeVariableDeclarationStatement(type.FullName, uniqueVariableName);
            statement.InitExpression = new CodeArrayCreateExpression(type.GetElementType(), array.Length);
            statements.Add(statement);
            var targetObject = new CodeVariableReferenceExpression(uniqueVariableName);
            string str2 = name + "_";
            for (int i = 0; i < array.Length; i++)
            {
                var left = new CodeArrayIndexerExpression(targetObject, new CodeExpression[0]);
                left.Indices.Add(new CodePrimitiveExpression(i));
                CodeExpression right = BuildObject(statements, str2 + i, array.GetValue(i));
                statements.Add(new CodeAssignStatement(left, right));
            }
            return targetObject;
        }

        private CodeExpression BuildClass(CodeStatementCollection statements, string name, object value)
        {
            Type type = value.GetType();
            string uniqueVariableName = GetUniqueVariableName(name, statements);
            var statement = new CodeVariableDeclarationStatement(type.FullName, uniqueVariableName);
            statement.InitExpression = new CodeObjectCreateExpression(type.FullName, new CodeExpression[0]);
            statements.Add(statement);
            var targetObject = new CodeVariableReferenceExpression(uniqueVariableName);
            foreach (
                MemberInfo info in type.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                object obj2 = null;
                Type fieldType = typeof (object);
                CodeExpression left = null;
                if (info is FieldInfo)
                {
                    var info2 = (FieldInfo) info;
                    if (info2.IsStatic || info2.IsInitOnly)
                    {
                        goto Label_014B;
                    }
                    fieldType = info2.FieldType;
                    obj2 = info2.GetValue(value);
                    left = new CodeFieldReferenceExpression(targetObject, info2.Name);
                }
                else if (info is PropertyInfo)
                {
                    var info3 = (PropertyInfo) info;
                    if (!info3.CanWrite)
                    {
                        goto Label_014B;
                    }
                    MethodInfo getMethod = info3.GetGetMethod();
                    if ((getMethod.GetParameters().Length > 0) || getMethod.IsStatic)
                    {
                        goto Label_014B;
                    }
                    fieldType = info3.PropertyType;
                    obj2 = info3.GetValue(value, null);
                    left = new CodePropertyReferenceExpression(targetObject, info3.Name);
                }
                if (left != null)
                {
                    CodeExpression right = BuildObject(statements, info.Name, obj2);
                    statements.Add(new CodeAssignStatement(left, right));
                }
                Label_014B:
                ;
            }
            return targetObject;
        }

        public CodeMethodReferenceExpression BuildDumper()
        {
            return new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("Dumper"), "Dump");
        }

        private void BuildDumpInvoke(CodeStatementCollection statements, string name, CodeExpression obj)
        {
            var expression = new CodeMethodInvokeExpression(dumpMethodRef, new CodeExpression[0]);
            expression.Parameters.Add(new CodePrimitiveExpression(name));
            expression.Parameters.Add(obj);
            statements.Add(expression);
        }

        private void BuildMethod(CodeTypeMemberCollection members, MethodInfo method, object[] parameters)
        {
            var method2 = new CodeMemberMethod();
            method2.Name = "Invoke" + method.Name;
            mainMethod.Statements.Add(new CodeMethodInvokeExpression(null, method2.Name, new CodeExpression[0]));
            method2.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            members.Add(method2);
            var expression = new CodeMethodInvokeExpression(BuildProxy(method2.Statements, method), method.Name,
                new CodeExpression[0]);
            BuildParameters(method2.Statements, method, parameters, expression.Parameters);
            if (method.ReturnType == typeof (void))
            {
                method2.Statements.Add(new CodeExpressionStatement(expression));
            }
            else
            {
                string uniqueVariableName = GetUniqueVariableName(method.Name + "Result", method2.Statements);
                method2.Statements.Add(new CodeVariableDeclarationStatement(method.ReturnType.FullName,
                    uniqueVariableName, expression));
                BuildDumpInvoke(method2.Statements, "result", new CodeVariableReferenceExpression(uniqueVariableName));
            }
            ParameterInfo[] infoArray = method.GetParameters();
            for (int i = 0; i < infoArray.Length; i++)
            {
                ParameterInfo info = infoArray[i];
                if (info.IsOut || info.ParameterType.IsByRef)
                {
                    BuildDumpInvoke(method2.Statements, info.Name,
                        ((CodeDirectionExpression) expression.Parameters[i]).Expression);
                }
            }
        }

        private CodeExpression BuildObject(CodeStatementCollection statements, string name, object value)
        {
            if (value == null)
            {
                return new CodePrimitiveExpression(null);
            }
            Type c = value.GetType();
            if (c.IsPrimitive || (c == typeof (string)))
            {
                return new CodePrimitiveExpression(value);
            }
            if (c.IsEnum)
            {
                string[] strArray = value.ToString().Split(new[] {','});
                if (strArray.Length > 1)
                {
                    CodeExpression left = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(c.FullName),
                        strArray[0]);
                    for (int i = 1; i < strArray.Length; i++)
                    {
                        left = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.BitwiseOr,
                            new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(c.FullName), strArray[i]));
                    }
                    return left;
                }
                return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(c.FullName), value.ToString());
            }
            if (c == typeof (DateTime))
            {
                var time = (DateTime) value;
                string str = ((DateTime) value).ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz",
                    DateTimeFormatInfo.InvariantInfo);
                long ticks = time.Ticks;
                statements.Add(new CodeCommentStatement("Init DateTime object value = " + str));
                statements.Add(new CodeCommentStatement("We going to use DateTime ctor that takes Ticks"));
                return new CodeObjectCreateExpression(new CodeTypeReference(typeof (DateTime)),
                    new CodeExpression[] {new CodePrimitiveExpression(ticks)});
            }
            if (typeof (XmlNode).IsAssignableFrom(c))
            {
                return BuildXmlNode(statements, name, value);
            }
            if (c.IsArray)
            {
                return BuildArray(statements, name, value);
            }
            if (c.IsAbstract || (c.GetConstructor(new Type[0]) == null))
            {
                statements.Add(
                    new CodeCommentStatement("Can not create object of type " + c.FullName +
                                             " because it does not have a default ctor"));
                return new CodePrimitiveExpression(null);
            }
            return BuildClass(statements, name, value);
        }

        private void BuildParameters(CodeStatementCollection statements, MethodInfo method, object[] paramValues,
            CodeExpressionCollection parameters)
        {
            ParameterInfo[] infoArray = method.GetParameters();
            for (int i = 0; i < infoArray.Length; i++)
            {
                ParameterInfo info = infoArray[i];
                Type parameterType = infoArray[i].ParameterType;
                var @in = FieldDirection.In;
                if (parameterType.IsByRef)
                {
                    @in = FieldDirection.Ref;
                    parameterType = parameterType.GetElementType();
                }
                CodeExpression expression = null;
                if (!info.IsOut)
                {
                    expression = BuildObject(statements, info.Name, paramValues[i]);
                }
                else
                {
                    @in = FieldDirection.Out;
                }
                if (@in != FieldDirection.In)
                {
                    if ((expression == null) || !(expression is CodeVariableReferenceExpression))
                    {
                        var statement = new CodeVariableDeclarationStatement(parameterType.FullName, info.Name);
                        if (expression != null)
                        {
                            statement.InitExpression = expression;
                        }
                        statements.Add(statement);
                        expression = new CodeVariableReferenceExpression(statement.Name);
                    }
                    expression = new CodeDirectionExpression(@in, expression);
                }
                parameters.Add(expression);
            }
        }

        private CodeExpression BuildProxy(CodeStatementCollection statements, MethodInfo method)
        {
            Type type = proxy.GetType();
            string name = CodeIdentifier.MakeCamel(type.Name);
            if (proxySetting == ProxySettings.AllProperties)
            {
                return BuildClass(statements, name, proxy);
            }
            var statement = new CodeVariableDeclarationStatement(type.Name, name);
            statement.InitExpression = new CodeObjectCreateExpression(type.FullName, new CodeExpression[0]);
            statements.Add(statement);
            CodeExpression targetObject = new CodeVariableReferenceExpression(name);
            FieldInfo[] soapHeaders = null;
            if (proxySetting == ProxySettings.RequiredHeaders)
            {
                soapHeaders = MethodProperty.GetSoapHeaders(method, true);
            }
            else
            {
                soapHeaders = type.GetFields();
            }
            for (int i = 0; i < soapHeaders.Length; i++)
            {
                FieldInfo info = soapHeaders[i];
                if (typeof (SoapHeader).IsAssignableFrom(info.FieldType))
                {
                    CodeExpression left = new CodeFieldReferenceExpression(targetObject, info.Name);
                    CodeExpression right = BuildObject(statements, info.Name, info.GetValue(proxy));
                    statements.Add(new CodeAssignStatement(left, right));
                }
            }
            return targetObject;
        }

        private CodeExpression BuildXmlNode(CodeStatementCollection statements, string name, object value)
        {
            Type type = value.GetType();
            if (type == typeof (XmlElement))
            {
                var element = (XmlElement) value;
                string str = GetUniqueVariableName(name + "Doc", statements);
                var statement = new CodeVariableDeclarationStatement(typeof (XmlDocument), str);
                statement.InitExpression = new CodeObjectCreateExpression(typeof (XmlDocument), new CodeExpression[0]);
                statements.Add(statement);
                var expression = new CodeVariableReferenceExpression(str);
                var expression2 = new CodeMethodInvokeExpression(expression, "LoadXml", new CodeExpression[0]);
                expression2.Parameters.Add(new CodePrimitiveExpression(element.OuterXml));
                statements.Add(expression2);
                return new CodeFieldReferenceExpression(expression, "DocumentElement");
            }
            if (type != typeof (XmlAttribute))
            {
                throw new Exception("Unsupported XmlNode type");
            }
            var attribute = (XmlAttribute) value;
            string uniqueVariableName = GetUniqueVariableName(name + "Doc", statements);
            var statement2 = new CodeVariableDeclarationStatement(typeof (XmlDocument), uniqueVariableName);
            statement2.InitExpression = new CodeObjectCreateExpression(typeof (XmlDocument), new CodeExpression[0]);
            statements.Add(statement2);
            var targetObject = new CodeVariableReferenceExpression(uniqueVariableName);
            var expression4 = new CodeMethodInvokeExpression(targetObject, "CreateAttribute", new CodeExpression[0]);
            expression4.Parameters.Add(new CodePrimitiveExpression(attribute.Name));
            expression4.Parameters.Add(new CodePrimitiveExpression(attribute.NamespaceURI));
            string str3 = GetUniqueVariableName(name + "Attr", statements);
            var statement3 = new CodeVariableDeclarationStatement(typeof (XmlAttribute), str3);
            statement3.InitExpression = expression4;
            statements.Add(statement3);
            var expression5 = new CodeVariableReferenceExpression(str3);
            statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(expression5, "Value"),
                new CodePrimitiveExpression(attribute.Value)));
            return expression5;
        }

        public string Generate(ICodeGenerator codeGen)
        {
            var w = new StringWriter();
            codeGen.GenerateCodeFromCompileUnit(compileUnit, w, null);
            return w.ToString();
        }

        public static string GetDumpCode(Language language)
        {
            if (language == Language.CS)
            {
                return dumpClassCS;
            }
            if (language == Language.VB)
            {
                return dumpClassVB;
            }
            return ("*** Dump classes is not generated for " + language + " ***");
        }

        private static string GetUniqueVariableName(string name, CodeStatementCollection statements)
        {
            name = CodeIdentifier.MakeCamel(name);
            foreach (CodeStatement statement in statements)
            {
                var statement2 = statement as CodeVariableDeclarationStatement;
                if ((statement2 != null) && (statement2.Name == name))
                {
                    return (name + "_" + statements.Count);
                }
            }
            return name;
        }

        public static string GetUsingCode(Language language)
        {
            if (language == Language.CS)
            {
                return usingStatementsCS;
            }
            if (language == Language.VB)
            {
                return usingStatementsVB;
            }
            return "";
        }

        private bool IsCLSCompliant(Type type)
        {
            var customAttributes =
                type.GetCustomAttributes(typeof (CLSCompliantAttribute), true) as CLSCompliantAttribute[];
            if (customAttributes.Length != 1)
            {
                return false;
            }
            return customAttributes[0].IsCompliant;
        }

        private enum ProxySettings
        {
            RequiredHeaders,
            AllHeaders,
            AllProperties
        }
    }
}