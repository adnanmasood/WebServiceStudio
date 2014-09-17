using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing.Design;

namespace WebServiceStudio
{
    public class DynamicEditor : UITypeEditor
    {
        private static readonly Hashtable editorTable;

        private static string typeNotFoundMessage =
            "ProxyPropertiesType {0} specified in WebServiceStudio.exe.options is not found";

        static DynamicEditor()
        {
            editorTable = new Hashtable();
            editorTable[typeof (object)] = Activator.CreateInstance(typeof (UITypeEditor));
            editorTable[typeof (DataSet)] = Activator.CreateInstance(typeof (DataSetEditor));
            editorTable[typeof (DateTime)] = Activator.CreateInstance(typeof (DateTimeEditor));
            foreach (CustomHandler handler in Configuration.MasterConfig.DataEditors)
            {
                Type type = Type.GetType(handler.TypeName);
                if (type == null)
                {
                    MainForm.ShowMessage(typeof (DynamicEditor), MessageType.Warning,
                        string.Format(typeNotFoundMessage, handler.TypeName));
                }
                else
                {
                    Type type2 = Type.GetType(handler.Handler);
                    if (type2 == null)
                    {
                        MainForm.ShowMessage(typeof (DynamicEditor), MessageType.Warning,
                            string.Format(typeNotFoundMessage, handler.Handler));
                    }
                    else
                    {
                        editorTable[type] = Activator.CreateInstance(type2);
                    }
                }
            }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Type type = null;
            if (value == null)
            {
                var instance = context.Instance as TreeNodeProperty;
                if (instance != null)
                {
                    type = instance.Type;
                }
            }
            else
            {
                type = value.GetType();
            }
            if (type != null)
            {
                return GetEditor(type).EditValue(context, provider, value);
            }
            return base.EditValue(context, provider, value);
        }

        private Type GetContainedType(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                var instance = context.Instance as TreeNodeProperty;
                if (instance != null)
                {
                    return instance.Type;
                }
            }
            return null;
        }

        public static UITypeEditor GetEditor(Type type)
        {
            object obj2 = null;
            if (type != null)
            {
                if (typeof (DataSet).IsAssignableFrom(type))
                {
                    type = typeof (DataSet);
                }
                obj2 = editorTable[type];
            }
            if (obj2 == null)
            {
                obj2 = editorTable[typeof (object)];
            }
            return (obj2 as UITypeEditor);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return GetEditor(GetContainedType(context)).GetEditStyle(context);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return base.GetPaintValueSupported(context);
        }

        public static bool IsEditorDefined(Type type)
        {
            if (GetEditor(type).GetType().IsAssignableFrom(typeof (UITypeEditor)))
            {
                return false;
            }
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            base.PaintValue(e);
        }
    }
}