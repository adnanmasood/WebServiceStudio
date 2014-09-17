using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Dumper
{
    public class Dumper
    {
        private readonly Hashtable objects;
        private readonly TextWriter writer;
        private int indent;

        public Dumper() : this(Console.Out)
        {
        }

        public Dumper(TextWriter writer)
        {
            objects = new Hashtable();
            indent = 0;
            this.writer = writer;
        }

        public static void Dump(string name, object o)
        {
            new Dumper().DumpInternal(name, o);
        }

        private void DumpInternal(string name, object o)
        {
            for (int i = 0; i < indent; i++)
            {
                writer.Write("--- ");
            }
            if (name == null)
            {
                name = string.Empty;
            }
            if (o == null)
            {
                writer.WriteLine(name + " = null");
            }
            else
            {
                Type c = o.GetType();
                writer.Write(c.Name + " " + name);
                if (objects[o] != null)
                {
                    writer.WriteLine(" = ...");
                }
                else
                {
                    if (!c.IsValueType && (c != typeof (string)))
                    {
                        objects.Add(o, o);
                    }
                    if (c.IsArray)
                    {
                        var array = (Array) o;
                        writer.WriteLine();
                        indent++;
                        for (int j = 0; j < array.Length; j++)
                        {
                            DumpInternal("[" + j + "]", array.GetValue(j));
                        }
                        indent--;
                    }
                    else if (o is XmlQualifiedName)
                    {
                        DumpInternal("Name", ((XmlQualifiedName) o).Name);
                        DumpInternal("Namespace", ((XmlQualifiedName) o).Namespace);
                    }
                    else if (o is XmlNode)
                    {
                        writer.WriteLine(" = " + ((XmlNode) o).OuterXml.Replace('\n', ' ').Replace('\r', ' '));
                    }
                    else if (c.IsEnum)
                    {
                        writer.WriteLine(" = " + o);
                    }
                    else if (c.IsPrimitive)
                    {
                        writer.WriteLine(" = " + o);
                    }
                    else if (typeof (Exception).IsAssignableFrom(c))
                    {
                        writer.WriteLine(" = " + ((Exception) o).Message);
                    }
                    else if (o is DataSet)
                    {
                        writer.WriteLine();
                        indent++;
                        DumpInternal("Tables", ((DataSet) o).Tables);
                        indent--;
                    }
                    else if (o is DateTime)
                    {
                        writer.WriteLine(" = " + o);
                    }
                    else if (o is DataTable)
                    {
                        writer.WriteLine();
                        indent++;
                        var table = (DataTable) o;
                        DumpInternal("TableName", table.TableName);
                        DumpInternal("Rows", table.Rows);
                        indent--;
                    }
                    else if (o is DataRow)
                    {
                        writer.WriteLine();
                        indent++;
                        var row = (DataRow) o;
                        DumpInternal("Values", row.ItemArray);
                        indent--;
                    }
                    else if (o is string)
                    {
                        var str2 = (string) o;
                        if (str2.Length > 40)
                        {
                            writer.WriteLine(" = ");
                            writer.WriteLine("\"" + str2 + "\"");
                        }
                        else
                        {
                            writer.WriteLine(" = \"" + str2 + "\"");
                        }
                    }
                    else if (o is IEnumerable)
                    {
                        IEnumerator enumerator = ((IEnumerable) o).GetEnumerator();
                        if (enumerator == null)
                        {
                            writer.WriteLine(" GetEnumerator() == null");
                        }
                        else
                        {
                            writer.WriteLine();
                            int num3 = 0;
                            indent++;
                            while (enumerator.MoveNext())
                            {
                                DumpInternal("[" + num3 + "]", enumerator.Current);
                                num3++;
                            }
                            indent--;
                        }
                    }
                    else
                    {
                        writer.WriteLine();
                        indent++;
                        if (typeof (Type).IsAssignableFrom(c) || typeof (PropertyInfo).IsAssignableFrom(c))
                        {
                            foreach (PropertyInfo info in c.GetProperties())
                            {
                                if ((info.CanRead &&
                                     (typeof (IEnumerable).IsAssignableFrom(info.PropertyType) || info.CanWrite)) &&
                                    (info.PropertyType != c))
                                {
                                    object obj2;
                                    try
                                    {
                                        obj2 = info.GetValue(o, null);
                                    }
                                    catch (Exception exception)
                                    {
                                        obj2 = exception;
                                    }
                                    DumpInternal(info.Name, obj2);
                                }
                            }
                        }
                        else
                        {
                            foreach (
                                FieldInfo info2 in
                                    c.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                            {
                                if (!info2.IsStatic)
                                {
                                    DumpInternal(info2.Name, info2.GetValue(o));
                                }
                            }
                        }
                        indent--;
                    }
                }
            }
        }
    }
}